using System;
using Microsoft.Xrm.Sdk;

namespace Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules
{
    public sealed class RollupConfigurationRuleContext
    {
        public string MessageName { get; }
        public Entity Target { get; }
        public Entity? PreImage { get; }
        public IOrganizationService Service { get; }
        public Action<string> Trace { get; }

        public bool IsCreate => string.Equals(MessageName, "Create", StringComparison.OrdinalIgnoreCase);
        public bool IsUpdate => string.Equals(MessageName, "Update", StringComparison.OrdinalIgnoreCase);

        public RollupConfigurationRuleContext(
            string messageName,
            Entity target,
            Entity? preImage,
            IOrganizationService service,
            Action<string> trace)
        {
            MessageName = messageName ?? throw new ArgumentNullException(nameof(messageName));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            PreImage = preImage;
            Service = service ?? throw new ArgumentNullException(nameof(service));
            Trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }
    }
}
