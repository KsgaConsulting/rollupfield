using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Dataverse.Configuration;
using Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules;
using Ksga.RollupConfigurationEngine.Dataverse.Validation;

namespace Ksga.RollupConfigurationEngine.Dataverse.Steps
{
    public sealed class RollupStepSynchronizer
    {
        private const int PostOperation = 40;
        private const int Synchronous = 0;
        private const int Asynchronous = 1;
        private const string PreImageName = "PreImage";

        private readonly string _pluginTypeName;

        public RollupStepSynchronizer(string pluginTypeName)
        {
            if (string.IsNullOrWhiteSpace(pluginTypeName))
                throw new ArgumentException("Le nom du type de plugin est obligatoire.", nameof(pluginTypeName));

            _pluginTypeName = pluginTypeName;
        }

        public static string StepName(
            string childEntity,
            string message = "Create",
            RollupExecutionMode mode = RollupExecutionMode.Synchronous)
            => "Ksga.RollupConfigurationEngine: " + message + " of " + childEntity
                + (mode == RollupExecutionMode.Asynchronous ? " (async)" : string.Empty);

        public static string FirstLookup(string? lookupPath)
            => (lookupPath ?? string.Empty).Split('.')[0].Split('@')[0].Trim();

        public void Synchronize(
            IOrganizationService service,
            string childEntity,
            Action<string> trace,
            Guid? pendingFunctionConfigurationId = null,
            Guid? excludedFunctionId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrWhiteSpace(childEntity)) throw new ArgumentException("L'entité enfant est obligatoire.", nameof(childEntity));
            if (trace == null) throw new ArgumentNullException(nameof(trace));

            var allConfigurations = ActiveConfigurations(service, childEntity);

            foreach (var mode in new[] { RollupExecutionMode.Synchronous, RollupExecutionMode.Asynchronous })
            {
                var configurations = allConfigurations
                    .Where(c => DataverseRollupConfigurationRepository.ModeOf(c) == mode)
                    .ToList();

                var onCreate = configurations.Where(c => c.GetAttributeValue<bool>(RollupConfigurationFields.TriggerOnCreate)).ToList();
                var onDelete = configurations.Where(c => c.GetAttributeValue<bool>(RollupConfigurationFields.TriggerOnDelete)).ToList();

                SynchronizeStep(service, childEntity, "Create", mode, onCreate.Count > 0, null, null, trace);

                var deleteLookups = Lookups(onDelete);
                SynchronizeStep(service, childEntity, "Delete", mode, deleteLookups.Count > 0, null, deleteLookups, trace);

                var withFunctions = configurations
                    .Where(c => HasFunction(service, c.Id, pendingFunctionConfigurationId, excludedFunctionId))
                    .ToList();

                var updateLookups = Lookups(withFunctions);
                var updateFilter = TriggerOnUpdateFields.Join(
                    updateLookups.Concat(withFunctions.SelectMany(
                        c => TriggerOnUpdateFields.ParseList(c.GetAttributeValue<string>(RollupConfigurationFields.TriggerOnUpdate)))));

                SynchronizeStep(service, childEntity, "Update", mode, updateLookups.Count > 0, updateFilter, updateLookups, trace);
            }
        }

        private static bool HasFunction(
            IOrganizationService service, Guid configurationId, Guid? pendingConfigurationId, Guid? excludedFunctionId)
        {
            if (configurationId == pendingConfigurationId)
                return true;

            var query = new QueryExpression(RollupFunctionValidationService.FunctionEntity) { ColumnSet = new ColumnSet(false) };
            query.Criteria.AddCondition(
                RollupFunctionValidationService.ConfigurationField, ConditionOperator.Equal, configurationId);

            return service.RetrieveMultiple(query).Entities.Any(f => f.Id != excludedFunctionId);
        }

        private static List<string> Lookups(IEnumerable<Entity> configurations)
            => configurations
                .Select(c => FirstLookup(c.GetAttributeValue<string>(RollupConfigurationFields.LookupPath)))
                .Where(a => a.Length > 0)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(a => a, StringComparer.Ordinal)
                .ToList();

        private void SynchronizeStep(
            IOrganizationService service,
            string childEntity,
            string message,
            RollupExecutionMode mode,
            bool needed,
            string? filteringAttributes,
            List<string>? imageAttributes,
            Action<string> trace)
        {
            var stepName = StepName(childEntity, message, mode);
            var existing = FindSteps(service, stepName);

            if (!needed)
            {
                if (existing.Count == 0)
                    return;

                foreach (var step in existing)
                    service.Delete("sdkmessageprocessingstep", step.Id);

                trace(stepName + " removed");
                return;
            }

            if (existing.Count > 0
                && !string.Equals(
                    existing[0].GetAttributeValue<string>("filteringattributes") ?? string.Empty,
                    filteringAttributes ?? string.Empty,
                    StringComparison.Ordinal))
            {
                foreach (var step in existing)
                    service.Delete("sdkmessageprocessingstep", step.Id);

                existing.Clear();
            }

            if (existing.Count == 0)
            {
                var stepId = CreateStep(service, childEntity, message, mode, filteringAttributes);

                if (imageAttributes != null)
                    EnsureImage(service, stepId, imageAttributes);

                trace(stepName + " registered");
                return;
            }

            if (imageAttributes != null && EnsureImage(service, existing[0].Id, imageAttributes))
                trace(stepName + " image updated");
        }

