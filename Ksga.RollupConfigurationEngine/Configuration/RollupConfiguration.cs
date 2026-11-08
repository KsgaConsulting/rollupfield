using System;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public sealed class RollupConfiguration
    {
        public string ParentEntityLogicalName { get; }
        public string ChildEntityLogicalName { get; }
        public string ParentLookupAttribute { get; }

        public string? ParentExecutionFilterXml { get; }

        public string? ChildFilterXml { get; }

        public IReadOnlyCollection<RollupFunctionConfiguration> Functions { get; }

        public IReadOnlyList<string> TriggerOnUpdateFields { get; }

        public RollupConfiguration(
            string parentEntityLogicalName,
            string childEntityLogicalName,
            string parentLookupAttribute,
            IEnumerable<RollupFunctionConfiguration> functions,
            string? parentExecutionFilterXml = null,
            string? childFilterXml = null,
            string? triggerOnUpdate = null)
        {
            ParentEntityLogicalName = parentEntityLogicalName
                ?? throw new ArgumentNullException(nameof(parentEntityLogicalName));

            ChildEntityLogicalName = childEntityLogicalName
                ?? throw new ArgumentNullException(nameof(childEntityLogicalName));

            ParentLookupAttribute = parentLookupAttribute
                ?? throw new ArgumentNullException(nameof(parentLookupAttribute));

            Functions = functions != null
                ? new List<RollupFunctionConfiguration>(functions)
                : throw new ArgumentNullException(nameof(functions));

            ParentExecutionFilterXml = parentExecutionFilterXml;
            ChildFilterXml = childFilterXml;

            TriggerOnUpdateFields = (triggerOnUpdate ?? string.Empty)
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
