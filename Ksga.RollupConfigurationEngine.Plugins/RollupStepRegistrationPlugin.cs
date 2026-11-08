using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules;
using Ksga.RollupConfigurationEngine.Dataverse.Steps;

namespace Ksga.RollupConfigurationEngine.Plugins
{
    public sealed class RollupStepRegistrationPlugin : IPlugin
    {
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

            if (!context.InputParameters.Contains("Target"))
                return;

            var service = serviceFactory.CreateOrganizationService(null);

            string? childEntity;

            if (context.InputParameters["Target"] is EntityReference reference)
            {
                if (reference.LogicalName != RollupConfigurationFields.Entity
                    || !context.PreEntityImages.Contains("PreImage"))
                {
                    return;
                }

                childEntity = context.PreEntityImages["PreImage"]
                    .GetAttributeValue<string>(RollupConfigurationFields.ChildEntity);
            }
            else if (context.InputParameters["Target"] is Entity target
                && target.LogicalName == RollupConfigurationFields.Entity)
            {
                childEntity = service.Retrieve(
                    RollupConfigurationFields.Entity,
                    target.Id,
                    new ColumnSet(RollupConfigurationFields.ChildEntity))
                    .GetAttributeValue<string>(RollupConfigurationFields.ChildEntity);
            }
            else
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(childEntity))
                return;

            new RollupStepSynchronizer(typeof(RollupConfigurationEnginePlugin).FullName)
                .Synchronize(service, childEntity!, message => tracing.Trace("{0}", message));
        }
    }
}
