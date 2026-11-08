using System;
using System.Collections.Generic;
using System.Linq;

namespace Ksga.RollupConfigurationEngine.Dataverse.Paths
{
    public sealed class DataverseLookupPath
    {
        public string ChildEntity { get; }

        public IReadOnlyList<LookupHop> Hops { get; }

        public bool IsMultiLevel => Hops.Count > 1;

        public DataverseLookupPath(string childEntity, IEnumerable<LookupHop> hops)
        {
            if (string.IsNullOrWhiteSpace(childEntity))
                throw new ArgumentNullException(nameof(childEntity));

            if (hops == null)
                throw new ArgumentNullException(nameof(hops));

            var list = hops.ToList();

            if (list.Count == 0)
                throw new InvalidOperationException(
                    "Lookup path must contain at least one hop.");

            ChildEntity = childEntity;
            Hops = list;
        }

        public string ToDotPath()
            => string.Join(".", Hops.Select(h => h.AttributeName));
    }
}
