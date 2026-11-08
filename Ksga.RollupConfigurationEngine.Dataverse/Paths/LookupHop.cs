using System;

namespace Ksga.RollupConfigurationEngine.Dataverse.Paths
{
    public sealed class LookupHop
    {
        public string EntityLogicalName { get; }

        public string AttributeName { get; }

        public LookupHop(string entityLogicalName, string attributeName)
        {
            EntityLogicalName = entityLogicalName
                ?? throw new ArgumentNullException(nameof(entityLogicalName));
            AttributeName = attributeName
                ?? throw new ArgumentNullException(nameof(attributeName));
        }
    }
}
