using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Core.Configuration;

namespace Ksga.RollupConfigurationEngine.Dataverse.Mapping
{
    public static class RollupConfigurationMapper
    {
        public static RollupConfiguration Map(
            Entity configurationEntity,
            IEnumerable<Entity> functionEntities)
        {
            if (configurationEntity == null)
                throw new ArgumentNullException(nameof(configurationEntity));

            if (functionEntities == null)
                throw new ArgumentNullException(nameof(functionEntities));

            var functions = functionEntities.Select(MapFunction).ToList();

            return new RollupConfiguration(
                parentEntityLogicalName:
                    configurationEntity.GetAttributeValue<string>("ksga_parententity"),

                childEntityLogicalName:
                    configurationEntity.GetAttributeValue<string>("ksga_childentity"),

                parentLookupAttribute:
                    configurationEntity.GetAttributeValue<string>("ksga_lookuppath"),

                functions: functions,

                parentExecutionFilterXml:
                    configurationEntity.GetAttributeValue<string>("ksga_parentexecutionfilter"),

                childFilterXml:
                    configurationEntity.GetAttributeValue<string>("ksga_childfilter"),

                triggerOnUpdate:
                    configurationEntity.GetAttributeValue<string>("ksga_triggeronupdate")
            );
        }

        public static RollupFunctionConfiguration MapFunction(Entity functionEntity)
        {
            var functionType = (RollupFunctionType)
                functionEntity.GetAttributeValue<OptionSetValue>("ksga_functiontypecode").Value;

            var sourceType = functionEntity.GetAttributeValue<string>("ksga_sourcechildfieldtype");
            var targetType = functionEntity.GetAttributeValue<string>("ksga_targetparentfieldtype");

            var convertCurrency =
                functionEntity.GetAttributeValue<bool?>("ksga_convertcurrency") == true
                && RollupCurrencyConversion.IsApplicable(functionType, sourceType, targetType);

            return new RollupFunctionConfiguration(
                targetParentField:
                    functionEntity.GetAttributeValue<string>("ksga_targetparentfield"),

                sourceChildField:
                    functionEntity.GetAttributeValue<string>("ksga_sourcechildfield"),

                functionType: functionType,

                separator:
                    functionEntity.GetAttributeValue<string>("ksga_separator"),

                convertCurrency: convertCurrency,

                sourceFieldType: sourceType,

                targetFieldType: targetType
            );
        }
    }
}
