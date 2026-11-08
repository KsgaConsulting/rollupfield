using System;
using System.Linq;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public static class RollupConfigurationValidator
    {
        public static void Validate(RollupConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            ValidateStructure(configuration);
            ValidateFunctions(configuration);
        }

        private static void ValidateStructure(RollupConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.ParentEntityLogicalName))
                throw new InvalidOperationException("ParentEntityLogicalName is mandatory.");

            if (string.IsNullOrWhiteSpace(configuration.ChildEntityLogicalName))
                throw new InvalidOperationException("ChildEntityLogicalName is mandatory.");

            if (string.IsNullOrWhiteSpace(configuration.ParentLookupAttribute))
                throw new InvalidOperationException("ParentLookupAttribute is mandatory.");

            if (configuration.Functions == null || !configuration.Functions.Any())
                throw new InvalidOperationException(
                    "At least one rollup function must be defined.");
        }

        private static void ValidateFunctions(RollupConfiguration configuration)
        {
            var noSourceFieldRequired = new[]
            {
                RollupFunctionType.Count,
                RollupFunctionType.Any,
                RollupFunctionType.None,
                RollupFunctionType.Refresh
            };

            foreach (var function in configuration.Functions)
            {
                if (string.IsNullOrWhiteSpace(function.TargetParentField))
                    throw new InvalidOperationException("TargetParentField is mandatory.");

                if (!noSourceFieldRequired.Contains(function.FunctionType)
                    && string.IsNullOrWhiteSpace(function.SourceChildField))
                {
                    throw new InvalidOperationException(
                        $"SourceChildField is mandatory for function {function.FunctionType}.");
                }
            }
        }
    }
}
