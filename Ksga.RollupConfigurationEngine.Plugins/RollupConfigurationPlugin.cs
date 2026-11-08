using System;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules;

namespace Ksga.RollupConfigurationEngine.Plugins
{
    public sealed class RollupConfigurationPlugin : IPlugin
    {
        private static readonly IRollupConfigurationRule[] Rules =
        {
            new RelationPathRule(),
            new ImmutableFieldsRule(),
            new FilterValidationRule(),
            new AdditionalTriggerFieldsRule()
        };

        public void Execute(IServiceProvider serviceProvider)
            => PluginErrorHandler.Run(serviceProvider, ExecuteCore);

        private void ExecuteCore(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            var tracing = (ITracingService)
                serviceProvider.GetService(typeof(ITracingService));

            var serviceFactory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            if (!context.InputParameters.Contains("Target")
                || context.InputParameters["Target"] is not Entity target
                || target.LogicalName != RollupConfigurationFields.Entity)
            {
                return;
            }

            var preImage = context.PreEntityImages.Contains("PreImage")
                ? context.PreEntityImages["PreImage"]
                : null;

            var ruleContext = new RollupConfigurationRuleContext(
                context.MessageName,
                target,
                preImage,
                serviceFactory.CreateOrganizationService(context.UserId),
                message => tracing.Trace("{0}", message));

            foreach (var rule in Rules)
                rule.Apply(ruleContext);
        }
    }
}
