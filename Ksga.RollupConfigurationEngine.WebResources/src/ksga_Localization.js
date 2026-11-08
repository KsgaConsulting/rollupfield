var Ksga = Ksga || {};

Ksga.Localization = {

    defaultLanguage: "en",

    languages: ["en", "fr", "es", "it", "pt", "de", "nl", "ar"],

    rtlLanguages: ["ar"],

    primaryLanguageIds: { 1: "ar", 7: "de", 9: "en", 10: "es", 12: "fr", 16: "it", 19: "nl", 22: "pt" },

    packs: {},

    forced: null,

    detected: null,

    add: function (language, messages) {

        var pack = this.packs[language] = this.packs[language] || {};

        Object.keys(messages).forEach(function (code) {
            pack[code] = messages[code];
        });

    },

    normalize: function (language) {

        if (typeof language !== "string") {
            return this.defaultLanguage;
        }

        var code = language.trim().toLowerCase().split(/[-_]/)[0];

        return this.languages.indexOf(code) !== -1 ? code : this.defaultLanguage;

    },

    fromLcid: function (lcid) {

        if (typeof lcid !== "number" || lcid <= 0) {
            return this.defaultLanguage;
        }

        return this.primaryLanguageIds[lcid & 0x3FF] || this.defaultLanguage;

    },

    setLanguage: function (language) {

        this.forced = language ? this.normalize(language) : null;
        this.detected = null;

    },

    getXrm: function () {

        var candidates = [];

        try { candidates.push(window.Xrm); } catch (error) { }
        try { candidates.push(window.parent && window.parent.Xrm); } catch (error) { }
        try { candidates.push(window.top && window.top.Xrm); } catch (error) { }
        try { candidates.push(typeof Xrm !== "undefined" ? Xrm : null); } catch (error) { }

        for (var i = 0; i < candidates.length; i++) {
            if (candidates[i] && candidates[i].Utility && typeof candidates[i].Utility.getGlobalContext === "function") {
                return candidates[i];
            }
        }

        return null;

    },

    getLanguage: function () {

        if (this.forced) {
            return this.forced;
        }

        if (this.detected) {
            return this.detected;
        }

        var xrm = this.getXrm();

        if (!xrm) {
            return this.defaultLanguage;
        }

        try {
            var settings = xrm.Utility.getGlobalContext().userSettings;

            this.detected = this.fromLcid(settings && settings.languageId);
        } catch (error) {
            this.detected = this.defaultLanguage;
        }

        return this.detected;

    },

    isRtl: function () {

        return this.rtlLanguages.indexOf(this.getLanguage()) !== -1;

    },

    format: function (template, args) {

        return String(template).replace(/\{(\d+)\}/g, function (match, index) {
            var value = args ? args[Number(index)] : undefined;

            return value === undefined || value === null ? match : String(value);
        });

    },

    t: function (code, args) {

        var language = this.getLanguage();
        var pack = this.packs[language];
        var fallback = this.packs[this.defaultLanguage];
        var template = (pack && pack[code]) || (fallback && fallback[code]) || code;

        return this.format(template, args);

    },

    error: function (code, args) {

        var failure = new Error(this.t(code, args));

        failure.code = code;
        failure.args = args || [];

        return failure;

    },

    apply: function (root) {

        var self = this;
        var scope = root || document;

        document.documentElement.lang = this.getLanguage();
        document.documentElement.dir = this.isRtl() ? "rtl" : "ltr";

        [["data-i18n", "textContent"], ["data-i18n-placeholder", "placeholder"], ["data-i18n-title", "title"]].forEach(function (pair) {
            Array.prototype.forEach.call(scope.querySelectorAll("[" + pair[0] + "]"), function (element) {
                element[pair[1]] = self.t(element.getAttribute(pair[0]));
            });
        });

    },

    init: function () {

        return this.getLanguage();

    }

};

