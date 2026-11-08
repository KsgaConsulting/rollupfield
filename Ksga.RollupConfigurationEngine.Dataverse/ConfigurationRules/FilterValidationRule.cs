using Ksga.RollupConfigurationEngine.Core.Localization;
using System;
using Microsoft.Xrm.Sdk;
using Ksga.RollupConfigurationEngine.Dataverse.Fetch;

namespace Ksga.RollupConfigurationEngine.Dataverse.ConfigurationRules
{
    public sealed class FilterValidationRule : IRollupConfigurationRule
    {
        public void Apply(RollupConfigurationRuleContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.IsCreate && !context.IsUpdate)
                return;

            Check(context, RollupConfigurationFields.ChildFilter, MessageCodes.ChildFilterInvalid);
            Check(context, RollupConfigurationFields.ParentExecutionFilter, MessageCodes.ParentFilterInvalid);
        }

        private static void Check(RollupConfigurationRuleContext context, string field, string code)
        {
            if (!context.Target.Contains(field))
                return;

            var value = context.Target.GetAttributeValue<string>(field);

            if (string.IsNullOrWhiteSpace(value))
                return;

            try
            {
                FetchFragment.Parse(value!.Trim());
            }
            catch (RollupException exception)
            {
                context.Trace(field + " rejected: " + exception.Message);

                throw RollupException.Create(code, exception.Localized);
            }
        }
    }
}
