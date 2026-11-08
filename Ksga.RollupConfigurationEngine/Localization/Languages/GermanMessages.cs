namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class GermanMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Ungültige Rollup-Funktion: {0}",
            [MessageCodes.FunctionTypeRequired] = "Der Funktionstyp ist erforderlich.",
            [MessageCodes.FunctionTypeUnsupported] = "Der Funktionstyp {0} wird nicht unterstützt.",
            [MessageCodes.FunctionTargetRequired] = "Das Zielfeld ist erforderlich.",
            [MessageCodes.FunctionSourceRequired] = "Das Quellfeld ist erforderlich.",
            [MessageCodes.FunctionTargetInvalidName] = "Das Zielfeld '{0}' ist kein gültiger logischer Name (nur Kleinbuchstaben, Ziffern und Unterstrich).",
            [MessageCodes.FunctionSourceInvalidName] = "Das Quellfeld '{0}' ist kein gültiger logischer Name (nur Kleinbuchstaben, Ziffern und Unterstrich).",
            [MessageCodes.FunctionSeparatorRequired] = "Das Trennzeichen ist für die Funktion Concat erforderlich.",
            [MessageCodes.FunctionConfigurationRequired] = "Die Rollup-Konfiguration ist erforderlich.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "Die Konfiguration definiert weder ihre übergeordnete noch ihre untergeordnete Entität.",
            [MessageCodes.FunctionTargetNotFound] = "Das Zielfeld '{0}' ist in der übergeordneten Entität '{1}' nicht vorhanden.",
            [MessageCodes.FunctionSourceNotFound] = "Das Quellfeld '{0}' ist in der untergeordneten Entität '{1}' nicht vorhanden.",
            [MessageCodes.FunctionParentEntityUnreadable] = "Die übergeordnete Entität '{0}' kann nicht gelesen werden: {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "Die untergeordnete Entität '{0}' kann nicht gelesen werden: {1}",

            [MessageCodes.ValidationTargetIsRollup] = "Das Zielfeld '{0}' ist ein Rollup-Feld: Es kann nicht von der Funktion {1} beschrieben werden.",
            [MessageCodes.ValidationTargetIsCalculated] = "Das Zielfeld '{0}' ist ein berechnetes oder schreibgeschütztes Feld: Es kann nicht von der Funktion {1} beschrieben werden.",
            [MessageCodes.ValidationTargetNotRollup] = "Das Zielfeld '{0}' ist kein Rollup-Feld: Die Funktion Refresh gilt nur für ein Rollup-Feld.",
            [MessageCodes.ValidationFunctionUnsupported] = "Die Funktion {0} wird nicht unterstützt.",
            [MessageCodes.ValidationSourceNumericOrDate] = "Das Quellfeld '{0}' ({1}) muss für die Funktion {2} numerisch oder ein Datum sein.",
            [MessageCodes.ValidationSourceType] = "Das Quellfeld '{0}' ({1}) muss für die Funktion {3} vom Typ {2} sein.",
            [MessageCodes.ValidationTargetType] = "Das Zielfeld '{0}' ({1}) muss für die Funktion {3} vom Typ {2} sein.",
            [MessageCodes.ValidationAveragePrecision] = "Das Zielfeld '{0}' ({1}) kann keinen Durchschnitt ohne Genauigkeitsverlust aufnehmen: Verwenden Sie ein Dezimal-, Double- oder Währungsfeld.",
            [MessageCodes.ValidationNarrowing] = "Das Zielfeld '{0}' ({1}) ist weniger genau als das Quellfeld '{2}' ({3}): Die Funktion {4} würde an Genauigkeit verlieren.",

            [MessageCodes.TypeNumeric] = "numerisch",
            [MessageCodes.TypeBoolean] = "boolesch",
            [MessageCodes.TypeText] = "Text",
            [MessageCodes.TypeDate] = "Datum",

            [MessageCodes.RelationsInvalid] = "Ungültige Rollup-Beziehungen: {0}",
            [MessageCodes.RelationsNone] = "Es wurde keine Beziehung angegeben.",
            [MessageCodes.RelationNotFound] = "Die Beziehung '{0}' ist nicht vorhanden.",
            [MessageCodes.RelationNotOneToMany] = "Die Beziehung '{0}' ist vom Typ N:N: Es werden nur 1:N-Beziehungen akzeptiert.",
            [MessageCodes.RelationChainInvalid] = "Der Pfad ist ungültig: Die Beziehung '{0}' endet bei '{1}', aber '{2}' beginnt bei '{3}'. Die Beziehungen müssen vom Übergeordneten zum Untergeordneten verkettet sein.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Zusätzliche Auslöserfelder: ungültige Feldnamen: {0} (nur Kleinbuchstaben, Ziffern und Unterstrich).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Zusätzliche Auslöserfelder: Diese Felder sind in der untergeordneten Entität '{0}' nicht vorhanden: {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Zusätzliche Auslöserfelder: Die untergeordnete Entität '{0}' kann nicht gelesen werden: {1}",

            [MessageCodes.ChildFilterInvalid] = "Der Filter der untergeordneten Datensätze: {0}",
            [MessageCodes.ParentFilterInvalid] = "Der Filter des übergeordneten Datensatzes: {0}",
            [MessageCodes.FilterEmpty] = "der Filter ist leer.",
            [MessageCodes.FilterDtd] = "er darf keine DTD-Deklaration enthalten.",
            [MessageCodes.FilterXmlInvalid] = "es ist kein gültiges XML: {0}",
            [MessageCodes.FilterXmlNoElement] = "es ist kein gültiges XML: kein Element gefunden.",

            [MessageCodes.ConfigurationImmutableFields] = "Diese Felder können nach dem Erstellen der Konfiguration nicht mehr geändert werden: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Zu viele Auslöserfelder für die Änderung: Die Liste überschreitet {0} Zeichen.",

            [MessageCodes.StepPluginTypeNotRegistered] = "Der Plugin-Typ '{0}' ist in der Umgebung nicht registriert.",
            [MessageCodes.StepMessageNotFound] = "Die Nachricht {0} wurde nicht gefunden.",
            [MessageCodes.StepMessageNotSupported] = "Die Entität '{0}' unterstützt keinen {1}-Schritt.",

            [MessageCodes.ExchangeRateNotFound] = "Der Wechselkurs des übergeordneten Datensatzes wurde nicht gefunden: Die Währungsumrechnung des Feldes '{0}' ist nicht möglich.",
            [MessageCodes.LookupPathInvalid] = "Ungültiger Lookup-Pfad '{0}'.",
            [MessageCodes.LookupPathEntityMissing] = "Ungültiger Lookup-Pfad '{0}': Auf '{1}' müssen '@' und die Zielentität folgen."
        };
    }
}
