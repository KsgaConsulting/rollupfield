using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules
{
    public sealed class ImmutableFieldsRule : IRollupConfigurationRule
    {
        private static readonly string[] Fields =
        {
            RollupConfigurationFields.Relations,
            RollupConfigurationFields.ParentEntity,
            RollupConfigurationFields.ChildEntity,
            RollupConfigurationFields.LookupPath
        };

        public void Apply(RollupConfigurationRuleContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.IsUpdate)
                return;

            var changed = new List<string>();

            foreach (var field in Fields)
            {
                if (!context.Target.Contains(field))
                    continue;

                var previous = context.PreImage?.GetAttributeValue<string>(field);
                var current = context.Target.GetAttributeValue<string>(field);

                if (!string.Equals(Normalize(previous), Normalize(current), StringComparison.Ordinal))
                    changed.Add(field);
            }

            if (changed.Count == 0)
                return;

            context.Trace("Immutable fields changed: " + string.Join(", ", changed));

            throw RollupException.Create(MessageCodes.ConfigurationImmutableFields, string.Join(", ", changed));
        }

        private static string Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value!.Trim();
    }
}
