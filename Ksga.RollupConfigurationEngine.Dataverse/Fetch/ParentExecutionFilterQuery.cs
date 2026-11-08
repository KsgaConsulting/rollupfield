using System;
using System.Xml.Linq;

namespace Ksga.RollupConfigurationEngine.Dataverse.Fetch
{
    public static class ParentExecutionFilterQuery
    {
        public static string Build(string parentEntity, Guid parentId, string filterXml)
        {
            if (string.IsNullOrWhiteSpace(parentEntity)) throw new ArgumentNullException(nameof(parentEntity));
            if (string.IsNullOrWhiteSpace(filterXml)) throw new ArgumentNullException(nameof(filterXml));

            var primaryKey = parentEntity + "id";

            var entity = new XElement("entity",
                new XAttribute("name", parentEntity),
                new XElement("attribute", new XAttribute("name", primaryKey)),
                FetchFragment.Parse(filterXml),
                new XElement("filter",
                    new XElement("condition",
                        new XAttribute("attribute", primaryKey),
                        new XAttribute("operator", "eq"),
                        new XAttribute("value", parentId.ToString("D")))));

            var fetch = new XElement("fetch",
                new XAttribute("top", 1),
                new XAttribute("mapping", "logical"),
                entity);

            return fetch.ToString(SaveOptions.DisableFormatting);
        }
    }
}
