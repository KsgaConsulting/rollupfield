namespace Ksga.RollupConfigurationEngine.Core.Localization
{
    public static class MessageCodes
    {
        public const string FunctionInvalid = "FUNCTION_INVALID";
        public const string FunctionTypeRequired = "FUNCTION_TYPE_REQUIRED";
        public const string FunctionTypeUnsupported = "FUNCTION_TYPE_UNSUPPORTED";
        public const string FunctionTargetRequired = "FUNCTION_TARGET_REQUIRED";
        public const string FunctionSourceRequired = "FUNCTION_SOURCE_REQUIRED";
        public const string FunctionTargetInvalidName = "FUNCTION_TARGET_INVALID_NAME";
        public const string FunctionSourceInvalidName = "FUNCTION_SOURCE_INVALID_NAME";
        public const string FunctionSeparatorRequired = "FUNCTION_SEPARATOR_REQUIRED";
        public const string FunctionConfigurationRequired = "FUNCTION_CONFIGURATION_REQUIRED";
        public const string FunctionConfigurationEntitiesMissing = "FUNCTION_CONFIGURATION_ENTITIES_MISSING";
        public const string FunctionTargetNotFound = "FUNCTION_TARGET_NOT_FOUND";
        public const string FunctionSourceNotFound = "FUNCTION_SOURCE_NOT_FOUND";
        public const string FunctionParentEntityUnreadable = "FUNCTION_PARENT_ENTITY_UNREADABLE";
        public const string FunctionChildEntityUnreadable = "FUNCTION_CHILD_ENTITY_UNREADABLE";

        public const string ValidationTargetIsRollup = "VALIDATION_TARGET_IS_ROLLUP";
        public const string ValidationTargetIsCalculated = "VALIDATION_TARGET_IS_CALCULATED";
        public const string ValidationTargetNotRollup = "VALIDATION_TARGET_NOT_ROLLUP";
        public const string ValidationFunctionUnsupported = "VALIDATION_FUNCTION_UNSUPPORTED";
        public const string ValidationSourceNumericOrDate = "VALIDATION_SOURCE_NUMERIC_OR_DATE";
        public const string ValidationSourceType = "VALIDATION_SOURCE_TYPE";
        public const string ValidationTargetType = "VALIDATION_TARGET_TYPE";
        public const string ValidationAveragePrecision = "VALIDATION_AVERAGE_PRECISION";
        public const string ValidationNarrowing = "VALIDATION_NARROWING";

        public const string TypeNumeric = "TYPE_NUMERIC";
        public const string TypeBoolean = "TYPE_BOOLEAN";
        public const string TypeText = "TYPE_TEXT";
        public const string TypeDate = "TYPE_DATE";

        public const string RelationsInvalid = "RELATIONS_INVALID";
        public const string RelationsNone = "RELATIONS_NONE";
        public const string RelationNotFound = "RELATION_NOT_FOUND";
        public const string RelationNotOneToMany = "RELATION_NOT_ONE_TO_MANY";
        public const string RelationChainInvalid = "RELATION_CHAIN_INVALID";

        public const string AdditionalTriggersInvalidNames = "ADDITIONAL_TRIGGERS_INVALID_NAMES";
        public const string AdditionalTriggersMissingFields = "ADDITIONAL_TRIGGERS_MISSING_FIELDS";
        public const string AdditionalTriggersEntityUnreadable = "ADDITIONAL_TRIGGERS_ENTITY_UNREADABLE";

        public const string ChildFilterInvalid = "CHILD_FILTER_INVALID";
        public const string ParentFilterInvalid = "PARENT_FILTER_INVALID";
        public const string FilterEmpty = "FILTER_EMPTY";
        public const string FilterDtd = "FILTER_DTD";
        public const string FilterXmlInvalid = "FILTER_XML_INVALID";
        public const string FilterXmlNoElement = "FILTER_XML_NO_ELEMENT";

        public const string ConfigurationImmutableFields = "CONFIGURATION_IMMUTABLE_FIELDS";
        public const string TriggerFieldsTooLong = "TRIGGER_FIELDS_TOO_LONG";

        public const string StepPluginTypeNotRegistered = "STEP_PLUGIN_TYPE_NOT_REGISTERED";
        public const string StepMessageNotFound = "STEP_MESSAGE_NOT_FOUND";
        public const string StepMessageNotSupported = "STEP_MESSAGE_NOT_SUPPORTED";

        public const string ExchangeRateNotFound = "EXCHANGE_RATE_NOT_FOUND";
        public const string LookupPathInvalid = "LOOKUP_PATH_INVALID";
        public const string LookupPathEntityMissing = "LOOKUP_PATH_ENTITY_MISSING";
    }
}
