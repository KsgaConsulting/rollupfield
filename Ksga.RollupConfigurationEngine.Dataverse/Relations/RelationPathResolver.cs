using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Ksga.RollupConfigurationEngine.Dataverse.Relations
{
    public sealed class RelationPathResolver
    {
        private static readonly char[] Separators = { ',', ';' };

        private readonly IRelationMetadataProvider _relations;

        public RelationPathResolver()
            : this(new DataverseRelationMetadataProvider())
        {
        }

        public RelationPathResolver(IRelationMetadataProvider relations)
        {
            _relations = relations ?? throw new ArgumentNullException(nameof(relations));
        }

        public static IReadOnlyList<string> Parse(string? relations)
        {
            return (relations ?? string.Empty)
                .Split(Separators)
                .Select(r => r.Trim())
                .Where(r => r.Length > 0)
                .ToList();
        }

        public RelationPathResult Resolve(IOrganizationService service, string? relations)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var names = Parse(relations);

            if (names.Count == 0)
                return new RelationPathResult(new[] { new LocalizedMessage(MessageCodes.RelationsNone) });

            var errors = new List<LocalizedMessage>();
            var resolved = new List<RelationInfo>();

            foreach (var name in names)
            {
                var relation = _relations.GetRelation(service, name);

                if (relation == null)
                    errors.Add(new LocalizedMessage(MessageCodes.RelationNotFound, name));
                else if (!relation.IsOneToMany)
                    errors.Add(new LocalizedMessage(MessageCodes.RelationNotOneToMany, name));
                else
                    resolved.Add(relation);
            }

            if (errors.Count > 0)
                return new RelationPathResult(errors);

            for (var i = 1; i < resolved.Count; i++)
            {
                var previous = resolved[i - 1];
                var current = resolved[i];

                if (!string.Equals(previous.ReferencingEntity, current.ReferencedEntity, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(new LocalizedMessage(
                        MessageCodes.RelationChainInvalid,
                        previous.Name,
                        previous.ReferencingEntity,
                        current.Name,
                        current.ReferencedEntity));
                }
            }

            if (errors.Count > 0)
                return new RelationPathResult(errors);

            var path = string.Join(".", Enumerable.Reverse(resolved).Select((r, i) =>
                i == resolved.Count - 1 ? r.ReferencingAttribute : r.ReferencingAttribute + "@" + r.ReferencedEntity));

            return new RelationPathResult(
                errors,
                resolved[0].ReferencedEntity,
                resolved[resolved.Count - 1].ReferencingEntity,
                path);
        }
    }
}
