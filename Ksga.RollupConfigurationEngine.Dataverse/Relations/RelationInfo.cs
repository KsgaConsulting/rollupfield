namespace Ksga.RollupConfigurationEngine.Dataverse.Relations
{
    public sealed class RelationInfo
    {
        public string Name { get; }
        public bool IsOneToMany { get; }
        public string ReferencedEntity { get; }
        public string ReferencingEntity { get; }
        public string ReferencingAttribute { get; }

        public RelationInfo(
            string name,
            bool isOneToMany,
            string referencedEntity = "",
            string referencingEntity = "",
            string referencingAttribute = "")
        {
            Name = name;
            IsOneToMany = isOneToMany;
            ReferencedEntity = referencedEntity;
            ReferencingEntity = referencingEntity;
            ReferencingAttribute = referencingAttribute;
        }
    }
}
