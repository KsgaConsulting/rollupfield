using System;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Core.Engine
{
    public sealed class RollupCalculationContext
    {
        public IReadOnlyList<IDictionary<string, object>> SourceRecords { get; }

        public decimal? ParentExchangeRate { get; }

        public RollupCalculationContext(
            IReadOnlyList<IDictionary<string, object>> sourceRecords,
            decimal? parentExchangeRate = null)
        {
            SourceRecords = sourceRecords
                ?? throw new ArgumentNullException(nameof(sourceRecords));

            ParentExchangeRate = parentExchangeRate;
        }
    }
}
