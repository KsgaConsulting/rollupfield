using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Core.Localization;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules;
using Ksga.RollupConfigurationEngine.Dataverse.Steps;
using Ksga.RollupConfigurationEngine.Dataverse.Validation;

namespace Ksga.RollupConfigurationEngine.Plugins
{
    public sealed class RollupFunctionValidationPlugin : IPlugin
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

            if (context.InputParameters.Contains("Target")
                && context.InputParameters["Target"] is EntityReference deleted
                && deleted.LogicalName == RollupFunctionValidationService.FunctionEntity)
            {
                RefreshAfterDelete(context, serviceFactory, tracing, deleted.Id);
                return;
            }

            if (!context.InputParameters.Contains("Target")
                || context.InputParameters["Target"] is not Entity target
                || target.LogicalName != RollupFunctionValidationService.FunctionEntity)
            {
                return;
            }

            var preImage = context.PreEntityImages.Contains("PreImage")
                ? context.PreEntityImages["PreImage"]
                : null;

            var service = serviceFactory.CreateOrganizationService(context.UserId);

            var result = new RollupFunctionValidationService().Validate(
                service,
                RollupFunctionValidationService.Merge(target, preImage));

            if (!result.IsValid)
            {
                tracing.Trace("Rollup function rejected: " + string.Join(" | ", result.Errors));

                throw RollupException.Create(MessageCodes.FunctionInvalid, result.Errors);
            }

            target[RollupFunctionValidationService.SourceTypeField] = result.SourceType;
            target[RollupFunctionValidationService.TargetTypeField] = result.TargetType;

            var requested = RollupFunctionValidationService.Merge(target, preImage)
                .GetAttributeValue<bool?>(RollupFunctionValidationService.ConvertCurrencyField) == true;

            if (requested && !result.ConvertCurrency)
                tracing.Trace("Currency conversion reset to false: it only applies to Money fields with Sum, Average, Minimum or Maximum.");

            target[RollupFunctionValidationService.ConvertCurrencyField] = result.ConvertCurrency;

            RollupFunctionValidationService.ClearUnusedFields(target, RollupFunctionValidationService.Merge(target, preImage));

            var merged = RollupFunctionValidationService.Merge(target, preImage);

            var configurationRef = merged.GetAttributeValue<EntityReference>(RollupFunctionValidationService.ConfigurationField);

            if (configurationRef != null)
            {
                var system = serviceFactory.CreateOrganizationService(null);

                TriggerOnUpdateFields.Refresh(system, configurationRef.Id, merged);

                SynchronizeSteps(system, tracing, configurationRef.Id, configurationRef.Id, null);
            }
        }

        private static void SynchronizeSteps(
            IOrganizationService system,
            ITracingService tracing,
            Guid configurationId,
            Guid? pendingFunctionConfigurationId,
            Guid? excludedFunctionId)
        {
            var childEntity = system
                .Retrieve(
                    RollupConfigurationFields.Entity,
                    configurationId,
                    new ColumnSet(RollupConfigurationFields.ChildEntity))
                .GetAttributeValue<string>(RollupConfigurationFields.ChildEntity);

            if (string.IsNullOrWhiteSpace(childEntity))
                return;

            new RollupStepSynchronizer(typeof(RollupConfigurationEnginePlugin).FullName).Synchronize(
                system,
                childEntity!,
                message => tracing.Trace("{0}", message),
                pendingFunctionConfigurationId,
                excludedFunctionId);
        }

        private static void RefreshAfterDelete(
            IPluginExecutionContext context,
            IOrganizationServiceFactory serviceFactory,
            ITracingService tracing,
            Guid functionId)
        {
            if (!context.PreEntityImages.Contains("PreImage"))
                return;

            var configurationRef = context.PreEntityImages["PreImage"]
                .GetAttributeValue<EntityReference>(RollupFunctionValidationService.ConfigurationField);

            if (configurationRef == null)
                return;

            try
            {
                var system = serviceFactory.CreateOrganizationService(null);

                TriggerOnUpdateFields.Refresh(system, configurationRef.Id, null, functionId);

                SynchronizeSteps(system, tracing, configurationRef.Id, null, functionId);
            }
            catch (FaultException<OrganizationServiceFault> fault)
                when (fault.Detail.ErrorCode == ObjectDoesNotExistErrorCode)
            {
            }
        }

        private const int ObjectDoesNotExistErrorCode = -2147220969;
    }
}
