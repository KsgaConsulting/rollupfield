using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Core.Engine;
using Ksga.RollupConfigurationEngine.Dataverse.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.Fetch;
using Ksga.RollupConfigurationEngine.Dataverse.Paths;

namespace Ksga.RollupConfigurationEngine.Dataverse.Execution
{
    public sealed class DataverseRollupOrchestrationService
    {
        private readonly DataverseRollupConfigurationRepository _configRepository;
        private readonly DataverseRollupExecutionService _executionService;

        public DataverseRollupOrchestrationService()
            : this(
                new DataverseRollupConfigurationRepository(),
                new DataverseRollupExecutionService(new BasicRollupEngine()))
        {
        }

        internal DataverseRollupOrchestrationService(
            DataverseRollupConfigurationRepository configRepository,
            DataverseRollupExecutionService executionService)
        {
            _configRepository = configRepository
                ?? throw new ArgumentNullException(nameof(configRepository));
            _executionService = executionService
                ?? throw new ArgumentNullException(nameof(executionService));
        }

        public void ExecuteForChildTrigger(
            IOrganizationService service,
            Entity child,
            string messageName,
            ITracingService? tracing = null,
            Entity? preImage = null,
            RollupExecutionMode executionMode = RollupExecutionMode.Synchronous)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (child == null) throw new ArgumentNullException(nameof(child));

            tracing?.Trace($"Orchestration START — entity: {child.LogicalName}, message: {messageName}, mode: {executionMode}");

            var configs = _configRepository.GetForChildEntity(
                service, child.LogicalName, messageName, executionMode);

            var accumulators = new Dictionary<(string, Guid), ParentUpdateAccumulator>();

            foreach (var config in configs)
            {
                var hops = LookupPathParser.Parse(
                    config.ParentLookupAttribute, config.ParentEntityLogicalName);

                if (string.Equals(messageName, "Update", StringComparison.OrdinalIgnoreCase)
                    && !UpdateTriggerFilter.ShouldRun(config, child, hops[0].AttributeName))
                {
                    tracing?.Trace(
                        $"Configuration on {config.ParentEntityLogicalName} skipped: no trigger field modified.");
                    continue;
                }

                foreach (var parentRef in ResolveParentReferences(
                    service, child, preImage, hops, tracing))
                {
                    if (!SatisfiesParentFilter(service, parentRef, config.ParentExecutionFilterXml))
                    {
                        tracing?.Trace(
                            $"Parent {parentRef.LogicalName} {parentRef.Id} does not satisfy the execution filter: rollup skipped.");
                        continue;
                    }

                    tracing?.Trace(
                        $"Executing rollup → parent {parentRef.LogicalName} {parentRef.Id}");

                    var result = _executionService.Execute(service, parentRef, config);

                    var key = (parentRef.LogicalName, parentRef.Id);
                    if (!accumulators.TryGetValue(key, out var acc))
                    {
                        acc = new ParentUpdateAccumulator(parentRef);
                        accumulators[key] = acc;
                    }

                    acc.Merge(result);
                }
            }

            foreach (var acc in accumulators.Values)
            {
                if (acc.HasValues)
                {
                    tracing?.Trace($"Updating parent {acc.Parent.LogicalName} {acc.Parent.Id}");
                    service.Update(acc.ToEntity());
                }
                else
                {
                    tracing?.Trace(
                        $"No values to write for parent {acc.Parent.LogicalName} {acc.Parent.Id} (Refresh only).");
                }
            }

            tracing?.Trace("Orchestration END");
        }

        private static bool SatisfiesParentFilter(
            IOrganizationService service,
            EntityReference parent,
            string? filterXml)
        {
            if (string.IsNullOrWhiteSpace(filterXml))
                return true;

            var fetchXml = ParentExecutionFilterQuery.Build(parent.LogicalName, parent.Id, filterXml!);

            return service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities.Count > 0;
        }

        private static List<EntityReference> ResolveParentReferences(
            IOrganizationService service,
            Entity child,
            Entity? preImage,
            IReadOnlyList<LookupHop> hops,
            ITracingService? tracing)
        {
            var lookupAttribute = hops[0].AttributeName;
            var parents = new List<EntityReference>();
            var isDelete = ReferenceEquals(child, preImage);

            EntityReference? current;
            if (child.Attributes.TryGetValue(lookupAttribute, out var value))
            {
                current = value as EntityReference;
            }
            else if (preImage != null && preImage.Attributes.TryGetValue(lookupAttribute, out var pre))
            {
                current = pre as EntityReference;
            }
            else if (child.Id != Guid.Empty)
            {
                current = service
                    .Retrieve(child.LogicalName, child.Id, new ColumnSet(lookupAttribute))
                    .GetAttributeValue<EntityReference>(lookupAttribute);
            }
            else
            {
                current = null;
            }

            if (current != null)
                parents.Add(current);
            else
                tracing?.Trace($"Lookup '{lookupAttribute}' has no current value on child.");

            if (preImage != null
                && !isDelete
                && preImage.Attributes.TryGetValue(lookupAttribute, out var oldValue)
                && oldValue is EntityReference previous
                && (current == null
                    || previous.Id != current.Id
                    || previous.LogicalName != current.LogicalName))
            {
                tracing?.Trace(
                    $"Lookup '{lookupAttribute}' changed: also recalculating previous parent {previous.LogicalName} {previous.Id}.");
                parents.Add(previous);
            }

            return hops.Count == 1 ? parents : ClimbToParents(service, parents, hops, tracing);
        }

        private static List<EntityReference> ClimbToParents(
            IOrganizationService service,
            List<EntityReference> firstLevel,
            IReadOnlyList<LookupHop> hops,
            ITracingService? tracing)
        {
            var parents = new List<EntityReference>();

            foreach (var start in firstLevel)
            {
                EntityReference? reference = start;

                for (var i = 1; i < hops.Count && reference != null; i++)
                {
                    var attribute = hops[i].AttributeName;

                    reference = service
                        .Retrieve(hops[i - 1].EntityLogicalName, reference.Id, new ColumnSet(attribute))
                        .GetAttributeValue<EntityReference>(attribute);

                    if (reference == null)
                        tracing?.Trace($"Lookup '{attribute}' has no value on {hops[i - 1].EntityLogicalName} {start.Id}.");
                }

                if (reference != null && !parents.Exists(p => p.Id == reference.Id && p.LogicalName == reference.LogicalName))
                    parents.Add(reference);
            }

            return parents;
        }
    }
}
