using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Core.Engine;

namespace Ksga.RollupConfigurationEngine.Dataverse.Execution
{
    public sealed class ParentUpdateAccumulator
    {
        public EntityReference Parent { get; }
        private readonly Dictionary<string, object> _values;

        public ParentUpdateAccumulator(EntityReference parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public bool HasValues => _values.Count > 0;

        public void Merge(RollupCalculationResult result)
        {
            foreach (var kvp in result.Values)
            {
                _values[kvp.Key] = kvp.Value;
            }
        }

        public Entity ToEntity()
        {
            var entity = new Entity(Parent.LogicalName, Parent.Id);

            foreach (var kvp in _values)
            {
                entity[kvp.Key] = kvp.Value;
            }

            return entity;
        }
    }
}
