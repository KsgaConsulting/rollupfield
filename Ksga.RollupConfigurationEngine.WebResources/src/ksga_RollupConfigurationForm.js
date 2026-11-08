var Ksga = Ksga || {};
Ksga.RollupConfiguration = Ksga.RollupConfiguration || {};

Ksga.RollupConfiguration.Form = {

    formContext: null,
    isCreate: false,

    fields: {
        name: "ksga_name",
        relations: "ksga_relations",
        parentEntity: "ksga_parententity",
        childEntity: "ksga_childentity",
        lookupPath: "ksga_lookuppath",
        childFilter: "ksga_childfilter",
        parentExecutionFilter: "ksga_parentexecutionfilter",
        triggerOnCreate: "ksga_triggeroncreate",
        triggerOnDelete: "ksga_triggerondelete",
        additionalTriggerOnUpdate: "ksga_additionaltriggeronupdate",
        triggerOnUpdate: "ksga_triggeronupdate"
    },

    functionsGrid: "Subgrid_Functions",

    createHiddenFields: [
        "ksga_name", "ksga_parententity", "ksga_childentity", "ksga_lookuppath",
        "ksga_childfilter", "ksga_parentexecutionfilter", "ksga_triggeroncreate", "ksga_triggerondelete",
        "ksga_additionaltriggeronupdate", "ksga_triggeronupdate", "ksga_executionmode"
    ],

    updateVisibleFields: [
        "ksga_name", "ksga_relations", "ksga_parententity", "ksga_childentity", "ksga_lookuppath",
        "ksga_childfilter", "ksga_parentexecutionfilter", "ksga_triggeroncreate", "ksga_triggerondelete",
        "ksga_additionaltriggeronupdate", "ksga_triggeronupdate", "ksga_executionmode"
    ],

    lockedFields: ["ksga_relations", "ksga_parententity", "ksga_childentity", "ksga_lookuppath", "ksga_triggeronupdate"],

    formTypeCreate: 1,

    relationNamePattern: /^[A-Za-z_][A-Za-z0-9_]*$/,

    logicalNamePattern: /^[a-z_][a-z0-9_]*$/,

    onLoad: function (executionContext) {

        this.formContext = executionContext.getFormContext();
        this.isCreate = this.formContext.ui.getFormType() === this.formTypeCreate;

        this.bindEvents();

        this.applyUI();

        this.registerPickers();

    },

    registerPickers: function () {

        var self = this;

        if (typeof Ksga.FieldPicker === "undefined" || typeof Ksga.FieldPicker.register !== "function" || this.isCreate) {
            return;
        }

        Ksga.FieldPicker.register(this.formContext, this.fields.additionalTriggerOnUpdate, {
            multi: true,
            getEntries: function () {
                return Promise.resolve(Ksga.FieldPicker.loadCatalog(self.getValue(self.fields.childEntity))).then(function (catalog) {
                    return Ksga.FieldPicker.entriesFromCatalog(catalog, function (field, name) {
                        return !/_base$/.test(name);
                    });
                });
            }
        });

    },

    bindEvents: function () {

        this.addOnChange(this.fields.relations, this.onRelationsChange);
        this.addOnChange(this.fields.childFilter, this.onChildFilterChange);
        this.addOnChange(this.fields.parentExecutionFilter, this.onParentFilterChange);
        this.addOnChange(this.fields.additionalTriggerOnUpdate, this.onAdditionalTriggerChange);

        this.formContext.data.entity.addOnSave(this.onSave.bind(this));

    },

    applyUI: function () {

        var self = this;

        if (this.isCreate) {
            this.setVisible(this.fields.relations, true);
            this.setRequired(this.fields.relations, true);
            this.setDisabled(this.fields.relations, false);

            this.createHiddenFields.forEach(function (fieldName) {
                self.setVisible(fieldName, false);
            });

            this.setControlVisible(this.functionsGrid, false);

            return;
        }

        this.setRequired(this.fields.relations, false);

        this.updateVisibleFields.forEach(function (fieldName) {
            self.setVisible(fieldName, true);
        });

        this.lockedFields.forEach(function (fieldName) {
            self.setDisabled(fieldName, true);
        });

        this.setControlVisible(this.functionsGrid, true);

    },

    onRelationsChange: function () {

        this.clearNotification(this.fields.relations);

        this.normalizeRelations();

    },

    onChildFilterChange: function () {

        this.clearNotification(this.fields.childFilter);

    },

    onParentFilterChange: function () {

        this.clearNotification(this.fields.parentExecutionFilter);

    },

    onAdditionalTriggerChange: function () {

        this.clearNotification(this.fields.additionalTriggerOnUpdate);

        this.normalizeAdditionalTriggers();

    },

    onSave: function (executionContext) {

        var errors = this.validate();

        if (errors.length > 0) {
            executionContext.getEventArgs().preventDefault();
        }

    },

    validate: function () {

        var errors = [];

        this.clearNotification(this.fields.relations);
        this.clearNotification(this.fields.childFilter);
        this.clearNotification(this.fields.parentExecutionFilter);
        this.clearNotification(this.fields.additionalTriggerOnUpdate);

        if (this.isCreate) {
            this.normalizeRelations();
            this.validateRelations(errors);
        }

        this.validateFilter(this.fields.childFilter, "FORM_CHILD_FILTER_INVALID", errors);
        this.validateFilter(this.fields.parentExecutionFilter, "FORM_PARENT_FILTER_INVALID", errors);

        this.normalizeAdditionalTriggers();
        this.validateAdditionalTriggers(errors);

        return errors;

    },

    normalizeRelations: function () {

        var attribute = this.formContext.getAttribute(this.fields.relations);

        if (!attribute) {
            return;
        }

        var value = attribute.getValue();

        if (typeof value !== "string") {
            return;
        }

        var normalized = this.cleanRelations(value);

        if (normalized !== value) {
            attribute.setValue(normalized);
        }

    },

    splitFieldNames: function (value) {

        return value
            .split(/[,;\s]+/)
            .filter(function (name) { return name.length > 0; });

    },

    normalizeAdditionalTriggers: function () {

        var attribute = this.formContext.getAttribute(this.fields.additionalTriggerOnUpdate);

        if (!attribute) {
            return;
        }

        var value = attribute.getValue();

        if (typeof value !== "string") {
            return;
        }

        var normalized = this.splitFieldNames(value).join(",");

        if (normalized !== value) {
            attribute.setValue(normalized.length > 0 ? normalized : null);
        }

    },

    validateAdditionalTriggers: function (errors) {

        var value = this.getValue(this.fields.additionalTriggerOnUpdate);

        if (typeof value !== "string" || value.length === 0) {
            return;
        }

        var self = this;

        var invalid = this.splitFieldNames(value).filter(function (name) {
            return !self.logicalNamePattern.test(name);
        });

        if (invalid.length > 0) {
            this.addError(
                this.fields.additionalTriggerOnUpdate,
                this.t("FORM_ADDITIONAL_INVALID", [invalid.join(", ")]),
                errors);
        }

    },

    cleanRelations: function (value) {

        return this.splitRelations(value).join(",");

    },

    splitRelations: function (value) {

        return value
            .replace(/\s+/g, "")
            .split(/[,;]/)
            .filter(function (name) { return name.length > 0; });

    },

    validateRelations: function (errors) {

        var value = this.getValue(this.fields.relations);
        var names = typeof value === "string" ? this.splitRelations(value) : [];
        var self = this;

        if (names.length === 0) {
            this.addError(this.fields.relations, this.t("FORM_RELATIONS_REQUIRED"), errors);
            return;
        }

        var invalid = names.filter(function (name) { return !self.relationNamePattern.test(name); });

        if (invalid.length > 0) {
            this.addError(
                this.fields.relations,
                this.t("FORM_RELATION_INVALID", [invalid.join(", ")]),
                errors);
        }

    },

    validateFilter: function (fieldName, code, errors) {

        var value = this.getValue(fieldName);

        if (typeof value !== "string" || value.trim().length === 0) {
            return;
        }

        var problem = this.checkFilterXml(value.trim());

        if (problem) {
            this.addError(fieldName, this.t(code, [problem]), errors);
        }

    },

    checkFilterXml: function (filter) {

        if (/<!DOCTYPE|<!ENTITY/i.test(filter)) {
            return this.t("FILTER_DTD");
        }

        if (typeof DOMParser === "undefined") {
            return filter.charAt(0) === "<" && filter.charAt(filter.length - 1) === ">"
                ? null
                : this.t("FORM_FILTER_XML_INVALID");
        }

        var document = new DOMParser().parseFromString("<fragment>" + filter + "</fragment>", "application/xml");

        if (document.getElementsByTagName("parsererror").length > 0 || document.documentElement.childNodes.length === 0) {
            return this.t("FORM_FILTER_XML_INVALID");
        }

        return null;

    },

    getValue: function (fieldName) {

        var attribute = this.formContext.getAttribute(fieldName);

        return attribute ? attribute.getValue() : null;

    },

    setVisible: function (fieldName, visible) {

        this.getControls(fieldName).forEach(function (control) {
            control.setVisible(visible);
        });

    },

    setControlVisible: function (controlName, visible) {

        var control = this.formContext.getControl(controlName);

        if (control) {
            control.setVisible(visible);
        }

    },

    setDisabled: function (fieldName, disabled) {

        this.getControls(fieldName).forEach(function (control) {
            control.setDisabled(disabled);
        });

    },

    setRequired: function (fieldName, required) {

        var attribute = this.formContext.getAttribute(fieldName);

        if (attribute) {
            attribute.setRequiredLevel(required ? "required" : "none");
        }

    },

    getControls: function (fieldName) {

        var attribute = this.formContext.getAttribute(fieldName);

        return attribute ? attribute.controls.get() : [];

    },

    t: function (code, args) {

        return typeof Ksga.Localization !== "undefined" ? Ksga.Localization.t(code, args) : code;

    },

    addError: function (fieldName, message, errors) {

        var id = this.notificationId(fieldName);

        this.getControls(fieldName).forEach(function (control) {
            control.setNotification(message, id);
        });

        errors.push(message);

    },

    clearNotification: function (fieldName) {

        var id = this.notificationId(fieldName);

        this.getControls(fieldName).forEach(function (control) {
            control.clearNotification(id);
        });

    },

    notificationId: function (fieldName) {

        return fieldName + "_invalid";

    },

    addOnChange: function (fieldName, handler) {

        var attribute = this.formContext.getAttribute(fieldName);

        if (attribute) {
            attribute.addOnChange(handler.bind(this));
        }

    }

};
