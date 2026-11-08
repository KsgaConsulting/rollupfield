using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Core.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.Validation;

namespace Ksga.RollupConfigurationEngine.Dataverse.Execution
{
    public static class UpdateTriggerFilter
    {
        public static bool ShouldRun(RollupConfiguration configuration, Entity target, string firstLookup)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var changed = new HashSet<string>(target.Attributes.Keys, StringComparer.OrdinalIgnoreCase);

            if (changed.Contains(firstLookup))
                return true;

            var triggers = TriggerFields(configuration);

            if (triggers.Count == 0)
                return configuration.Functions.Any(f => f.FunctionType == RollupFunctionType.Refresh);

            return triggers.Any(changed.Contains);
        }

        private static HashSet<string> TriggerFields(RollupConfiguration configuration)
        {
            var fields = new HashSet<string>(configuration.TriggerOnUpdateFields, StringComparer.OrdinalIgnoreCase);

            foreach (var function in configuration.Functions)
            {
                if (RollupFunctionValidationService.RequiresSource(function.FunctionType)
                    && !string.IsNullOrWhiteSpace(function.SourceChildField))
                {
                    fields.Add(function.SourceChildField!.Trim());
                }
            }

            return fields;
        }
    }
}
