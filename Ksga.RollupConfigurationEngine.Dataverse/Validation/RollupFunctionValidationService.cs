using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Core.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.Metadata;

namespace Ksga.RollupConfigurationEngine.Dataverse.Validation
{
    public sealed class RollupFunctionValidationService
    {
        public const string FunctionEntity = "ksga_rollupfunction";
        public const string FunctionTypeField = "ksga_functiontypecode";
        public const string TargetField = "ksga_targetparentfield";
        public const string SourceField = "ksga_sourcechildfield";
        public const string SeparatorField = "ksga_separator";
        public const string ConfigurationField = "ksga_rollupconfigurationid";
        public const string SourceTypeField = "ksga_sourcechildfieldtype";
        public const string TargetTypeField = "ksga_targetparentfieldtype";
        public const string ConvertCurrencyField = "ksga_convertcurrency";

        private const string ConfigurationEntity = "ksga_rollupconfiguration";
        private const string ParentEntityField = "ksga_parententity";
        private const string ChildEntityField = "ksga_childentity";

        private static readonly Regex LogicalNamePattern = new Regex("^[a-z_][a-z0-9_]*$");

        private readonly IFieldMetadataProvider _metadata;

        public RollupFunctionValidationService()
            : this(new DataverseFieldMetadataProvider())
        {
        }

        public RollupFunctionValidationService(IFieldMetadataProvider metadata)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public static Entity Merge(Entity target, Entity? preImage)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var merged = new Entity(target.LogicalName, target.Id);

            if (preImage != null)
            {
                foreach (var attribute in preImage.Attributes)
                    merged[attribute.Key] = attribute.Value;
            }

            foreach (var attribute in target.Attributes)
                merged[attribute.Key] = attribute.Value;

            return merged;
        }

        public static void ClearUnusedFields(Entity target, Entity merged)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (merged == null) throw new ArgumentNullException(nameof(merged));

            var type = merged.GetAttributeValue<OptionSetValue>(FunctionTypeField);

            if (type == null || !Enum.IsDefined(typeof(RollupFunctionType), type.Value))
                return;

            var functionType = (RollupFunctionType)type.Value;

            if (!RequiresSource(functionType) && !string.IsNullOrEmpty(merged.GetAttributeValue<string>(SourceField)))
                target[SourceField] = null;

            if (functionType != RollupFunctionType.Concat && !string.IsNullOrEmpty(merged.GetAttributeValue<string>(SeparatorField)))
                target[SeparatorField] = null;
        }

        public RollupFunctionValidationResult Validate(IOrganizationService service, Entity function)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (function == null) throw new ArgumentNullException(nameof(function));

            var errors = new List<LocalizedMessage>();

            var functionType = ReadFunctionType(function, errors);
            var targetField = ReadFieldName(function, TargetField, true, errors);
            var needsSource = functionType.HasValue && RequiresSource(functionType.Value);
            var sourceField = ReadFieldName(function, SourceField, needsSource, errors);

            if (functionType == RollupFunctionType.Concat
                && string.IsNullOrEmpty(function.GetAttributeValue<string>(SeparatorField)))
            {
                errors.Add(new LocalizedMessage(MessageCodes.FunctionSeparatorRequired));
            }

            var configurationRef = function.GetAttributeValue<EntityReference>(ConfigurationField);

            if (configurationRef == null)
                errors.Add(new LocalizedMessage(MessageCodes.FunctionConfigurationRequired));

            if (errors.Count > 0 || functionType == null || configurationRef == null)
                return Failed(errors);

            var configuration = service.Retrieve(
                ConfigurationEntity,
                configurationRef.Id,
                new ColumnSet(ParentEntityField, ChildEntityField));

            var parentEntity = configuration.GetAttributeValue<string>(ParentEntityField);
            var childEntity = configuration.GetAttributeValue<string>(ChildEntityField);

            if (string.IsNullOrWhiteSpace(parentEntity) || string.IsNullOrWhiteSpace(childEntity))
            {
                errors.Add(new LocalizedMessage(MessageCodes.FunctionConfigurationEntitiesMissing));
                return Failed(errors);
            }

            var target = FindField(service, parentEntity, targetField!, true, errors);
            var source = needsSource
                ? FindField(service, childEntity, sourceField!, false, errors)
                : null;

            if (errors.Count > 0 || target == null)
                return Failed(errors);

            errors.AddRange(RollupFunctionTypeValidator.Validate(
                functionType.Value,
                needsSource ? sourceField : null,
                source,
                targetField!,
                target));

            if (errors.Count > 0)
                return Failed(errors);

            var sourceType = needsSource ? source?.TypeName : null;

            var requested = function.GetAttributeValue<bool?>(ConvertCurrencyField) == true;
            var convertCurrency = requested
                && RollupCurrencyConversion.IsApplicable(functionType.Value, sourceType, target.TypeName);

            return new RollupFunctionValidationResult(
                errors,
                sourceType,
                target.TypeName,
                convertCurrency);
        }

        public static bool RequiresSource(RollupFunctionType function)
        {
            switch (function)
            {
                case RollupFunctionType.Sum:
                case RollupFunctionType.Average:
                case RollupFunctionType.Minimum:
                case RollupFunctionType.Maximum:
                case RollupFunctionType.And:
                case RollupFunctionType.Or:
                case RollupFunctionType.Concat:
                    return true;

                default:
                    return false;
            }
        }

        private static RollupFunctionType? ReadFunctionType(Entity function, List<LocalizedMessage> errors)
        {
            var value = function.GetAttributeValue<OptionSetValue>(FunctionTypeField);

            if (value == null)
            {
                errors.Add(new LocalizedMessage(MessageCodes.FunctionTypeRequired));
                return null;
            }

            if (!Enum.IsDefined(typeof(RollupFunctionType), value.Value))
            {
                errors.Add(new LocalizedMessage(MessageCodes.FunctionTypeUnsupported, value.Value));
                return null;
            }

            return (RollupFunctionType)value.Value;
        }

        private static string? ReadFieldName(
            Entity function,
            string attribute,
            bool required,
            List<LocalizedMessage> errors)
        {
            if (!required)
                return null;

            var isTarget = attribute == TargetField;
            var value = function.GetAttributeValue<string>(attribute);

            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new LocalizedMessage(
                    isTarget ? MessageCodes.FunctionTargetRequired : MessageCodes.FunctionSourceRequired));
                return null;
            }

            if (!LogicalNamePattern.IsMatch(value))
            {
                errors.Add(new LocalizedMessage(
                    isTarget ? MessageCodes.FunctionTargetInvalidName : MessageCodes.FunctionSourceInvalidName,
                    value));
                return null;
            }

            return value;
        }

        private FieldTypeInfo? FindField(
            IOrganizationService service,
            string entity,
            string field,
            bool isTarget,
            List<LocalizedMessage> errors)
        {
            if (string.IsNullOrEmpty(field))
                return null;

            try
            {
                var info = _metadata.GetField(service, entity, field);

                if (info == null)
                    errors.Add(new LocalizedMessage(
                        isTarget ? MessageCodes.FunctionTargetNotFound : MessageCodes.FunctionSourceNotFound,
                        field,
                        entity));

                return info;
            }
            catch (Exception ex)
            {
                errors.Add(new LocalizedMessage(
                    isTarget ? MessageCodes.FunctionParentEntityUnreadable : MessageCodes.FunctionChildEntityUnreadable,
                    entity,
                    ex.Message));
                return null;
            }
        }

        private static RollupFunctionValidationResult Failed(List<LocalizedMessage> errors)
        {
            return new RollupFunctionValidationResult(errors, null, null);
        }
    }
}
