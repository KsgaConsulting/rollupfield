using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Ksga.RollupConfigurationEngine.Dataverse.Relations
{
    public sealed class DataverseRelationMetadataProvider : IRelationMetadataProvider
    {
        public RelationInfo? GetRelation(IOrganizationService service, string schemaName)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            RetrieveRelationshipResponse response;

            try
            {
                response = (RetrieveRelationshipResponse)service.Execute(new RetrieveRelationshipRequest
                {
                    Name = schemaName,
                    RetrieveAsIfPublished = true
                });
            }
            catch (FaultException<OrganizationServiceFault>)
            {
                return null;
            }

            if (response.RelationshipMetadata is OneToManyRelationshipMetadata oneToMany)
            {
                return new RelationInfo(
                    schemaName,
                    true,
                    oneToMany.ReferencedEntity,
                    oneToMany.ReferencingEntity,
                    oneToMany.ReferencingAttribute);
            }

            return new RelationInfo(schemaName, false);
        }
    }
}
