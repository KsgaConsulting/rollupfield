using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Core.Crm
{
    public sealed class CrmRelationPathBuilder
    {
        public RollupRelationPath Build(
            IOrganizationService service,
            IReadOnlyList<string> relationshipNames)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (relationshipNames == null || relationshipNames.Count == 0)
                throw new ArgumentException("At least one relationship is required.");

            var lookupPath = new List<string>();

            string parentEntity = null;
            string childEntity = null;

            foreach (var relationshipName in relationshipNames)
            {
                var request = new RetrieveRelationshipRequest
                {
                    Name = relationshipName,
                    RetrieveAsIfPublished = true
                };

                RetrieveRelationshipResponse response;

                try
                {
                    response = (RetrieveRelationshipResponse)service.Execute(request);
                }
                catch
                {
                    throw new InvalidOperationException(
                        $"Relationship '{relationshipName}' not found.");
                }

                if (response.RelationshipMetadata is not OneToManyRelationshipMetadata rel)
                {
                    throw new InvalidOperationException(
                        $"Relationship '{relationshipName}' is not One-To-Many.");
                }

                if (parentEntity == null)
                    parentEntity = rel.ReferencedEntity;

                childEntity = rel.ReferencingEntity;

                lookupPath.Add(rel.ReferencingAttribute);
            }

            return new RollupRelationPath
            {
                ParentEntity = parentEntity,
                ChildEntity = childEntity,
                LookupPath = lookupPath
            };
        }
    }
}
