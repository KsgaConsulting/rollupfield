namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class SpanishMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Función de rollup no válida: {0}",
            [MessageCodes.FunctionTypeRequired] = "El tipo de función es obligatorio.",
            [MessageCodes.FunctionTypeUnsupported] = "El tipo de función {0} no es compatible.",
            [MessageCodes.FunctionTargetRequired] = "El campo de destino es obligatorio.",
            [MessageCodes.FunctionSourceRequired] = "El campo de origen es obligatorio.",
            [MessageCodes.FunctionTargetInvalidName] = "El campo de destino '{0}' no es un nombre lógico válido (solo letras minúsculas, dígitos y guion bajo).",
            [MessageCodes.FunctionSourceInvalidName] = "El campo de origen '{0}' no es un nombre lógico válido (solo letras minúsculas, dígitos y guion bajo).",
            [MessageCodes.FunctionSeparatorRequired] = "El separador es obligatorio para la función Concat.",
            [MessageCodes.FunctionConfigurationRequired] = "La configuración de rollup es obligatoria.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "La configuración no define su entidad principal o su entidad secundaria.",
            [MessageCodes.FunctionTargetNotFound] = "El campo de destino '{0}' no existe en la entidad principal '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "El campo de origen '{0}' no existe en la entidad secundaria '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "No se puede leer la entidad principal '{0}': {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "No se puede leer la entidad secundaria '{0}': {1}",

            [MessageCodes.ValidationTargetIsRollup] = "El campo de destino '{0}' es un campo de acumulación: la función {1} no puede escribir en él.",
            [MessageCodes.ValidationTargetIsCalculated] = "El campo de destino '{0}' es un campo calculado o de solo lectura: la función {1} no puede escribir en él.",
            [MessageCodes.ValidationTargetNotRollup] = "El campo de destino '{0}' no es un campo de acumulación: la función Refresh solo se aplica a un campo de acumulación.",
            [MessageCodes.ValidationFunctionUnsupported] = "La función {0} no es compatible.",
            [MessageCodes.ValidationSourceNumericOrDate] = "El campo de origen '{0}' ({1}) debe ser numérico o una fecha para la función {2}.",
            [MessageCodes.ValidationSourceType] = "El campo de origen '{0}' ({1}) debe ser de tipo {2} para la función {3}.",
            [MessageCodes.ValidationTargetType] = "El campo de destino '{0}' ({1}) debe ser de tipo {2} para la función {3}.",
            [MessageCodes.ValidationAveragePrecision] = "El campo de destino '{0}' ({1}) no puede recibir un promedio sin perder precisión: use un campo decimal, doble o de moneda.",
            [MessageCodes.ValidationNarrowing] = "El campo de destino '{0}' ({1}) es menos preciso que el campo de origen '{2}' ({3}): la función {4} perdería precisión.",

            [MessageCodes.TypeNumeric] = "numérico",
            [MessageCodes.TypeBoolean] = "booleano",
            [MessageCodes.TypeText] = "texto",
            [MessageCodes.TypeDate] = "fecha",

            [MessageCodes.RelationsInvalid] = "Relaciones de rollup no válidas: {0}",
            [MessageCodes.RelationsNone] = "No se ha indicado ninguna relación.",
            [MessageCodes.RelationNotFound] = "La relación '{0}' no existe.",
            [MessageCodes.RelationNotOneToMany] = "La relación '{0}' es de tipo N:N: solo se aceptan relaciones 1:N.",
            [MessageCodes.RelationChainInvalid] = "La ruta no es válida: la relación '{0}' termina en '{1}', pero '{2}' parte de '{3}'. Las relaciones deben encadenarse del principal al secundario.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Campos desencadenantes adicionales: nombres de campo no válidos: {0} (solo letras minúsculas, dígitos y guion bajo).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Campos desencadenantes adicionales: estos campos no existen en la entidad secundaria '{0}': {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Campos desencadenantes adicionales: no se puede leer la entidad secundaria '{0}': {1}",

            [MessageCodes.ChildFilterInvalid] = "El filtro de secundarios: {0}",
            [MessageCodes.ParentFilterInvalid] = "El filtro del principal: {0}",
            [MessageCodes.FilterEmpty] = "el filtro está vacío.",
            [MessageCodes.FilterDtd] = "no debe contener una declaración DTD.",
            [MessageCodes.FilterXmlInvalid] = "no es un XML válido: {0}",
            [MessageCodes.FilterXmlNoElement] = "no es un XML válido: no se encontró ningún elemento.",

            [MessageCodes.ConfigurationImmutableFields] = "Estos campos no se pueden modificar después de crear la configuración: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Demasiados campos desencadenantes de la modificación: la lista supera los {0} caracteres.",

            [MessageCodes.StepPluginTypeNotRegistered] = "El tipo de plugin '{0}' no está registrado en el entorno.",
            [MessageCodes.StepMessageNotFound] = "No se encuentra el mensaje {0}.",
            [MessageCodes.StepMessageNotSupported] = "La entidad '{0}' no admite un paso {1}.",

            [MessageCodes.ExchangeRateNotFound] = "No se encuentra el tipo de cambio del principal: la conversión de moneda del campo '{0}' es imposible.",
            [MessageCodes.LookupPathInvalid] = "Ruta de búsqueda '{0}' no válida.",
            [MessageCodes.LookupPathEntityMissing] = "Ruta de búsqueda '{0}' no válida: '{1}' debe ir seguido de '@' y de la entidad a la que apunta."
        };
    }
}
