namespace Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules
{
    public interface IRollupConfigurationRule
    {
        void Apply(RollupConfigurationRuleContext context);
    }
}