Ksga.Localization.add("en", {
    FORM_LOGICAL_NAME_INVALID: "Invalid logical name: lowercase letters, digits and underscore only.",
    FORM_SEPARATOR_REQUIRED: "The separator is required for the Concat function.",
    FORM_FIELD_NOT_ON_PARENT: "This field does not exist on the parent entity of the configuration.",
    FORM_FIELD_NOT_ON_CHILD: "This field does not exist on the child entity of the configuration.",
    FORM_CHILD_FILTER_INVALID: "The child filter: {0}",
    FORM_PARENT_FILTER_INVALID: "The parent execution filter: {0}",
    FILTER_DTD: "DTD declarations are not allowed.",
    FORM_FILTER_XML_INVALID: "the XML is not valid.",
    FORM_ADDITIONAL_INVALID: "Invalid field name: {0}. Lowercase logical names, separated by a comma.",
    FORM_RELATION_INVALID: "Invalid relationship name: {0}. Letters, digits and underscore only, not starting with a digit.",
    FORM_RELATIONS_REQUIRED: "Enter at least one 1-N relationship.",
    CMD_SELECT_AT_LEAST_ONE: "Select at least one configuration to export.",
    CMD_TITLE_EXPORT: "Export configurations",
    CMD_TITLE_IMPORT: "Import configurations"
});

Ksga.Localization.add("fr", {
    FORM_LOGICAL_NAME_INVALID: "Nom logique invalide : lettres minuscules, chiffres et underscore uniquement.",
    FORM_SEPARATOR_REQUIRED: "Le séparateur est obligatoire pour la fonction Concat.",
    FORM_FIELD_NOT_ON_PARENT: "Ce champ n'existe pas sur l'entité parent de la configuration.",
    FORM_FIELD_NOT_ON_CHILD: "Ce champ n'existe pas sur l'entité enfant de la configuration.",
    FORM_CHILD_FILTER_INVALID: "Le filtre enfants : {0}",
    FORM_PARENT_FILTER_INVALID: "Le filtre d'exécution du parent : {0}",
    FILTER_DTD: "les déclarations DTD ne sont pas autorisées.",
    FORM_FILTER_XML_INVALID: "le XML n'est pas valide.",
    FORM_ADDITIONAL_INVALID: "Nom de champ invalide : {0}. Noms logiques en minuscules, séparés par une virgule.",
    FORM_RELATION_INVALID: "Nom de relation invalide : {0}. Lettres, chiffres et underscore uniquement, sans commencer par un chiffre.",
    FORM_RELATIONS_REQUIRED: "Saisissez au moins une relation 1-N.",
    CMD_SELECT_AT_LEAST_ONE: "Sélectionnez au moins une configuration à exporter.",
    CMD_TITLE_EXPORT: "Exporter des configurations",
    CMD_TITLE_IMPORT: "Importer des configurations"
});

Ksga.Localization.add("es", {
    FORM_LOGICAL_NAME_INVALID: "Nombre lógico no válido: solo letras minúsculas, dígitos y guion bajo.",
    FORM_SEPARATOR_REQUIRED: "El separador es obligatorio para la función Concat.",
    FORM_FIELD_NOT_ON_PARENT: "Este campo no existe en la entidad principal de la configuración.",
    FORM_FIELD_NOT_ON_CHILD: "Este campo no existe en la entidad secundaria de la configuración.",
    FORM_CHILD_FILTER_INVALID: "El filtro de secundarios: {0}",
    FORM_PARENT_FILTER_INVALID: "El filtro de ejecución del principal: {0}",
    FILTER_DTD: "las declaraciones DTD no están permitidas.",
    FORM_FILTER_XML_INVALID: "el XML no es válido.",
    FORM_ADDITIONAL_INVALID: "Nombre de campo no válido: {0}. Nombres lógicos en minúsculas, separados por una coma.",
    FORM_RELATION_INVALID: "Nombre de relación no válido: {0}. Solo letras, dígitos y guion bajo, sin empezar por un dígito.",
    FORM_RELATIONS_REQUIRED: "Introduzca al menos una relación 1-N.",
    CMD_SELECT_AT_LEAST_ONE: "Seleccione al menos una configuración para exportar.",
    CMD_TITLE_EXPORT: "Exportar configuraciones",
    CMD_TITLE_IMPORT: "Importar configuraciones"
});

