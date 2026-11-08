namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class DutchMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Ongeldige rollupfunctie: {0}",
            [MessageCodes.FunctionTypeRequired] = "Het functietype is verplicht.",
            [MessageCodes.FunctionTypeUnsupported] = "Het functietype {0} wordt niet ondersteund.",
            [MessageCodes.FunctionTargetRequired] = "Het doelveld is verplicht.",
            [MessageCodes.FunctionSourceRequired] = "Het bronveld is verplicht.",
            [MessageCodes.FunctionTargetInvalidName] = "Het doelveld '{0}' is geen geldige logische naam (alleen kleine letters, cijfers en underscore).",
            [MessageCodes.FunctionSourceInvalidName] = "Het bronveld '{0}' is geen geldige logische naam (alleen kleine letters, cijfers en underscore).",
            [MessageCodes.FunctionSeparatorRequired] = "Het scheidingsteken is verplicht voor de functie Concat.",
            [MessageCodes.FunctionConfigurationRequired] = "De rollupconfiguratie is verplicht.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "De configuratie definieert haar bovenliggende of onderliggende entiteit niet.",
            [MessageCodes.FunctionTargetNotFound] = "Het doelveld '{0}' bestaat niet in de bovenliggende entiteit '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "Het bronveld '{0}' bestaat niet in de onderliggende entiteit '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "De bovenliggende entiteit '{0}' kan niet worden gelezen: {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "De onderliggende entiteit '{0}' kan niet worden gelezen: {1}",

            [MessageCodes.ValidationTargetIsRollup] = "Het doelveld '{0}' is een rollupveld: het kan niet worden beschreven door de functie {1}.",
            [MessageCodes.ValidationTargetIsCalculated] = "Het doelveld '{0}' is een berekend of alleen-lezen veld: het kan niet worden beschreven door de functie {1}.",
            [MessageCodes.ValidationTargetNotRollup] = "Het doelveld '{0}' is geen rollupveld: de functie Refresh geldt alleen voor een rollupveld.",
            [MessageCodes.ValidationFunctionUnsupported] = "De functie {0} wordt niet ondersteund.",
            [MessageCodes.ValidationSourceNumericOrDate] = "Het bronveld '{0}' ({1}) moet numeriek zijn of een datum voor de functie {2}.",
            [MessageCodes.ValidationSourceType] = "Het bronveld '{0}' ({1}) moet van het type {2} zijn voor de functie {3}.",
            [MessageCodes.ValidationTargetType] = "Het doelveld '{0}' ({1}) moet van het type {2} zijn voor de functie {3}.",
            [MessageCodes.ValidationAveragePrecision] = "Het doelveld '{0}' ({1}) kan geen gemiddelde ontvangen zonder precisieverlies: gebruik een decimaal, double of valutaveld.",
            [MessageCodes.ValidationNarrowing] = "Het doelveld '{0}' ({1}) is minder nauwkeurig dan het bronveld '{2}' ({3}): de functie {4} zou precisie verliezen.",

            [MessageCodes.TypeNumeric] = "numeriek",
            [MessageCodes.TypeBoolean] = "booleaans",
            [MessageCodes.TypeText] = "tekst",
            [MessageCodes.TypeDate] = "datum",

            [MessageCodes.RelationsInvalid] = "Ongeldige rolluprelaties: {0}",
            [MessageCodes.RelationsNone] = "Er is geen relatie opgegeven.",
            [MessageCodes.RelationNotFound] = "De relatie '{0}' bestaat niet.",
            [MessageCodes.RelationNotOneToMany] = "De relatie '{0}' is van het type N:N: alleen 1:N-relaties worden geaccepteerd.",
            [MessageCodes.RelationChainInvalid] = "Het pad is ongeldig: de relatie '{0}' eindigt bij '{1}', maar '{2}' begint bij '{3}'. De relaties moeten van boven- naar onderliggend aan elkaar gekoppeld zijn.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Extra triggervelden: ongeldige veldnamen: {0} (alleen kleine letters, cijfers en underscore).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Extra triggervelden: deze velden bestaan niet in de onderliggende entiteit '{0}': {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Extra triggervelden: de onderliggende entiteit '{0}' kan niet worden gelezen: {1}",

            [MessageCodes.ChildFilterInvalid] = "Het filter voor onderliggende records: {0}",
            [MessageCodes.ParentFilterInvalid] = "Het filter voor het bovenliggende record: {0}",
            [MessageCodes.FilterEmpty] = "het filter is leeg.",
            [MessageCodes.FilterDtd] = "het mag geen DTD-declaratie bevatten.",
            [MessageCodes.FilterXmlInvalid] = "het is geen geldige XML: {0}",
            [MessageCodes.FilterXmlNoElement] = "het is geen geldige XML: geen element gevonden.",

            [MessageCodes.ConfigurationImmutableFields] = "Deze velden kunnen niet meer worden gewijzigd nadat de configuratie is aangemaakt: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Te veel triggervelden voor de wijziging: de lijst is langer dan {0} tekens.",

            [MessageCodes.StepPluginTypeNotRegistered] = "Het plugintype '{0}' is niet geregistreerd in de omgeving.",
            [MessageCodes.StepMessageNotFound] = "Het bericht {0} is niet gevonden.",
            [MessageCodes.StepMessageNotSupported] = "De entiteit '{0}' ondersteunt geen {1}-stap.",

            [MessageCodes.ExchangeRateNotFound] = "De wisselkoers van het bovenliggende record is niet gevonden: de valutaconversie van het veld '{0}' is onmogelijk.",
            [MessageCodes.LookupPathInvalid] = "Ongeldig lookuppad '{0}'.",
            [MessageCodes.LookupPathEntityMissing] = "Ongeldig lookuppad '{0}': '{1}' moet worden gevolgd door '@' en de entiteit waarnaar het verwijst."
        };
    }
}
