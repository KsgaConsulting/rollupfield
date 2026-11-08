using System;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public static class RollupCurrencyConversion
    {
        public static bool IsApplicable(
            RollupFunctionType function,
            string? sourceType,
            string? targetType)
        {
            return SupportsAggregation(function)
                && IsMoney(sourceType)
                && IsMoney(targetType);
        }

        private static bool SupportsAggregation(RollupFunctionType function)
        {
            return function == RollupFunctionType.Sum
                || function == RollupFunctionType.Average
                || function == RollupFunctionType.Minimum
                || function == RollupFunctionType.Maximum;
        }

        private static bool IsMoney(string? typeName)
        {
            return string.Equals(typeName, "Money", StringComparison.OrdinalIgnoreCase);
        }
    }
}
