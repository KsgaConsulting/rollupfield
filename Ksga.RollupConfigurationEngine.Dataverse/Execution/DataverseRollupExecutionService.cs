using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Core.Configuration;
using Ksga.RollupConfigurationEngine.Core.Engine;
using Ksga.RollupConfigurationEngine.Dataverse.Fetch;
using Ksga.RollupConfigurationEngine.Dataverse.Mapping;
using Ksga.RollupConfigurationEngine.Dataverse.Paths;

namespace Ksga.RollupConfigurationEngine.Dataverse.Execution
{
    public sealed class DataverseRollupExecutionService
    {
        private const int PageSize = 5000;

        private readonly IRollupEngine _rollupEngine;

        public DataverseRollupExecutionService(IRollupEngine rollupEngine)
        {
            _rollupEngine = rollupEngine
                ?? throw new ArgumentNullException(nameof(rollupEngine));
        }

        public RollupCalculationResult Execute(
            IOrganizationService service,
            EntityReference parent,
            RollupConfiguration configuration,
            string? childFilterXml = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            RollupConfigurationValidator.Validate(configuration);

            var onlyRefresh = configuration.Functions
                .All(f => f.FunctionType == RollupFunctionType.Refresh);

            if (onlyRefresh)
            {
                var refreshResult = _rollupEngine.Execute(
                    configuration,
                    new RollupCalculationContext(new List<IDictionary<string, object>>()));

                RefreshNativeFields(service, parent, refreshResult);

                return refreshResult;
            }

            var effectiveChildFilter = childFilterXml ?? configuration.ChildFilterXml;

            var attributesToSelect = configuration.Functions
                .Select(f => f.EffectiveSourceField)
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Distinct()
                .ToList()!;

            var lookupPath = LookupPathParser.Parse(
                configuration.ParentLookupAttribute,
                configuration.ParentEntityLogicalName);

            var fetchBuilder = new FetchXmlBuilder(
                childEntity: configuration.ChildEntityLogicalName,
                lookupPath: lookupPath,
                attributesToSelect: attributesToSelect,
                childFilterXml: effectiveChildFilter
            );

            var fetchXml = fetchBuilder.Build(parent.Id);

            var records = DistinctById(RetrieveAll(service, fetchXml))
                .Select(DataverseChildRecordMapper.Map)
                .ToList<IDictionary<string, object>>();

            var context = new RollupCalculationContext(
                records,
                ReadParentExchangeRate(service, parent, configuration));

            var result = _rollupEngine.Execute(configuration, context);

            RefreshNativeFields(service, parent, result);

            return result;
        }

        private static IEnumerable<Entity> DistinctById(IEnumerable<Entity> entities)
        {
            var seen = new HashSet<Guid>();

            foreach (var entity in entities)
            {
                if (entity.Id == Guid.Empty || seen.Add(entity.Id))
                    yield return entity;
            }
        }

        private static decimal? ReadParentExchangeRate(
            IOrganizationService service,
            EntityReference parent,
            RollupConfiguration configuration)
        {
            if (!configuration.Functions.Any(f => f.ConvertCurrency))
                return null;

            return service
                .Retrieve(parent.LogicalName, parent.Id, new ColumnSet("exchangerate"))
                .GetAttributeValue<decimal?>("exchangerate");
        }

        private static void RefreshNativeFields(
            IOrganizationService service,
            EntityReference parent,
            RollupCalculationResult result)
        {
            foreach (var field in result.RefreshFields)
            {
                service.Execute(new CalculateRollupFieldRequest
                {
                    Target = parent,
                    FieldName = field
                });
            }
        }

        private static List<Entity> RetrieveAll(IOrganizationService service, string fetchXml)
        {
            var document = XDocument.Parse(fetchXml);
            var fetch = document.Root!;
            var all = new List<Entity>();
            var page = 1;
            string? cookie = null;

            while (true)
            {
                fetch.SetAttributeValue("count", PageSize);
                fetch.SetAttributeValue("page", page);
                fetch.SetAttributeValue("paging-cookie", cookie);

                var response = service.RetrieveMultiple(
                    new FetchExpression(document.ToString(SaveOptions.DisableFormatting)));

                all.AddRange(response.Entities);

                if (!response.MoreRecords)
                    return all;

                cookie = response.PagingCookie;
                page++;
            }
        }
    }
}
