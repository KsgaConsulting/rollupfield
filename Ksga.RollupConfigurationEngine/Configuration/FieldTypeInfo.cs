using System;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public sealed class FieldTypeInfo
    {
        public string TypeName { get; }
        public bool IsRollup { get; }
        public bool IsCalculated { get; }

        public FieldTypeInfo(string typeName, bool isRollup = false, bool isCalculated = false)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            IsRollup = isRollup;
            IsCalculated = isCalculated;
        }
    }
}
