namespace Ksga.RollupConfigurationEngine.Core.Localization.Languages
{
    internal static class ArabicMessages
    {
        internal static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>
        {
            [MessageCodes.FunctionInvalid] = "دالة التجميع غير صالحة: {0}",
            [MessageCodes.FunctionTypeRequired] = "نوع الدالة مطلوب.",
            [MessageCodes.FunctionTypeUnsupported] = "نوع الدالة {0} غير مدعوم.",
            [MessageCodes.FunctionTargetRequired] = "الحقل الهدف مطلوب.",
            [MessageCodes.FunctionSourceRequired] = "الحقل المصدر مطلوب.",
            [MessageCodes.FunctionTargetInvalidName] = "الحقل الهدف '{0}' ليس اسمًا منطقيًا صالحًا (أحرف صغيرة وأرقام وشرطة سفلية فقط).",
            [MessageCodes.FunctionSourceInvalidName] = "الحقل المصدر '{0}' ليس اسمًا منطقيًا صالحًا (أحرف صغيرة وأرقام وشرطة سفلية فقط).",
            [MessageCodes.FunctionSeparatorRequired] = "الفاصل مطلوب للدالة Concat.",
            [MessageCodes.FunctionConfigurationRequired] = "إعداد التجميع مطلوب.",
            [MessageCodes.FunctionConfigurationEntitiesMissing] = "لا يحدد الإعداد الكيان الأب أو الكيان الابن.",
            [MessageCodes.FunctionTargetNotFound] = "الحقل الهدف '{0}' غير موجود في الكيان الأب '{1}'.",
            [MessageCodes.FunctionSourceNotFound] = "الحقل المصدر '{0}' غير موجود في الكيان الابن '{1}'.",
            [MessageCodes.FunctionParentEntityUnreadable] = "تعذّرت قراءة الكيان الأب '{0}': {1}",
            [MessageCodes.FunctionChildEntityUnreadable] = "تعذّرت قراءة الكيان الابن '{0}': {1}",

            [MessageCodes.ValidationTargetIsRollup] = "الحقل الهدف '{0}' حقل تجميعي: لا يمكن للدالة {1} الكتابة فيه.",
            [MessageCodes.ValidationTargetIsCalculated] = "الحقل الهدف '{0}' حقل محسوب أو للقراءة فقط: لا يمكن للدالة {1} الكتابة فيه.",
            [MessageCodes.ValidationTargetNotRollup] = "الحقل الهدف '{0}' ليس حقلًا تجميعيًا: تنطبق الدالة Refresh على الحقل التجميعي فقط.",
            [MessageCodes.ValidationFunctionUnsupported] = "الدالة {0} غير مدعومة.",
            [MessageCodes.ValidationSourceNumericOrDate] = "يجب أن يكون الحقل المصدر '{0}' ({1}) رقميًا أو تاريخًا للدالة {2}.",
            [MessageCodes.ValidationSourceType] = "يجب أن يكون الحقل المصدر '{0}' ({1}) من النوع {2} للدالة {3}.",
            [MessageCodes.ValidationTargetType] = "يجب أن يكون الحقل الهدف '{0}' ({1}) من النوع {2} للدالة {3}.",
            [MessageCodes.ValidationAveragePrecision] = "لا يمكن للحقل الهدف '{0}' ({1}) استقبال متوسط دون فقدان الدقة: استخدم حقلًا عشريًا أو مزدوجًا أو نقديًا.",
            [MessageCodes.ValidationNarrowing] = "الحقل الهدف '{0}' ({1}) أقل دقة من الحقل المصدر '{2}' ({3}): ستفقد الدالة {4} الدقة.",

            [MessageCodes.TypeNumeric] = "رقمي",
            [MessageCodes.TypeBoolean] = "منطقي",
            [MessageCodes.TypeText] = "نصي",
            [MessageCodes.TypeDate] = "تاريخ",

            [MessageCodes.RelationsInvalid] = "علاقات التجميع غير صالحة: {0}",
            [MessageCodes.RelationsNone] = "لم يتم تحديد أي علاقة.",
            [MessageCodes.RelationNotFound] = "العلاقة '{0}' غير موجودة.",
            [MessageCodes.RelationNotOneToMany] = "العلاقة '{0}' من النوع N:N: تُقبل علاقات 1:N فقط.",
            [MessageCodes.RelationChainInvalid] = "المسار غير صالح: العلاقة '{0}' تنتهي عند '{1}' بينما تبدأ '{2}' من '{3}'. يجب ربط العلاقات من الأب إلى الابن.",

            [MessageCodes.AdditionalTriggersInvalidNames] = "حقول التشغيل الإضافية: أسماء حقول غير صالحة: {0} (أحرف صغيرة وأرقام وشرطة سفلية فقط).",
            [MessageCodes.AdditionalTriggersMissingFields] = "حقول التشغيل الإضافية: هذه الحقول غير موجودة في الكيان الابن '{0}': {1}.",
            [MessageCodes.AdditionalTriggersEntityUnreadable] = "حقول التشغيل الإضافية: تعذّرت قراءة الكيان الابن '{0}': {1}",

            [MessageCodes.ChildFilterInvalid] = "مرشّح السجلات الفرعية: {0}",
            [MessageCodes.ParentFilterInvalid] = "مرشّح السجل الأب: {0}",
            [MessageCodes.FilterEmpty] = "المرشّح فارغ.",
            [MessageCodes.FilterDtd] = "يجب ألا يحتوي على تصريح DTD.",
            [MessageCodes.FilterXmlInvalid] = "ليس XML صالحًا: {0}",
            [MessageCodes.FilterXmlNoElement] = "ليس XML صالحًا: لم يتم العثور على أي عنصر.",

            [MessageCodes.ConfigurationImmutableFields] = "لا يمكن تعديل هذه الحقول بعد إنشاء الإعداد: {0}.",
            [MessageCodes.TriggerFieldsTooLong] = "عدد كبير جدًا من حقول التشغيل عند التعديل: تتجاوز القائمة {0} حرفًا.",

            [MessageCodes.StepPluginTypeNotRegistered] = "نوع المكوّن الإضافي '{0}' غير مسجّل في البيئة.",
            [MessageCodes.StepMessageNotFound] = "الرسالة {0} غير موجودة.",
            [MessageCodes.StepMessageNotSupported] = "الكيان '{0}' لا يدعم خطوة {1}.",

            [MessageCodes.ExchangeRateNotFound] = "تعذّر العثور على سعر الصرف للسجل الأب: تحويل العملة للحقل '{0}' مستحيل.",
            [MessageCodes.LookupPathInvalid] = "مسار البحث '{0}' غير صالح.",
            [MessageCodes.LookupPathEntityMissing] = "مسار البحث '{0}' غير صالح: يجب أن يتبع '{1}' الرمز '@' ثم الكيان المقصود."
        };
    }
}
