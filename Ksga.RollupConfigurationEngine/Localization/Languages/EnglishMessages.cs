namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class EnglishMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Invalid rollup function: {0}",
            [MessageCodes.FunctionTypeRequired] = "The function type is required.",
            [MessageCodes.FunctionTypeUnsupported] = "The function type {0} is not supported.",
            [MessageCodes.FunctionTargetRequired] = "The target field is required.",
            [MessageCodes.FunctionSourceRequired] = "The source field is required.",
            [MessageCodes.FunctionTargetInvalidName] = "The target field '{0}' is not a valid logical name (lowercase letters, digits and underscore only).",
            [MessageCodes.FunctionSourceInvalidName] = "The source field '{0}' is not a valid logical name (lowercase letters, digits and underscore only).",
            [MessageCodes.FunctionSeparatorRequired] = "The separator is required for the Concat function.",
            [MessageCodes.FunctionConfigurationRequired] = "The rollup configuration is required.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "The configuration does not define its parent entity or its child entity.",
            [MessageCodes.FunctionTargetNotFound] = "The target field '{0}' does not exist on the parent entity '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "The source field '{0}' does not exist on the child entity '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "Unable to read the parent entity '{0}': {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "Unable to read the child entity '{0}': {1}",

            [MessageCodes.ValidationTargetIsRollup] = "The target field '{0}' is a rollup field: it cannot be written by the {1} function.",
            [MessageCodes.ValidationTargetIsCalculated] = "The target field '{0}' is a calculated or read-only field: it cannot be written by the {1} function.",
            [MessageCodes.ValidationTargetNotRollup] = "The target field '{0}' is not a rollup field: the Refresh function only applies to a rollup field.",
            [MessageCodes.ValidationFunctionUnsupported] = "The {0} function is not supported.",
            [MessageCodes.ValidationSourceNumericOrDate] = "The source field '{0}' ({1}) must be numeric or a date for the {2} function.",
            [MessageCodes.ValidationSourceType] = "The source field '{0}' ({1}) must be of type {2} for the {3} function.",
            [MessageCodes.ValidationTargetType] = "The target field '{0}' ({1}) must be of type {2} for the {3} function.",
            [MessageCodes.ValidationAveragePrecision] = "The target field '{0}' ({1}) cannot receive an average without losing precision: use a decimal, double or money field.",
            [MessageCodes.ValidationNarrowing] = "The target field '{0}' ({1}) is less precise than the source field '{2}' ({3}): the {4} function would lose precision.",

            [MessageCodes.TypeNumeric] = "numeric",
            [MessageCodes.TypeBoolean] = "boolean",
            [MessageCodes.TypeText] = "text",
            [MessageCodes.TypeDate] = "date",

            [MessageCodes.RelationsInvalid] = "Invalid rollup relationships: {0}",
            [MessageCodes.RelationsNone] = "No relationship is provided.",
            [MessageCodes.RelationNotFound] = "The relationship '{0}' does not exist.",
            [MessageCodes.RelationNotOneToMany] = "The relationship '{0}' is many-to-many: only one-to-many relationships are accepted.",
            [MessageCodes.RelationChainInvalid] = "The path is not valid: the relationship '{0}' ends at '{1}' but '{2}' starts from '{3}'. Relationships must be chained from the parent to the child.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Additional trigger fields: invalid field names: {0} (lowercase letters, digits and underscore only).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Additional trigger fields: these fields do not exist on the child entity '{0}': {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Additional trigger fields: unable to read the child entity '{0}': {1}",

            [MessageCodes.ChildFilterInvalid] = "The child filter: {0}",
            [MessageCodes.ParentFilterInvalid] = "The parent filter: {0}",
            [MessageCodes.FilterEmpty] = "the filter is empty.",
            [MessageCodes.FilterDtd] = "it must not contain a DTD declaration.",
            [MessageCodes.FilterXmlInvalid] = "it is not valid XML: {0}",
            [MessageCodes.FilterXmlNoElement] = "it is not valid XML: no element found.",

            [MessageCodes.ConfigurationImmutableFields] = "These fields cannot be changed after the configuration is created: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Too many update trigger fields: the list exceeds {0} characters.",

            [MessageCodes.StepPluginTypeNotRegistered] = "The plugin type '{0}' is not registered in the environment.",
            [MessageCodes.StepMessageNotFound] = "The message {0} cannot be found.",
            [MessageCodes.StepMessageNotSupported] = "The entity '{0}' does not support a {1} step.",

            [MessageCodes.ExchangeRateNotFound] = "The exchange rate of the parent cannot be found: the currency conversion of the field '{0}' is impossible.",
            [MessageCodes.LookupPathInvalid] = "Invalid lookup path '{0}'.",
            [MessageCodes.LookupPathEntityMissing] = "Invalid lookup path '{0}': '{1}' must be followed by '@' and the entity it points to."
        };
    }
}
