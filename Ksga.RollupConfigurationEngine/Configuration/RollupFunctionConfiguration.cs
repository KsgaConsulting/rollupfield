using System;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public sealed class RollupFunctionConfiguration
    {
        public string TargetParentField { get; }
        public string? SourceChildField { get; }
        public RollupFunctionType FunctionType { get; }
        public string? Separator { get; }

        public bool ConvertCurrency { get; }

        public string? SourceFieldType { get; }

        public string? TargetFieldType { get; }

        public string? EffectiveSourceField =>
            ConvertCurrency && !string.IsNullOrWhiteSpace(SourceChildField)
                ? SourceChildField + "_base"
                : SourceChildField;

        public RollupFunctionConfiguration(
            string targetParentField,
            RollupFunctionType functionType,
            string? sourceChildField = null,
            string? separator = null,
            bool convertCurrency = false,
            string? sourceFieldType = null,
            string? targetFieldType = null)
        {
            TargetParentField = targetParentField
                ?? throw new ArgumentNullException(nameof(targetParentField));

            FunctionType = functionType;
            SourceChildField = sourceChildField;
            Separator = separator;
            ConvertCurrency = convertCurrency;
            SourceFieldType = sourceFieldType;
            TargetFieldType = targetFieldType;
        }
    }
}
