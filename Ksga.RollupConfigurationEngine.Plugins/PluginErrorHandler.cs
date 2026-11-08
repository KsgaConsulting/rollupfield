using System;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Core.Localization;
using Ksga.RollupConfigurationEngine.Dataverse.Localization;

namespace Ksga.RollupConfigurationEngine.Plugins
{
    internal static class PluginErrorHandler
    {
        internal static void Run(IServiceProvider serviceProvider, Action<IServiceProvider> execute)
        {
            try
            {
                execute(serviceProvider);
            }
            catch (RollupException exception)
            {
                throw Translate(serviceProvider, exception);
            }
        }

        private static InvalidPluginExecutionException Translate(IServiceProvider serviceProvider, RollupException exception)
        {
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            var tracing = (ITracingService)
                serviceProvider.GetService(typeof(ITracingService));

            var serviceFactory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            var language = UserLanguageResolver.Resolve(
                serviceFactory.CreateOrganizationService(null),
                context.InitiatingUserId,
                message => tracing.Trace("{0}", message));

            tracing.Trace("{0}", exception.Code + " (" + language + "): " + exception.Message);

            return new InvalidPluginExecutionException(exception.GetMessage(language), exception);
        }
    }
}
