using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public enum RollupFunctionType
    {
        Sum = 0,

        Average = 1,

        Minimum = 2,

        Maximum = 3,

        Count = 4,

        Any = 5,

        None = 6,

        And = 7,

        Or = 8,

        Concat = 9,

        Refresh = 10
    }
}

