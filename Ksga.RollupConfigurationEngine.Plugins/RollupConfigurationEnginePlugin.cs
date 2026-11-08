using System;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Dataverse.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.Execution;

namespace Ksga.RollupConfigurationEngine.Plugins
{
    public sealed class RollupConfigurationEnginePlugin : IPlugin
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

            var service = serviceFactory.CreateOrganizationService(null);

            tracing.Trace(
                $"RollupConfigurationEnginePlugin START — Message: {context.MessageName}");

            var target = ResolveTargetEntity(context, tracing);

            if (target == null)
            {
                tracing.Trace("RollupConfigurationEnginePlugin EXIT — no target resolved.");
                return;
            }

            Entity? preImage = null;
            if (context.MessageName == "Update" && context.PreEntityImages.Contains("PreImage"))
                preImage = context.PreEntityImages["PreImage"];

            var orchestrationService = new DataverseRollupOrchestrationService();

            orchestrationService.ExecuteForChildTrigger(
                service,
                target,
                context.MessageName,
                tracing,
                preImage,
                context.Mode == (int)RollupExecutionMode.Asynchronous
                    ? RollupExecutionMode.Asynchronous
                    : RollupExecutionMode.Synchronous);

            tracing.Trace("RollupConfigurationEnginePlugin END");
        }

        private static Entity? ResolveTargetEntity(
            IPluginExecutionContext context,
            ITracingService tracing)
        {
            if (context.MessageName == "Delete")
            {
                if (!context.PreEntityImages.Contains("PreImage"))
                {
                    tracing.Trace(
                        "Delete message: Pre-Image 'PreImage' is not registered. " +
                        "Cannot resolve parent lookups. " +
                        "Register a Pre-Image named 'PreImage' on the Delete step.");
                    return null;
                }

                var preImage = context.PreEntityImages["PreImage"];
                var targetRef = context.InputParameters["Target"] as EntityReference;

                if (targetRef != null)
                    preImage.Id = targetRef.Id;

                tracing.Trace(
                    $"Delete: using Pre-Image for {preImage.LogicalName} {preImage.Id}");

                return preImage;
            }

            if (!context.InputParameters.Contains("Target") ||
                context.InputParameters["Target"] is not Entity target)
            {
                tracing.Trace("Target missing or not an Entity.");
                return null;
            }

            return target;
        }
    }
}