        private static List<Entity> ActiveConfigurations(IOrganizationService service, string childEntity)
        {
            var query = new QueryExpression(RollupConfigurationFields.Entity)
            {
                ColumnSet = new ColumnSet(
                    RollupConfigurationFields.LookupPath,
                    RollupConfigurationFields.TriggerOnCreate,
                    RollupConfigurationFields.TriggerOnDelete,
                    RollupConfigurationFields.TriggerOnUpdate,
                    RollupConfigurationFields.ExecutionMode)
            };
            query.Criteria.AddCondition(RollupConfigurationFields.ChildEntity, ConditionOperator.Equal, childEntity);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return service.RetrieveMultiple(query).Entities.ToList();
        }

        private static List<Entity> FindSteps(IOrganizationService service, string stepName)
        {
            var query = new QueryExpression("sdkmessageprocessingstep") { ColumnSet = new ColumnSet("filteringattributes") };
            query.Criteria.AddCondition("name", ConditionOperator.Equal, stepName);

            return service.RetrieveMultiple(query).Entities.ToList();
        }

        private static bool EnsureImage(IOrganizationService service, Guid stepId, List<string> attributes)
        {
            var wanted = string.Join(",", attributes);

            var query = new QueryExpression("sdkmessageprocessingstepimage") { ColumnSet = new ColumnSet("attributes") };
            query.Criteria.AddCondition("sdkmessageprocessingstepid", ConditionOperator.Equal, stepId);

            var image = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (image != null)
            {
                if (string.Equals(image.GetAttributeValue<string>("attributes"), wanted, StringComparison.Ordinal))
                    return false;

                service.Delete("sdkmessageprocessingstepimage", image.Id);
            }

            service.Create(new Entity("sdkmessageprocessingstepimage")
            {
                ["name"] = PreImageName,
                ["entityalias"] = PreImageName,
                ["imagetype"] = new OptionSetValue(0),
                ["messagepropertyname"] = "Target",
                ["attributes"] = wanted,
                ["sdkmessageprocessingstepid"] = new EntityReference("sdkmessageprocessingstep", stepId)
            });

            return true;
        }

        private Guid CreateStep(
            IOrganizationService service,
            string childEntity,
            string message,
            RollupExecutionMode mode,
            string? filteringAttributes)
        {
            var pluginTypeId = FindId(service, "plugintype", "typename", _pluginTypeName)
                ?? throw RollupException.Create(MessageCodes.StepPluginTypeNotRegistered, _pluginTypeName);

            var messageId = FindId(service, "sdkmessage", "name", message)
                ?? throw RollupException.Create(MessageCodes.StepMessageNotFound, message);

            var filterQuery = new QueryExpression("sdkmessagefilter")
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1
            };
            filterQuery.Criteria.AddCondition("sdkmessageid", ConditionOperator.Equal, messageId);
            filterQuery.Criteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, childEntity);

            var filter = service.RetrieveMultiple(filterQuery).Entities.FirstOrDefault()
                ?? throw RollupException.Create(MessageCodes.StepMessageNotSupported, childEntity, message);

            var step = new Entity("sdkmessageprocessingstep")
            {
                ["name"] = StepName(childEntity, message, mode),
                ["description"] = StepName(childEntity, message, mode),
                ["plugintypeid"] = new EntityReference("plugintype", pluginTypeId),
                ["eventhandler"] = new EntityReference("plugintype", pluginTypeId),
                ["sdkmessageid"] = new EntityReference("sdkmessage", messageId),
                ["sdkmessagefilterid"] = new EntityReference("sdkmessagefilter", filter.Id),
                ["stage"] = new OptionSetValue(PostOperation),
                ["mode"] = new OptionSetValue(mode == RollupExecutionMode.Asynchronous ? Asynchronous : Synchronous),
                ["rank"] = 1,
                ["supporteddeployment"] = new OptionSetValue(0)
            };

            if (mode == RollupExecutionMode.Asynchronous)
                step["asyncautodelete"] = true;

            if (!string.IsNullOrEmpty(filteringAttributes))
                step["filteringattributes"] = filteringAttributes;

            return service.Create(step);
        }

        private static Guid? FindId(IOrganizationService service, string entity, string attribute, string value)
        {
            var query = new QueryExpression(entity) { ColumnSet = new ColumnSet(false), TopCount = 1 };
            query.Criteria.AddCondition(attribute, ConditionOperator.Equal, value);

            var found = service.RetrieveMultiple(query).Entities.FirstOrDefault();
            return found?.Id;
        }
    }
}
