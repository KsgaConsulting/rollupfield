using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Dataverse.Metadata;
using Ksga.RollupConfigurationEngine.Dataverse.Steps;

namespace Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules
{
    public sealed class AdditionalTriggerFieldsRule : IRollupConfigurationRule
    {
        private static readonly Regex LogicalNamePattern = new Regex("^[a-z_][a-z0-9_]*$");

        private readonly IFieldMetadataProvider _metadata;

        public AdditionalTriggerFieldsRule()
            : this(new DataverseFieldMetadataProvider())
        {
        }

        public AdditionalTriggerFieldsRule(IFieldMetadataProvider metadata)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public void Apply(RollupConfigurationRuleContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.IsCreate && !context.IsUpdate)
                return;

            if (!context.Target.Contains(RollupConfigurationFields.AdditionalTriggerOnUpdate))
                return;

            var names = TriggerOnUpdateFields
                .ParseList(context.Target.GetAttributeValue<string>(RollupConfigurationFields.AdditionalTriggerOnUpdate))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            var invalid = names.Where(n => !LogicalNamePattern.IsMatch(n)).ToList();

            if (invalid.Count > 0)
                throw Refuse(context, MessageCodes.AdditionalTriggersInvalidNames, string.Join(", ", invalid));

            var childEntity = context.Target.GetAttributeValue<string>(RollupConfigurationFields.ChildEntity)
                ?? context.PreImage?.GetAttributeValue<string>(RollupConfigurationFields.ChildEntity);

            if (!string.IsNullOrWhiteSpace(childEntity))
                CheckExistence(context, childEntity!, names);

            var joined = TriggerOnUpdateFields.Join(names);

            context.Target[RollupConfigurationFields.AdditionalTriggerOnUpdate] = joined.Length == 0 ? null : joined;

            var computed = TriggerOnUpdateFields.Compute(
                context.Service,
                context.Target.Id,
                joined,
                null);

            context.Target[RollupConfigurationFields.TriggerOnUpdate] = computed.Length == 0 ? null : computed;
        }

        private void CheckExistence(RollupConfigurationRuleContext context, string childEntity, List<string> names)
        {
            var missing = new List<string>();

            foreach (var name in names)
            {
                CheckField(context, childEntity, name, missing);
            }

            if (missing.Count > 0)
                throw Refuse(context, MessageCodes.AdditionalTriggersMissingFields, childEntity, string.Join(", ", missing));
        }

        private void CheckField(
            RollupConfigurationRuleContext context, string childEntity, string name, List<string> missing)
        {
            try
            {
                if (_metadata.GetField(context.Service, childEntity, name) == null)
                    missing.Add(name);
            }
            catch (RollupException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw Refuse(context, MessageCodes.AdditionalTriggersEntityUnreadable, childEntity, exception.Message);
            }
        }

        private static RollupException Refuse(RollupConfigurationRuleContext context, string code, params object?[] args)
        {
            var exception = RollupException.Create(code, args);

            context.Trace("Additional trigger fields rejected: " + exception.Message);

            return exception;
        }
    }
}
