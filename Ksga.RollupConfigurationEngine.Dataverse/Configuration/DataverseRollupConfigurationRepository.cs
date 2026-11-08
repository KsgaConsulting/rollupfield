using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Core.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.Mapping;

namespace Ksga.RollupConfigurationEngine.Dataverse.Configuration
{
    public sealed class DataverseRollupConfigurationRepository
    {
        public IEnumerable<RollupConfiguration> GetForChildEntity(
            IOrganizationService service,
            string childEntityLogicalName,
            string messageName,
            RollupExecutionMode executionMode = RollupExecutionMode.Synchronous)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrWhiteSpace(childEntityLogicalName))
                throw new ArgumentNullException(nameof(childEntityLogicalName));

            var query = new QueryExpression("ksga_rollupconfiguration")
            {
                ColumnSet = new ColumnSet(
                    "ksga_childentity",
                    "ksga_parententity",
                    "ksga_lookuppath",
                    "ksga_childfilter",
                    "ksga_parentexecutionfilter",
                    "ksga_triggeroncreate",
                    "ksga_triggerondelete",
                    "ksga_triggeronupdate",
                    "ksga_executionmode"
                )
            };

            query.Criteria.AddCondition(
                "ksga_childentity", ConditionOperator.Equal, childEntityLogicalName);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            query.AddOrder("createdon", OrderType.Ascending);
            query.AddOrder("ksga_rollupconfigurationid", OrderType.Ascending);

            if (messageName == "Create")
                query.Criteria.AddCondition(
                    "ksga_triggeroncreate", ConditionOperator.Equal, true);

            if (messageName == "Delete")
                query.Criteria.AddCondition(
                    "ksga_triggerondelete", ConditionOperator.Equal, true);

            var configs = service.RetrieveMultiple(query).Entities
                .Where(c => ModeOf(c) == executionMode);

            foreach (var configEntity in configs)
            {
                yield return BuildConfiguration(service, configEntity);
            }
        }

        public static RollupExecutionMode ModeOf(Entity configEntity)
            => configEntity.GetAttributeValue<bool>("ksga_executionmode")
                ? RollupExecutionMode.Asynchronous
                : RollupExecutionMode.Synchronous;

        private RollupConfiguration BuildConfiguration(
            IOrganizationService service,
            Entity configEntity)
        {
            var functions = LoadFunctions(service, configEntity.Id);

            return new RollupConfiguration(
                parentEntityLogicalName:
                    configEntity.GetAttributeValue<string>("ksga_parententity"),
                childEntityLogicalName:
                    configEntity.GetAttributeValue<string>("ksga_childentity"),
                parentLookupAttribute:
                    configEntity.GetAttributeValue<string>("ksga_lookuppath"),
                functions: functions,
                parentExecutionFilterXml:
                    configEntity.GetAttributeValue<string>("ksga_parentexecutionfilter"),
                childFilterXml:
                    configEntity.GetAttributeValue<string>("ksga_childfilter"),
                triggerOnUpdate:
                    configEntity.GetAttributeValue<string>("ksga_triggeronupdate")
            );
        }

        private IReadOnlyCollection<RollupFunctionConfiguration> LoadFunctions(
            IOrganizationService service,
            Guid configurationId)
        {
            var query = new QueryExpression("ksga_rollupfunction")
            {
                ColumnSet = new ColumnSet(
                    "ksga_functiontypecode",
                    "ksga_sourcechildfield",
                    "ksga_targetparentfield",
                    "ksga_separator",
                    "ksga_convertcurrency",
                    "ksga_sourcechildfieldtype",
                    "ksga_targetparentfieldtype"
                )
            };

            query.Criteria.AddCondition(
                "ksga_rollupconfigurationid",
                ConditionOperator.Equal,
                configurationId);

            return service.RetrieveMultiple(query)
                .Entities
                .Select(RollupConfigurationMapper.MapFunction)
                .ToList();
        }
    }
}
