using Microsoft.Xrm.Sdk;

namespace Ksga.RollupConfigurationEngine.Dataverse.Relations
{
    public interface IRelationMetadataProvider
    {
        RelationInfo? GetRelation(IOrganizationService service, string schemaName);
    }
}
