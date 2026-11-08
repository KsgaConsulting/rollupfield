using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Core.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules;
using Ksga.RollupConfigurationEngine.Dataverse.Validation;

namespace Ksga.RollupConfigurationEngine.Dataverse.Steps
{
    public static class TriggerOnUpdateFields
    {
        public const int MaxLength = 500;

        private static readonly char[] Separators = { ',', ';', ' ', '\t', '\r', '\n' };

        public static IReadOnlyList<string> ParseList(string? value)
            => (value ?? string.Empty)
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim())
                .Where(n => n.Length > 0)
                .ToList();

        public static string Join(IEnumerable<string> names)
            => string.Join(",", names.Distinct(StringComparer.Ordinal).OrderBy(n => n, StringComparer.Ordinal));

        public static string Compute(
            IOrganizationService service,
            Guid configurationId,
            string? additional,
            Entity? pendingFunction,
            Guid? excludedFunctionId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var names = new List<string>(ParseList(additional));

            var query = new QueryExpression(RollupFunctionValidationService.FunctionEntity)
            {
                ColumnSet = new ColumnSet(
                    RollupFunctionValidationService.FunctionTypeField,
                    RollupFunctionValidationService.SourceField)
            };
            query.Criteria.AddCondition(
                RollupFunctionValidationService.ConfigurationField, ConditionOperator.Equal, configurationId);

            foreach (var function in service.RetrieveMultiple(query).Entities)
            {
                if (function.Id == excludedFunctionId || (pendingFunction != null && function.Id == pendingFunction.Id))
                    continue;

                AddSource(function, names);
            }

            if (pendingFunction != null)
                AddSource(pendingFunction, names);

            var result = Join(names);

            if (result.Length > MaxLength)
            {
                throw RollupException.Create(MessageCodes.TriggerFieldsTooLong, MaxLength);
            }

            return result;
        }

        public static void Refresh(
            IOrganizationService service, Guid configurationId, Entity? pendingFunction, Guid? excludedFunctionId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var configuration = service.Retrieve(
                RollupConfigurationFields.Entity,
                configurationId,
                new ColumnSet(
                    RollupConfigurationFields.AdditionalTriggerOnUpdate,
                    RollupConfigurationFields.TriggerOnUpdate));

            var computed = Compute(
                service,
                configurationId,
                configuration.GetAttributeValue<string>(RollupConfigurationFields.AdditionalTriggerOnUpdate),
                pendingFunction,
                excludedFunctionId);

            var current = configuration.GetAttributeValue<string>(RollupConfigurationFields.TriggerOnUpdate) ?? string.Empty;

            if (string.Equals(current, computed, StringComparison.Ordinal))
                return;

            var update = new Entity(RollupConfigurationFields.Entity, configurationId);
            update[RollupConfigurationFields.TriggerOnUpdate] = computed.Length == 0 ? null : computed;
            service.Update(update);
        }

        private static void AddSource(Entity function, List<string> names)
        {
            var type = function.GetAttributeValue<OptionSetValue>(RollupFunctionValidationService.FunctionTypeField);

            if (type == null
                || !Enum.IsDefined(typeof(RollupFunctionType), type.Value)
                || !RollupFunctionValidationService.RequiresSource((RollupFunctionType)type.Value))
            {
                return;
            }

            var source = function.GetAttributeValue<string>(RollupFunctionValidationService.SourceField);

            if (!string.IsNullOrWhiteSpace(source))
                names.Add(source!.Trim());
        }
    }
}
