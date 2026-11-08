using Ksga.RollupConfigurationEngine.Core.Configuration;

namespace Ksga.RollupConfigurationEngine.Core.Engine
{
    public interface IRollupEngine
    {
        RollupCalculationResult Execute(
            RollupConfiguration configuration,
            RollupCalculationContext context);
    }
}
