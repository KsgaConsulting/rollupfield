using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksga.RollupConfigurationEngine.Core.Crm
{
    public sealed class RollupRelationPath
    {
        public string ParentEntity { get; init; }
        public string ChildEntity { get; init; }
        public IReadOnlyList<string> LookupPath { get; init; }
    }

}
