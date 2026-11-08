namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class FrenchMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Fonction de rollup invalide : {0}",
            [MessageCodes.FunctionTypeRequired] = "Le type de fonction est obligatoire.",
            [MessageCodes.FunctionTypeUnsupported] = "Le type de fonction {0} n'est pas pris en charge.",
            [MessageCodes.FunctionTargetRequired] = "Le champ cible est obligatoire.",
            [MessageCodes.FunctionSourceRequired] = "Le champ source est obligatoire.",
            [MessageCodes.FunctionTargetInvalidName] = "Le champ cible '{0}' n'est pas un nom logique valide (lettres minuscules, chiffres et underscore uniquement).",
            [MessageCodes.FunctionSourceInvalidName] = "Le champ source '{0}' n'est pas un nom logique valide (lettres minuscules, chiffres et underscore uniquement).",
            [MessageCodes.FunctionSeparatorRequired] = "Le séparateur est obligatoire pour la fonction Concat.",
            [MessageCodes.FunctionConfigurationRequired] = "La configuration de rollup est obligatoire.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "La configuration ne définit pas son entité parent ou son entité enfant.",
            [MessageCodes.FunctionTargetNotFound] = "Le champ cible '{0}' n'existe pas sur l'entité parent '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "Le champ source '{0}' n'existe pas sur l'entité enfant '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "Impossible de lire l'entité parent '{0}' : {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "Impossible de lire l'entité enfant '{0}' : {1}",

            [MessageCodes.ValidationTargetIsRollup] = "Le champ cible '{0}' est un champ de cumul : il ne peut pas être écrit par la fonction {1}.",
            [MessageCodes.ValidationTargetIsCalculated] = "Le champ cible '{0}' est un champ calculé ou en lecture seule : il ne peut pas être écrit par la fonction {1}.",
            [MessageCodes.ValidationTargetNotRollup] = "Le champ cible '{0}' n'est pas un champ de cumul : la fonction Refresh ne s'applique qu'à un champ de cumul.",
            [MessageCodes.ValidationFunctionUnsupported] = "Fonction {0} non prise en charge.",
            [MessageCodes.ValidationSourceNumericOrDate] = "Le champ source '{0}' ({1}) doit être numérique ou une date pour la fonction {2}.",
            [MessageCodes.ValidationSourceType] = "Le champ source '{0}' ({1}) doit être de type {2} pour la fonction {3}.",
            [MessageCodes.ValidationTargetType] = "Le champ cible '{0}' ({1}) doit être de type {2} pour la fonction {3}.",
            [MessageCodes.ValidationAveragePrecision] = "Le champ cible '{0}' ({1}) ne peut pas recevoir une moyenne sans perte de précision : utilisez un champ décimal, double ou monétaire.",
            [MessageCodes.ValidationNarrowing] = "Le champ cible '{0}' ({1}) est moins précis que le champ source '{2}' ({3}) : la fonction {4} perdrait de la précision.",

            [MessageCodes.TypeNumeric] = "numérique",
            [MessageCodes.TypeBoolean] = "booléen",
            [MessageCodes.TypeText] = "texte",
            [MessageCodes.TypeDate] = "date",

            [MessageCodes.RelationsInvalid] = "Relations de rollup invalides : {0}",
            [MessageCodes.RelationsNone] = "Aucune relation n'est renseignée.",
            [MessageCodes.RelationNotFound] = "La relation « {0} » n'existe pas.",
            [MessageCodes.RelationNotOneToMany] = "La relation « {0} » est de type N-N : seules les relations 1-N sont acceptées.",
            [MessageCodes.RelationChainInvalid] = "Le chemin n'est pas valide : la relation « {0} » aboutit à « {1} » alors que « {2} » part de « {3} ». Les relations doivent s'enchaîner du parent vers l'enfant.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Champs déclencheurs supplémentaires : noms de champs invalides : {0} (lettres minuscules, chiffres et underscore uniquement).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Champs déclencheurs supplémentaires : champs inexistants sur l'entité enfant '{0}' : {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Champs déclencheurs supplémentaires : impossible de lire l'entité enfant '{0}' : {1}",

            [MessageCodes.ChildFilterInvalid] = "Le filtre enfants : {0}",
            [MessageCodes.ParentFilterInvalid] = "Le filtre parent : {0}",
            [MessageCodes.FilterEmpty] = "le filtre est vide.",
            [MessageCodes.FilterDtd] = "il ne doit pas contenir de déclaration DTD.",
            [MessageCodes.FilterXmlInvalid] = "ce n'est pas un XML valide : {0}",
            [MessageCodes.FilterXmlNoElement] = "ce n'est pas un XML valide : aucun élément trouvé.",

            [MessageCodes.ConfigurationImmutableFields] = "Ces champs ne peuvent pas être modifiés après la création de la configuration : {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Trop de champs déclencheurs à la modification : la liste dépasse {0} caractères.",

            [MessageCodes.StepPluginTypeNotRegistered] = "Le type de plugin '{0}' n'est pas enregistré dans l'environnement.",
            [MessageCodes.StepMessageNotFound] = "Le message {0} est introuvable.",
            [MessageCodes.StepMessageNotSupported] = "L'entité '{0}' ne prend pas en charge un step {1}.",

            [MessageCodes.ExchangeRateNotFound] = "Le taux de change du parent est introuvable : la conversion de devise du champ '{0}' est impossible.",
            [MessageCodes.LookupPathInvalid] = "Chemin de lookup '{0}' invalide.",
            [MessageCodes.LookupPathEntityMissing] = "Chemin de lookup '{0}' invalide : '{1}' doit être suivi de '@' et de l'entité visée."
        };
    }
}