Ksga.Localization.add("it", {
    FORM_LOGICAL_NAME_INVALID: "Nome logico non valido: solo lettere minuscole, cifre e underscore.",
    FORM_SEPARATOR_REQUIRED: "Il separatore è obbligatorio per la funzione Concat.",
    FORM_FIELD_NOT_ON_PARENT: "Questo campo non esiste nell'entità padre della configurazione.",
    FORM_FIELD_NOT_ON_CHILD: "Questo campo non esiste nell'entità figlia della configurazione.",
    FORM_CHILD_FILTER_INVALID: "Il filtro dei figli: {0}",
    FORM_PARENT_FILTER_INVALID: "Il filtro di esecuzione del padre: {0}",
    FILTER_DTD: "le dichiarazioni DTD non sono consentite.",
    FORM_FILTER_XML_INVALID: "l'XML non è valido.",
    FORM_ADDITIONAL_INVALID: "Nome di campo non valido: {0}. Nomi logici in minuscolo, separati da una virgola.",
    FORM_RELATION_INVALID: "Nome di relazione non valido: {0}. Solo lettere, cifre e underscore, senza iniziare con una cifra.",
    FORM_RELATIONS_REQUIRED: "Inserire almeno una relazione 1-N.",
    CMD_SELECT_AT_LEAST_ONE: "Selezionare almeno una configurazione da esportare.",
    CMD_TITLE_EXPORT: "Esporta configurazioni",
    CMD_TITLE_IMPORT: "Importa configurazioni"
});

Ksga.Localization.add("pt", {
    FORM_LOGICAL_NAME_INVALID: "Nome lógico inválido: apenas letras minúsculas, dígitos e sublinhado.",
    FORM_SEPARATOR_REQUIRED: "O separador é obrigatório para a função Concat.",
    FORM_FIELD_NOT_ON_PARENT: "Este campo não existe na entidade principal da configuração.",
    FORM_FIELD_NOT_ON_CHILD: "Este campo não existe na entidade secundária da configuração.",
    FORM_CHILD_FILTER_INVALID: "O filtro dos secundários: {0}",
    FORM_PARENT_FILTER_INVALID: "O filtro de execução do principal: {0}",
    FILTER_DTD: "as declarações DTD não são permitidas.",
    FORM_FILTER_XML_INVALID: "o XML não é válido.",
    FORM_ADDITIONAL_INVALID: "Nome de campo inválido: {0}. Nomes lógicos em minúsculas, separados por vírgula.",
    FORM_RELATION_INVALID: "Nome de relação inválido: {0}. Apenas letras, dígitos e sublinhado, sem começar por um dígito.",
    FORM_RELATIONS_REQUIRED: "Introduza pelo menos uma relação 1-N.",
    CMD_SELECT_AT_LEAST_ONE: "Selecione pelo menos uma configuração para exportar.",
    CMD_TITLE_EXPORT: "Exportar configurações",
    CMD_TITLE_IMPORT: "Importar configurações"
});

Ksga.Localization.add("de", {
    FORM_LOGICAL_NAME_INVALID: "Ungültiger logischer Name: nur Kleinbuchstaben, Ziffern und Unterstrich.",
    FORM_SEPARATOR_REQUIRED: "Das Trennzeichen ist für die Funktion Concat erforderlich.",
    FORM_FIELD_NOT_ON_PARENT: "Dieses Feld ist in der übergeordneten Entität der Konfiguration nicht vorhanden.",
    FORM_FIELD_NOT_ON_CHILD: "Dieses Feld ist in der untergeordneten Entität der Konfiguration nicht vorhanden.",
    FORM_CHILD_FILTER_INVALID: "Der Filter der untergeordneten Datensätze: {0}",
    FORM_PARENT_FILTER_INVALID: "Der Ausführungsfilter des übergeordneten Datensatzes: {0}",
    FILTER_DTD: "DTD-Deklarationen sind nicht erlaubt.",
    FORM_FILTER_XML_INVALID: "das XML ist nicht gültig.",
    FORM_ADDITIONAL_INVALID: "Ungültiger Feldname: {0}. Logische Namen in Kleinbuchstaben, durch Komma getrennt.",
    FORM_RELATION_INVALID: "Ungültiger Beziehungsname: {0}. Nur Buchstaben, Ziffern und Unterstrich, nicht mit einer Ziffer beginnend.",
    FORM_RELATIONS_REQUIRED: "Geben Sie mindestens eine 1:N-Beziehung ein.",
    CMD_SELECT_AT_LEAST_ONE: "Wählen Sie mindestens eine Konfiguration zum Exportieren aus.",
    CMD_TITLE_EXPORT: "Konfigurationen exportieren",
    CMD_TITLE_IMPORT: "Konfigurationen importieren"
});

