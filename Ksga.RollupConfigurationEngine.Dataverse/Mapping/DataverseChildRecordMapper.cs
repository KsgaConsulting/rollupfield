using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Ksga.RollupConfigurationEngine.Dataverse.Mapping
{
    public static class DataverseChildRecordMapper
    {
        public static IDictionary<string, object> Map(Entity entity)
        {
            var result = new Dictionary<string, object>();

            foreach (var attribute in entity.Attributes)
            {
                result[attribute.Key] = ExtractValue(attribute.Value);
            }

            return result;
        }

        private static object ExtractValue(object value)
        {
            return value switch
            {
                Money money => money.Value,
                OptionSetValue option => option.Value,
                EntityReference reference => reference.Id,
                _ => value
            };
        }
    }
}
