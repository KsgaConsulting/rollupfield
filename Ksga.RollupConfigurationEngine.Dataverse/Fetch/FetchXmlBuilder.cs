using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Ksga.RollupConfigurationEngine.Dataverse.Paths;

namespace Ksga.RollupConfigurationEngine.Dataverse.Fetch
{
    public sealed class FetchXmlBuilder
    {
        private readonly string _childEntity;
        private readonly IReadOnlyList<LookupHop> _lookupPath;
        private readonly IReadOnlyList<string> _attributesToSelect;
        private readonly string? _childFilterXml;

        public FetchXmlBuilder(
            string childEntity,
            IReadOnlyList<LookupHop> lookupPath,
            IReadOnlyList<string> attributesToSelect,
            string? childFilterXml = null)
        {
            _childEntity = childEntity ?? throw new ArgumentNullException(nameof(childEntity));
            _lookupPath = lookupPath ?? throw new ArgumentNullException(nameof(lookupPath));

            if (_lookupPath.Count == 0)
                throw new ArgumentException("Lookup path must contain at least one hop.", nameof(lookupPath));

            _attributesToSelect = attributesToSelect ?? Array.Empty<string>();
            _childFilterXml = childFilterXml;
        }

        public string Build(Guid parentId)
        {
            var fetch = new XElement("fetch",
                new XAttribute("mapping", "logical"),
                new XAttribute("distinct", "false")
            );

            var entity = new XElement("entity",
                new XAttribute("name", _childEntity)
            );

            entity.Add(new XElement("attribute",
                new XAttribute("name", $"{_childEntity}id")
            ));

            foreach (var attribute in _attributesToSelect)
            {
                entity.Add(new XElement("attribute",
                    new XAttribute("name", attribute)
                ));
            }

            if (!string.IsNullOrWhiteSpace(_childFilterXml))
            {
                entity.Add(FetchFragment.Parse(_childFilterXml!));
            }

            if (_lookupPath.Count == 1)
            {
                entity.Add(BuildFilterCondition(_lookupPath[0].AttributeName, parentId));
            }
            else
            {
                XElement current = entity;

                for (int i = 0; i < _lookupPath.Count - 1; i++)
                {
                    var hop = _lookupPath[i];

                    var link = new XElement("link-entity",
                        new XAttribute("name", hop.EntityLogicalName),
                        new XAttribute("from", $"{hop.EntityLogicalName}id"),
                        new XAttribute("to", hop.AttributeName),
                        new XAttribute("link-type", "inner")
                    );

                    current.Add(link);
                    current = link;
                }

                var lastHop = _lookupPath[_lookupPath.Count - 1];
                current.Add(BuildFilterCondition(lastHop.AttributeName, parentId));
            }

            fetch.Add(entity);
            return fetch.ToString(SaveOptions.DisableFormatting);
        }

        private static XElement BuildFilterCondition(string attribute, Guid parentId)
        {
            return new XElement("filter",
                new XElement("condition",
                    new XAttribute("attribute", attribute),
                    new XAttribute("operator", "eq"),
                    new XAttribute("value", parentId.ToString("D"))
                )
            );
        }
    }
}
