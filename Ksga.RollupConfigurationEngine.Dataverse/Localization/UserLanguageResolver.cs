using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ksga.RollupConfigurationEngine.Core.Localization;

namespace Ksga.RollupConfigurationEngine.Dataverse.Localization
{
    public static class UserLanguageResolver
    {
        private const string UserSettingsEntity = "usersettings";
        private const string UiLanguageField = "uilanguageid";

        public static string Resolve(IOrganizationService service, Guid userId, Action<string>? trace = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            try
            {
                var settings = service.Retrieve(UserSettingsEntity, userId, new ColumnSet(UiLanguageField));

                return FromLcid(settings.GetAttributeValue<int?>(UiLanguageField));
            }
            catch (Exception exception)
            {
                trace?.Invoke("User language not read, using English: " + exception.Message);

                return MessageCatalog.DefaultLanguage;
            }
        }

        public static string FromLcid(int? lcid)
        {
            if (!lcid.HasValue || lcid.Value <= 0)
                return MessageCatalog.DefaultLanguage;

            try
            {
                return MessageCatalog.NormalizeLanguage(
                    CultureInfo.GetCultureInfo(lcid.Value).TwoLetterISOLanguageName);
            }
            catch (CultureNotFoundException)
            {
                return MessageCatalog.DefaultLanguage;
            }
        }
    }
}