Ksga.Localization.add("nl", {
    FORM_LOGICAL_NAME_INVALID: "Ongeldige logische naam: alleen kleine letters, cijfers en underscore.",
    FORM_SEPARATOR_REQUIRED: "Het scheidingsteken is verplicht voor de functie Concat.",
    FORM_FIELD_NOT_ON_PARENT: "Dit veld bestaat niet in de bovenliggende entiteit van de configuratie.",
    FORM_FIELD_NOT_ON_CHILD: "Dit veld bestaat niet in de onderliggende entiteit van de configuratie.",
    FORM_CHILD_FILTER_INVALID: "Het filter voor onderliggende records: {0}",
    FORM_PARENT_FILTER_INVALID: "Het uitvoeringsfilter van het bovenliggende record: {0}",
    FILTER_DTD: "DTD-declaraties zijn niet toegestaan.",
    FORM_FILTER_XML_INVALID: "de XML is niet geldig.",
    FORM_ADDITIONAL_INVALID: "Ongeldige veldnaam: {0}. Logische namen in kleine letters, gescheiden door een komma.",
    FORM_RELATION_INVALID: "Ongeldige relatienaam: {0}. Alleen letters, cijfers en underscore, niet beginnend met een cijfer.",
    FORM_RELATIONS_REQUIRED: "Voer minstens één 1:N-relatie in.",
    CMD_SELECT_AT_LEAST_ONE: "Selecteer minstens één configuratie om te exporteren.",
    CMD_TITLE_EXPORT: "Configuraties exporteren",
    CMD_TITLE_IMPORT: "Configuraties importeren"
});

Ksga.Localization.add("ar", {
    FORM_LOGICAL_NAME_INVALID: "اسم منطقي غير صالح: أحرف صغيرة وأرقام وشرطة سفلية فقط.",
    FORM_SEPARATOR_REQUIRED: "الفاصل مطلوب للدالة Concat.",
    FORM_FIELD_NOT_ON_PARENT: "هذا الحقل غير موجود في الكيان الأب للإعداد.",
    FORM_FIELD_NOT_ON_CHILD: "هذا الحقل غير موجود في الكيان الابن للإعداد.",
    FORM_CHILD_FILTER_INVALID: "مرشّح السجلات الفرعية: {0}",
    FORM_PARENT_FILTER_INVALID: "مرشّح تنفيذ السجل الأب: {0}",
    FILTER_DTD: "تصريحات DTD غير مسموح بها.",
    FORM_FILTER_XML_INVALID: "XML غير صالح.",
    FORM_ADDITIONAL_INVALID: "اسم حقل غير صالح: {0}. أسماء منطقية بأحرف صغيرة، مفصولة بفاصلة.",
    FORM_RELATION_INVALID: "اسم علاقة غير صالح: {0}. أحرف وأرقام وشرطة سفلية فقط، دون البدء برقم.",
    FORM_RELATIONS_REQUIRED: "أدخل علاقة واحدة على الأقل من النوع 1-N.",
    CMD_SELECT_AT_LEAST_ONE: "حدّد إعدادًا واحدًا على الأقل للتصدير.",
    CMD_TITLE_EXPORT: "تصدير الإعدادات",
    CMD_TITLE_IMPORT: "استيراد الإعدادات"
});
