using System;

namespace Ksga.RollupConfigurationEngine.Core.Engine
{
    internal static class RollupValueConverter
    {
        public static object? ToTargetType(object? value, string? targetType)
        {
            if (value == null || string.IsNullOrWhiteSpace(targetType))
                return value;

            switch (targetType!.Trim().ToLowerInvariant())
            {
                case "integer":
                    return IsNumeric(value)
                        ? decimal.ToInt32(decimal.Round(Convert.ToDecimal(value), MidpointRounding.AwayFromZero))
                        : value;

                case "bigint":
                    return IsNumeric(value)
                        ? decimal.ToInt64(decimal.Round(Convert.ToDecimal(value), MidpointRounding.AwayFromZero))
                        : value;

                case "decimal":
                case "money":
                    return IsNumeric(value) ? Convert.ToDecimal(value) : value;

                case "double":
                    return IsNumeric(value) ? Convert.ToDouble(value) : value;

                case "string":
                case "memo":
                    return value is string ? value : Convert.ToString(value);

                default:
                    return value;
            }
        }

        private static bool IsNumeric(object value)
        {
            return value is int || value is long || value is short || value is byte
                || value is decimal || value is double || value is float;
        }
    }
}
