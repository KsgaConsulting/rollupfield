using System;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    internal static class RollupFieldTypes
    {
        public const int DecimalRank = 3;

        public static bool IsNumeric(string typeName)
        {
            return Rank(typeName) > 0;
        }

        public static bool IsDate(string typeName)
        {
            return Is(typeName, "DateTime");
        }

        public static bool IsBoolean(string typeName)
        {
            return Is(typeName, "Boolean");
        }

        public static bool IsText(string typeName)
        {
            return Is(typeName, "String") || Is(typeName, "Memo");
        }

        public static int Rank(string typeName)
        {
            if (Is(typeName, "Integer")) return 1;
            if (Is(typeName, "BigInt")) return 2;
            if (Is(typeName, "Decimal") || Is(typeName, "Money")) return DecimalRank;
            if (Is(typeName, "Double")) return 4;

            return 0;
        }

        private static bool Is(string typeName, string expected)
        {
            return string.Equals(typeName, expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
