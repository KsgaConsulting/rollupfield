namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class ItalianMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Funzione di rollup non valida: {0}",
            [MessageCodes.FunctionTypeRequired] = "Il tipo di funzione è obbligatorio.",
            [MessageCodes.FunctionTypeUnsupported] = "Il tipo di funzione {0} non è supportato.",
            [MessageCodes.FunctionTargetRequired] = "Il campo di destinazione è obbligatorio.",
            [MessageCodes.FunctionSourceRequired] = "Il campo di origine è obbligatorio.",
            [MessageCodes.FunctionTargetInvalidName] = "Il campo di destinazione '{0}' non è un nome logico valido (solo lettere minuscole, cifre e underscore).",
            [MessageCodes.FunctionSourceInvalidName] = "Il campo di origine '{0}' non è un nome logico valido (solo lettere minuscole, cifre e underscore).",
            [MessageCodes.FunctionSeparatorRequired] = "Il separatore è obbligatorio per la funzione Concat.",
            [MessageCodes.FunctionConfigurationRequired] = "La configurazione di rollup è obbligatoria.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "La configurazione non definisce la propria entità padre o la propria entità figlia.",
            [MessageCodes.FunctionTargetNotFound] = "Il campo di destinazione '{0}' non esiste nell'entità padre '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "Il campo di origine '{0}' non esiste nell'entità figlia '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "Impossibile leggere l'entità padre '{0}': {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "Impossibile leggere l'entità figlia '{0}': {1}",

            [MessageCodes.ValidationTargetIsRollup] = "Il campo di destinazione '{0}' è un campo di rollup: non può essere scritto dalla funzione {1}.",
            [MessageCodes.ValidationTargetIsCalculated] = "Il campo di destinazione '{0}' è un campo calcolato o di sola lettura: non può essere scritto dalla funzione {1}.",
            [MessageCodes.ValidationTargetNotRollup] = "Il campo di destinazione '{0}' non è un campo di rollup: la funzione Refresh si applica solo a un campo di rollup.",
            [MessageCodes.ValidationFunctionUnsupported] = "La funzione {0} non è supportata.",
            [MessageCodes.ValidationSourceNumericOrDate] = "Il campo di origine '{0}' ({1}) deve essere numerico o una data per la funzione {2}.",
            [MessageCodes.ValidationSourceType] = "Il campo di origine '{0}' ({1}) deve essere di tipo {2} per la funzione {3}.",
            [MessageCodes.ValidationTargetType] = "Il campo di destinazione '{0}' ({1}) deve essere di tipo {2} per la funzione {3}.",
            [MessageCodes.ValidationAveragePrecision] = "Il campo di destinazione '{0}' ({1}) non può ricevere una media senza perdita di precisione: usare un campo decimale, double o valuta.",
            [MessageCodes.ValidationNarrowing] = "Il campo di destinazione '{0}' ({1}) è meno preciso del campo di origine '{2}' ({3}): la funzione {4} perderebbe precisione.",

            [MessageCodes.TypeNumeric] = "numerico",
            [MessageCodes.TypeBoolean] = "booleano",
            [MessageCodes.TypeText] = "testo",
            [MessageCodes.TypeDate] = "data",

            [MessageCodes.RelationsInvalid] = "Relazioni di rollup non valide: {0}",
            [MessageCodes.RelationsNone] = "Nessuna relazione indicata.",
            [MessageCodes.RelationNotFound] = "La relazione '{0}' non esiste.",
            [MessageCodes.RelationNotOneToMany] = "La relazione '{0}' è di tipo N:N: sono accettate solo relazioni 1:N.",
            [MessageCodes.RelationChainInvalid] = "Il percorso non è valido: la relazione '{0}' termina in '{1}' ma '{2}' parte da '{3}'. Le relazioni devono essere concatenate dal padre al figlio.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Campi trigger aggiuntivi: nomi di campo non validi: {0} (solo lettere minuscole, cifre e underscore).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Campi trigger aggiuntivi: questi campi non esistono nell'entità figlia '{0}': {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Campi trigger aggiuntivi: impossibile leggere l'entità figlia '{0}': {1}",

            [MessageCodes.ChildFilterInvalid] = "Il filtro dei figli: {0}",
            [MessageCodes.ParentFilterInvalid] = "Il filtro del padre: {0}",
            [MessageCodes.FilterEmpty] = "il filtro è vuoto.",
            [MessageCodes.FilterDtd] = "non deve contenere una dichiarazione DTD.",
            [MessageCodes.FilterXmlInvalid] = "non è un XML valido: {0}",
            [MessageCodes.FilterXmlNoElement] = "non è un XML valido: nessun elemento trovato.",

            [MessageCodes.ConfigurationImmutableFields] = "Questi campi non possono essere modificati dopo la creazione della configurazione: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Troppi campi trigger per la modifica: l'elenco supera {0} caratteri.",

            [MessageCodes.StepPluginTypeNotRegistered] = "Il tipo di plugin '{0}' non è registrato nell'ambiente.",
            [MessageCodes.StepMessageNotFound] = "Il messaggio {0} non è stato trovato.",
            [MessageCodes.StepMessageNotSupported] = "L'entità '{0}' non supporta uno step {1}.",

            [MessageCodes.ExchangeRateNotFound] = "Il tasso di cambio del padre non è stato trovato: la conversione di valuta del campo '{0}' è impossibile.",
            [MessageCodes.LookupPathInvalid] = "Percorso di lookup '{0}' non valido.",
            [MessageCodes.LookupPathEntityMissing] = "Percorso di lookup '{0}' non valido: '{1}' deve essere seguito da '@' e dall'entità a cui punta."
        };
    }
}
