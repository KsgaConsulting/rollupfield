using System;
using System.Collections.Generic;
using Ksga.RollupConfigurationEngine.Core.Localization;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public static class RollupFunctionTypeValidator
    {
        public static IReadOnlyList<LocalizedMessage> Validate(
            RollupFunctionType function,
            string? sourceField,
            FieldTypeInfo? source,
            string targetField,
            FieldTypeInfo target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var errors = new List<LocalizedMessage>();

            if (function != RollupFunctionType.Refresh && (target.IsRollup || target.IsCalculated))
            {
                errors.Add(new LocalizedMessage(
                    target.IsRollup ? MessageCodes.ValidationTargetIsRollup : MessageCodes.ValidationTargetIsCalculated,
                    targetField,
                    function));
                return errors;
            }

            switch (function)
            {
                case RollupFunctionType.Sum:
                    RequireNumericSource(function, sourceField, source, errors);
                    RequireNumericTarget(function, targetField, target, errors);
                    RequireNoNarrowing(function, sourceField, source, targetField, target, errors);
                    break;

                case RollupFunctionType.Average:
                    RequireNumericSource(function, sourceField, source, errors);
                    RequireNumericTarget(function, targetField, target, errors);
                    RequireFractionalTarget(targetField, target, errors);
                    break;

                case RollupFunctionType.Minimum:
                case RollupFunctionType.Maximum:
                    ValidateMinimumMaximum(function, sourceField, source, targetField, target, errors);
                    break;

                case RollupFunctionType.Count:
                    RequireNumericTarget(function, targetField, target, errors);
                    break;

                case RollupFunctionType.Any:
                case RollupFunctionType.None:
                    RequireTarget(function, targetField, target, RollupFieldTypes.IsBoolean, MessageCodes.TypeBoolean, errors);
                    break;

                case RollupFunctionType.And:
                case RollupFunctionType.Or:
                    RequireSource(function, sourceField, source, RollupFieldTypes.IsBoolean, MessageCodes.TypeBoolean, errors);
                    RequireTarget(function, targetField, target, RollupFieldTypes.IsBoolean, MessageCodes.TypeBoolean, errors);
                    break;

                case RollupFunctionType.Concat:
                    RequireSource(function, sourceField, source, RollupFieldTypes.IsText, MessageCodes.TypeText, errors);
                    RequireTarget(function, targetField, target, RollupFieldTypes.IsText, MessageCodes.TypeText, errors);
                    break;

                case RollupFunctionType.Refresh:
                    if (!target.IsRollup)
                    {
                        errors.Add(new LocalizedMessage(MessageCodes.ValidationTargetNotRollup, targetField));
                    }
                    break;

                default:
                    errors.Add(new LocalizedMessage(MessageCodes.ValidationFunctionUnsupported, function));
                    break;
            }

            return errors;
        }

        private static void ValidateMinimumMaximum(
            RollupFunctionType function,
            string? sourceField,
            FieldTypeInfo? source,
            string targetField,
            FieldTypeInfo target,
            List<LocalizedMessage> errors)
        {
            if (source != null && RollupFieldTypes.IsDate(source.TypeName))
            {
                RequireTarget(function, targetField, target, RollupFieldTypes.IsDate, MessageCodes.TypeDate, errors);
                return;
            }

            if (source == null || !RollupFieldTypes.IsNumeric(source.TypeName))
            {
                errors.Add(new LocalizedMessage(
                    MessageCodes.ValidationSourceNumericOrDate, sourceField, Describe(source), function));
                return;
            }

            RequireNumericTarget(function, targetField, target, errors);
            RequireNoNarrowing(function, sourceField, source, targetField, target, errors);
        }

        private static void RequireNumericSource(
            RollupFunctionType function,
            string? sourceField,
            FieldTypeInfo? source,
            List<LocalizedMessage> errors)
        {
            RequireSource(function, sourceField, source, RollupFieldTypes.IsNumeric, MessageCodes.TypeNumeric, errors);
        }

        private static void RequireNumericTarget(
            RollupFunctionType function,
            string targetField,
            FieldTypeInfo target,
            List<LocalizedMessage> errors)
        {
            RequireTarget(function, targetField, target, RollupFieldTypes.IsNumeric, MessageCodes.TypeNumeric, errors);
        }

        private static void RequireSource(
            RollupFunctionType function,
            string? sourceField,
            FieldTypeInfo? source,
            Func<string, bool> accepts,
            string expected,
            List<LocalizedMessage> errors)
        {
            if (source == null || !accepts(source.TypeName))
            {
                errors.Add(new LocalizedMessage(
                    MessageCodes.ValidationSourceType, sourceField, Describe(source), new LocalizedMessage(expected), function));
            }
        }

        private static void RequireTarget(
            RollupFunctionType function,
            string targetField,
            FieldTypeInfo target,
            Func<string, bool> accepts,
            string expected,
            List<LocalizedMessage> errors)
        {
            if (!accepts(target.TypeName))
            {
                errors.Add(new LocalizedMessage(
                    MessageCodes.ValidationTargetType, targetField, target.TypeName, new LocalizedMessage(expected), function));
            }
        }

        private static void RequireFractionalTarget(
            string targetField,
            FieldTypeInfo target,
            List<LocalizedMessage> errors)
        {
            if (RollupFieldTypes.IsNumeric(target.TypeName) && RollupFieldTypes.Rank(target.TypeName) < RollupFieldTypes.DecimalRank)
            {
                errors.Add(new LocalizedMessage(MessageCodes.ValidationAveragePrecision, targetField, target.TypeName));
            }
        }

        private static void RequireNoNarrowing(
            RollupFunctionType function,
            string? sourceField,
            FieldTypeInfo? source,
            string targetField,
            FieldTypeInfo target,
            List<LocalizedMessage> errors)
        {
            if (source == null
                || !RollupFieldTypes.IsNumeric(source.TypeName)
                || !RollupFieldTypes.IsNumeric(target.TypeName))
            {
                return;
            }

            if (RollupFieldTypes.Rank(target.TypeName) < RollupFieldTypes.Rank(source.TypeName))
            {
                errors.Add(new LocalizedMessage(
                    MessageCodes.ValidationNarrowing, targetField, target.TypeName, sourceField, source.TypeName, function));
            }
        }

        private static string Describe(FieldTypeInfo? field)
        {
            return field == null ? "?" : field.TypeName;
        }
    }
}
