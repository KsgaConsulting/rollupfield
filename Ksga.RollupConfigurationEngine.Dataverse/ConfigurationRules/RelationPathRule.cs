using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Dataverse.Relations;

namespace Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules
{
    public sealed class RelationPathRule : IRollupConfigurationRule
    {
        private const int MaxNameLength = 850;

        private readonly RelationPathResolver _resolver;

        public RelationPathRule()
            : this(new RelationPathResolver())
        {
        }

        public RelationPathRule(RelationPathResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public void Apply(RollupConfigurationRuleContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.IsCreate)
                return;

            var relations = context.Target.GetAttributeValue<string>(RollupConfigurationFields.Relations);

            if (string.IsNullOrWhiteSpace(relations))
                return;

            var result = _resolver.Resolve(context.Service, relations);

            if (!result.IsValid)
            {
                context.Trace("Rollup relations rejected: " + string.Join(" | ", result.Errors));

                throw RollupException.Create(MessageCodes.RelationsInvalid, result.Errors);
            }

            context.Target[RollupConfigurationFields.ParentEntity] = result.ParentEntity;
            context.Target[RollupConfigurationFields.ChildEntity] = result.ChildEntity;
            context.Target[RollupConfigurationFields.LookupPath] = result.LookupPath;

            if (string.IsNullOrWhiteSpace(context.Target.GetAttributeValue<string>(RollupConfigurationFields.Name)))
            {
                var name = string.Join(",", RelationPathResolver.Parse(relations));

                context.Target[RollupConfigurationFields.Name] =
                    name.Length > MaxNameLength ? name.Substring(0, MaxNameLength) : name;
            }
        }
    }
}
