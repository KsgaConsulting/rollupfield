using System;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Core.Engine
{
    public sealed class RollupCalculationResult
    {
        private readonly Dictionary<string, object?> _values;
        private readonly List<string> _refreshFields;

        public IReadOnlyDictionary<string, object?> Values => _values;

        public IReadOnlyList<string> RefreshFields => _refreshFields;

        public RollupCalculationResult()
        {
            _values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            _refreshFields = new List<string>();
        }

        public void SetValue(string parentFieldLogicalName, object? value)
        {
            if (string.IsNullOrWhiteSpace(parentFieldLogicalName))
                throw new ArgumentException(
                    "Parent field name cannot be null or empty.",
                    nameof(parentFieldLogicalName));

            _values[parentFieldLogicalName] = value;
        }

        public void AddRefreshField(string parentFieldLogicalName)
        {
            if (string.IsNullOrWhiteSpace(parentFieldLogicalName))
                throw new ArgumentException(
                    "Parent field name cannot be null or empty.",
                    nameof(parentFieldLogicalName));

            _refreshFields.Add(parentFieldLogicalName);
        }

        public bool HasValues => _values.Count > 0;
        public bool HasRefreshFields => _refreshFields.Count > 0;
    }
}
