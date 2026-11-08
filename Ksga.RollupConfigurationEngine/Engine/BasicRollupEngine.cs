using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Ksga.RollupConfigurationEngine.Core.Configuration;

namespace Ksga.RollupConfigurationEngine.Core.Engine
{
    public sealed class BasicRollupEngine : IRollupEngine
    {
        public RollupCalculationResult Execute(
            RollupConfiguration configuration,
            RollupCalculationContext context)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var result = new RollupCalculationResult();

            foreach (var function in configuration.Functions)
            {
                switch (function.FunctionType)
                {
                    case RollupFunctionType.Count:
                        SetValue(function, result, context.SourceRecords.Count);
                        break;

                    case RollupFunctionType.Any:
                        SetValue(function, result, context.SourceRecords.Any());
                        break;

                    case RollupFunctionType.None:
                        SetValue(function, result, !context.SourceRecords.Any());
                        break;

                    case RollupFunctionType.Sum:
                        SetDecimal(function, context, result,
                            values => values.Sum());
                        break;

                    case RollupFunctionType.Average:
                        SetDecimal(function, context, result,
                            values => values.Any() ? values.Average() : 0m);
                        break;

                    case RollupFunctionType.Minimum:
                        SetMinimumMaximum(function, context, result, isMaximum: false);
                        break;

                    case RollupFunctionType.Maximum:
                        SetMinimumMaximum(function, context, result, isMaximum: true);
                        break;

                    case RollupFunctionType.And:
                        SetBooleanAnd(function, context, result);
                        break;

                    case RollupFunctionType.Or:
                        SetBooleanOr(function, context, result);
                        break;

                    case RollupFunctionType.Concat:
                        SetConcat(function, context, result);
                        break;

                    case RollupFunctionType.Refresh:
                        result.AddRefreshField(function.TargetParentField);
                        break;

                    default:
                        throw new NotSupportedException(
                            $"Function {function.FunctionType} is not supported.");
                }
            }

            return result;
        }

        private static void SetValue(
            RollupFunctionConfiguration function,
            RollupCalculationResult result,
            object? value)
        {
            result.SetValue(
                function.TargetParentField,
                RollupValueConverter.ToTargetType(value, function.TargetFieldType));
        }

        private static List<object> ReadValues(
            RollupFunctionConfiguration function,
            RollupCalculationContext context)
        {
            var field = function.EffectiveSourceField!;

            return context.SourceRecords
                .Where(r => r.ContainsKey(field) && r[field] != null)
                .Select(r => r[field])
                .ToList();
        }

        private static void SetDecimal(
            RollupFunctionConfiguration function,
            RollupCalculationContext context,
            RollupCalculationResult result,
            Func<List<decimal>, decimal> operation)
        {
            var values = ReadValues(function, context)
                .Select(Convert.ToDecimal)
                .ToList();

            SetValue(function, result, ApplyCurrency(function, context, operation(values)));
        }

        private static void SetMinimumMaximum(
            RollupFunctionConfiguration function,
            RollupCalculationContext context,
            RollupCalculationResult result,
            bool isMaximum)
        {
            var raw = ReadValues(function, context);

            if (IsDate(function, raw))
            {
                var dates = raw.Select(Convert.ToDateTime).ToList();

                object? date = null;
                if (dates.Any())
                    date = isMaximum ? dates.Max() : dates.Min();

                SetValue(function, result, date);
                return;
            }

            var values = raw.Select(Convert.ToDecimal).ToList();

            var value = !values.Any() ? 0m : isMaximum ? values.Max() : values.Min();

            SetValue(function, result, ApplyCurrency(function, context, value));
        }

        private static bool IsDate(RollupFunctionConfiguration function, List<object> values)
        {
            return string.Equals(function.SourceFieldType, "DateTime", StringComparison.OrdinalIgnoreCase)
                || values.Any(v => v is DateTime);
        }

        private static decimal ApplyCurrency(
            RollupFunctionConfiguration function,
            RollupCalculationContext context,
            decimal value)
        {
            if (!function.ConvertCurrency)
                return value;

            if (!context.ParentExchangeRate.HasValue || context.ParentExchangeRate.Value <= 0m)
            {
                throw RollupException.Create(MessageCodes.ExchangeRateNotFound, function.TargetParentField);
            }

            return value * context.ParentExchangeRate.Value;
        }

        private static void SetBooleanAnd(
            RollupFunctionConfiguration function,
            RollupCalculationContext context,
            RollupCalculationResult result)
        {
            var matching = ReadValues(function, context);

            var value = matching.Any() && matching.All(Convert.ToBoolean);

            SetValue(function, result, value);
        }

        private static void SetBooleanOr(
            RollupFunctionConfiguration function,
            RollupCalculationContext context,
            RollupCalculationResult result)
        {
            var value = ReadValues(function, context).Any(Convert.ToBoolean);

            SetValue(function, result, value);
        }

        private static void SetConcat(
            RollupFunctionConfiguration function,
            RollupCalculationContext context,
            RollupCalculationResult result)
        {
            var separator = function.Separator ?? " ";

            var value = string.Join(
                separator,
                ReadValues(function, context)
                    .Select(v => v.ToString())
                    .Where(v => !string.IsNullOrWhiteSpace(v)));

            SetValue(function, result, value);
        }
    }
}
