using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Ksga.RollupConfigurationEngine.Core.Configuration;

namespace Ksga.RollupConfigurationEngine.Dataverse.Metadata
{
    public sealed class DataverseFieldMetadataProvider : IFieldMetadataProvider
    {
        private const int CalculatedSourceType = 1;
        private const int RollupSourceType = 2;
        private const int FormulaSourceType = 3;

        public FieldTypeInfo? GetField(
            IOrganizationService service,
            string entityLogicalName,
            string fieldLogicalName)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var response = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            });

            var attribute = response.EntityMetadata.Attributes?.FirstOrDefault(a =>
                string.Equals(a.LogicalName, fieldLogicalName, StringComparison.OrdinalIgnoreCase));

            return attribute == null ? null : Map(attribute);
        }

        public static FieldTypeInfo Map(AttributeMetadata attribute)
        {
            var isRollup = attribute.SourceType == RollupSourceType;

            var isReadOnly = !isRollup
                && (attribute.SourceType == CalculatedSourceType
                    || attribute.SourceType == FormulaSourceType
                    || attribute.IsValidForUpdate == false);

            return new FieldTypeInfo(
                attribute.AttributeType?.ToString() ?? "Unknown",
                isRollup: isRollup,
                isCalculated: isReadOnly);
        }
    }
}
