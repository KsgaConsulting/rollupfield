using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksga.RollupConfigurationEngine.Core.Configuration
{
    public class RollupExecutionConfiguration
    {
        public bool RunOnCreate { get; set; }

        public bool RunOnUpdate { get; set; }

        public bool RunOnDelete { get; set; }

        public IReadOnlyList<string> TriggerFields { get; set; } = new List<string>();

        public bool RunnableFromParent { get; set; }

        public RollupExecutionMode ExecutionMode { get; set; } = RollupExecutionMode.Synchronous;
    }

    public enum RollupExecutionMode
    {
        Synchronous = 0,
        Asynchronous = 1
    }
}

