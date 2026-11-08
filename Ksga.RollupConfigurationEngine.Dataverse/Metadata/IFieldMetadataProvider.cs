using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Core.Configuration;

namespace Ksga.RollupConfigurationEngine.Dataverse.Metadata
{
    public interface IFieldMetadataProvider
    {
        FieldTypeInfo? GetField(IOrganizationService service, string entityLogicalName, string fieldLogicalName);
    }
}
