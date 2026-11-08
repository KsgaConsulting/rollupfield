using Ksga.RollupConfigurationEngine.Core.Localization;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Dataverse.Relations
{
    public sealed class RelationPathResult
    {
        public IReadOnlyList<LocalizedMessage> Errors { get; }
        public string? ParentEntity { get; }
        public string? ChildEntity { get; }
        public string? LookupPath { get; }

        public bool IsValid => Errors.Count == 0;

        public RelationPathResult(
            IReadOnlyList<LocalizedMessage> errors,
            string? parentEntity = null,
            string? childEntity = null,
            string? lookupPath = null)
        {
            Errors = errors;
            ParentEntity = parentEntity;
            ChildEntity = childEntity;
            LookupPath = lookupPath;
        }
    }
}
