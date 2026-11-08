using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Ksga.RollupConfigurationEngine.Dataverse.Fetch
{
    public static class FetchFragment
    {
        public static IReadOnlyList<XElement> Parse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw RollupException.Create(MessageCodes.FilterEmpty);

            if (xml.IndexOf("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) >= 0
                || xml.IndexOf("<!ENTITY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw RollupException.Create(MessageCodes.FilterDtd);
            }

            XElement wrapper;

            try
            {
                wrapper = XElement.Parse("<fragment>" + xml + "</fragment>");
            }
            catch (XmlException exception)
            {
                throw RollupException.Create(MessageCodes.FilterXmlInvalid, exception.Message);
            }

            var elements = wrapper.Elements().ToList();

            if (elements.Count == 0)
                throw RollupException.Create(MessageCodes.FilterXmlNoElement);

            return elements;
        }
    }
}
