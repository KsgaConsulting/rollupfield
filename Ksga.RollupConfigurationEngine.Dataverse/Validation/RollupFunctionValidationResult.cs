using Ksga.RollupConfigurationEngine.Core.Localization;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Dataverse.Validation
{
    public sealed class RollupFunctionValidationResult
    {
        public IReadOnlyList<LocalizedMessage> Errors { get; }
        public string? SourceType { get; }
        public string? TargetType { get; }
        public bool ConvertCurrency { get; }

        public bool IsValid => Errors.Count == 0;

        public RollupFunctionValidationResult(
            IReadOnlyList<LocalizedMessage> errors,
            string? sourceType,
            string? targetType,
            bool convertCurrency = false)
        {
            Errors = errors;
            SourceType = sourceType;
            TargetType = targetType;
            ConvertCurrency = convertCurrency;
        }
    }
}
