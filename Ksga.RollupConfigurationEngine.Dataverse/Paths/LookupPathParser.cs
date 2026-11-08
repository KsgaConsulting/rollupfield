using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using System.Collections.Generic;

namespace Ksga.RollupConfigurationEngine.Dataverse.Paths
{
    public static class LookupPathParser
    {
        public static IReadOnlyList<LookupHop> Parse(string path, string parentEntity)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrWhiteSpace(parentEntity))
                throw new ArgumentNullException(nameof(parentEntity));

            var segments = path.Split('.');
            var hops = new List<LookupHop>();

            for (var i = 0; i < segments.Length; i++)
            {
                var parts = segments[i].Split('@');
                var attribute = parts[0].Trim();
                var isLast = i == segments.Length - 1;

                if (attribute.Length == 0)
                    throw RollupException.Create(MessageCodes.LookupPathInvalid, path);

                string entity;

                if (isLast)
                    entity = parentEntity;
                else if (parts.Length == 2 && parts[1].Trim().Length > 0)
                    entity = parts[1].Trim();
                else
                    throw RollupException.Create(MessageCodes.LookupPathEntityMissing, path, attribute);

                hops.Add(new LookupHop(entity, attribute));
            }

            return hops;
        }
    }
}
