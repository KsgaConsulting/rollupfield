namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class PortugueseMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "Função de rollup inválida: {0}",
            [MessageCodes.FunctionTypeRequired] = "O tipo de função é obrigatório.",
            [MessageCodes.FunctionTypeUnsupported] = "O tipo de função {0} não é suportado.",
            [MessageCodes.FunctionTargetRequired] = "O campo de destino é obrigatório.",
            [MessageCodes.FunctionSourceRequired] = "O campo de origem é obrigatório.",
            [MessageCodes.FunctionTargetInvalidName] = "O campo de destino '{0}' não é um nome lógico válido (apenas letras minúsculas, dígitos e sublinhado).",
            [MessageCodes.FunctionSourceInvalidName] = "O campo de origem '{0}' não é um nome lógico válido (apenas letras minúsculas, dígitos e sublinhado).",
            [MessageCodes.FunctionSeparatorRequired] = "O separador é obrigatório para a função Concat.",
            [MessageCodes.FunctionConfigurationRequired] = "A configuração de rollup é obrigatória.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "A configuração não define a sua entidade principal ou a sua entidade secundária.",
            [MessageCodes.FunctionTargetNotFound] = "O campo de destino '{0}' não existe na entidade principal '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "O campo de origem '{0}' não existe na entidade secundária '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "Não é possível ler a entidade principal '{0}': {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "Não é possível ler a entidade secundária '{0}': {1}",

            [MessageCodes.ValidationTargetIsRollup] = "O campo de destino '{0}' é um campo de acumulação: não pode ser escrito pela função {1}.",
            [MessageCodes.ValidationTargetIsCalculated] = "O campo de destino '{0}' é um campo calculado ou só de leitura: não pode ser escrito pela função {1}.",
            [MessageCodes.ValidationTargetNotRollup] = "O campo de destino '{0}' não é um campo de acumulação: a função Refresh aplica-se apenas a um campo de acumulação.",
            [MessageCodes.ValidationFunctionUnsupported] = "A função {0} não é suportada.",
            [MessageCodes.ValidationSourceNumericOrDate] = "O campo de origem '{0}' ({1}) deve ser numérico ou uma data para a função {2}.",
            [MessageCodes.ValidationSourceType] = "O campo de origem '{0}' ({1}) deve ser do tipo {2} para a função {3}.",
            [MessageCodes.ValidationTargetType] = "O campo de destino '{0}' ({1}) deve ser do tipo {2} para a função {3}.",
            [MessageCodes.ValidationAveragePrecision] = "O campo de destino '{0}' ({1}) não pode receber uma média sem perda de precisão: utilize um campo decimal, double ou monetário.",
            [MessageCodes.ValidationNarrowing] = "O campo de destino '{0}' ({1}) é menos preciso do que o campo de origem '{2}' ({3}): a função {4} perderia precisão.",

            [MessageCodes.TypeNumeric] = "numérico",
            [MessageCodes.TypeBoolean] = "booleano",
            [MessageCodes.TypeText] = "texto",
            [MessageCodes.TypeDate] = "data",

            [MessageCodes.RelationsInvalid] = "Relações de rollup inválidas: {0}",
            [MessageCodes.RelationsNone] = "Nenhuma relação foi indicada.",
            [MessageCodes.RelationNotFound] = "A relação '{0}' não existe.",
            [MessageCodes.RelationNotOneToMany] = "A relação '{0}' é do tipo N:N: apenas relações 1:N são aceites.",
            [MessageCodes.RelationChainInvalid] = "O caminho não é válido: a relação '{0}' termina em '{1}', mas '{2}' parte de '{3}'. As relações devem encadear-se do principal para o secundário.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "Campos de acionamento adicionais: nomes de campo inválidos: {0} (apenas letras minúsculas, dígitos e sublinhado).",
            [MessageCodes.AdditionalTriggersMissingFields] = "Campos de acionamento adicionais: estes campos não existem na entidade secundária '{0}': {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "Campos de acionamento adicionais: não é possível ler a entidade secundária '{0}': {1}",

            [MessageCodes.ChildFilterInvalid] = "O filtro dos secundários: {0}",
            [MessageCodes.ParentFilterInvalid] = "O filtro do principal: {0}",
            [MessageCodes.FilterEmpty] = "o filtro está vazio.",
            [MessageCodes.FilterDtd] = "não deve conter uma declaração DTD.",
            [MessageCodes.FilterXmlInvalid] = "não é um XML válido: {0}",
            [MessageCodes.FilterXmlNoElement] = "não é um XML válido: nenhum elemento encontrado.",

            [MessageCodes.ConfigurationImmutableFields] = "Estes campos não podem ser alterados após a criação da configuração: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "Demasiados campos de acionamento na alteração: a lista excede {0} caracteres.",

            [MessageCodes.StepPluginTypeNotRegistered] = "O tipo de plugin '{0}' não está registado no ambiente.",
            [MessageCodes.StepMessageNotFound] = "A mensagem {0} não foi encontrada.",
            [MessageCodes.StepMessageNotSupported] = "A entidade '{0}' não suporta um passo {1}.",

            [MessageCodes.ExchangeRateNotFound] = "A taxa de câmbio do principal não foi encontrada: a conversão de moeda do campo '{0}' é impossível.",
            [MessageCodes.LookupPathInvalid] = "Caminho de lookup '{0}' inválido.",
            [MessageCodes.LookupPathEntityMissing] = "Caminho de lookup '{0}' inválido: '{1}' deve ser seguido de '@' e da entidade para a qual aponta."
        };
    }
}
