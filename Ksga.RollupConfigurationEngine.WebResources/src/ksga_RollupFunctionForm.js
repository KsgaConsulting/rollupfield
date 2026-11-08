var Ksga = Ksga || {};
Ksga.RollupFunction = Ksga.RollupFunction || {};

Ksga.RollupFunction.Form = {

    formContext: null,
    entitiesPromise: null,
    catalogs: {},
    verifiedFields: {},

    fields: {
        functionType: "ksga_functiontypecode",
        targetParentField: "ksga_targetparentfield",
        sourceChildField: "ksga_sourcechildfield",
        separator: "ksga_separator",
        convertCurrency: "ksga_convertcurrency"
    },

    functionTypes: {
        Sum: 0,
        Average: 1,
        Minimum: 2,
        Maximum: 3,
        Count: 4,
        Any: 5,
        None: 6,
        And: 7,
        Or: 8,
        Concat: 9,
        Refresh: 10
    },

    logicalNamePattern: /^[a-z_][a-z0-9_]*$/,

    onLoad: function (executionContext) {

        this.formContext = executionContext.getFormContext();
        this.entitiesPromise = null;
        this.catalogs = {};
        this.verifiedFields = {};

        this.setConversionVisible(false, false);

        this.bindEvents();

        this.applyUI(false);

        this.preloadCatalogs();

        this.registerPickers();

        this.refreshConversion(false);

    },

    registerPickers: function () {

        var self = this;

        if (typeof Ksga.FieldPicker === "undefined" || typeof Ksga.FieldPicker.register !== "function") {
            return;
        }

        Ksga.FieldPicker.register(this.formContext, this.fields.targetParentField, {
            getEntries: function () {
                return self.getPickerEntries("parentEntity", "target");
            }
        });

        Ksga.FieldPicker.register(this.formContext, this.fields.sourceChildField, {
            getEntries: function () {
                return self.getPickerEntries("childEntity", "source");
            }
        });

    },

    getPickerEntries: function (entityKey, role) {

        var self = this;
        var picker = Ksga.FieldPicker;

        return this.getEntities().then(function (entities) {

            if (!entities || !entities[entityKey]) {
                return [];
            }

            return self.getCatalog(entities[entityKey]).then(function (catalog) {

                var functionType = self.getFunctionType();
                var types = picker.typesForFunction(functionType, role);
                var refresh = functionType === self.functionTypes.Refresh;

                return picker.entriesFromCatalog(catalog, function (field, name) {

                    if (role === "source") {
                        return !/_base$/.test(name) && (!types || types.indexOf(field.type) !== -1);
                    }

                    if (refresh) {
                        return field.sourceType === 2;
                    }

                    if (field.sourceType === 1 || field.sourceType === 2 || field.sourceType === 3 || field.isValidForUpdate === false) {
                        return false;
                    }

                    return !types || types.indexOf(field.type) !== -1;

                });

            });

        }).catch(function () {
            return [];
        });

    },

    bindEvents: function () {

        var self = this;

        this.addOnChange(this.fields.functionType, this.onFunctionTypeChange);

        [this.fields.targetParentField, this.fields.sourceChildField, this.fields.separator].forEach(function (fieldName) {
            self.addOnChange(fieldName, function () {
                self.onFieldChange(fieldName);
            });
        });

        this.formContext.data.entity.addOnSave(this.onSave.bind(this));

    },

    onFunctionTypeChange: function () {

        this.clearNotifications();

        this.applyUI(true);

        this.refreshConversion(true);

    },

    onFieldChange: function (fieldName) {

        this.clearNotification(fieldName);

        delete this.verifiedFields[fieldName];

        if (this.getExistenceChecks().hasOwnProperty(fieldName)) {
            this.verifyField(fieldName);
        }

        if (fieldName === this.fields.targetParentField || fieldName === this.fields.sourceChildField) {
            this.refreshConversion(true);
        }

    },

    onSave: function (executionContext) {

        var self = this;
        var eventArgs = executionContext.getEventArgs();
        var errors = this.validate();

        if (errors.length > 0) {
            eventArgs.preventDefault();
            return;
        }

        var unverified = this.getUnverifiedFields();

        if (unverified.length === 0) {
            return;
        }

        eventArgs.preventDefault();

        var saveAndClose = typeof eventArgs.getSaveMode === "function" && eventArgs.getSaveMode() === 2;

        Promise.all(unverified.map(function (fieldName) {
            return self.verifyField(fieldName);
        })).then(function (results) {
            if (results.every(function (exists) { return exists; })) {
                self.save(saveAndClose);
            }
        });

    },

    save: function (saveAndClose) {

        if (saveAndClose) {
            this.formContext.data.entity.save("saveandclose");
        } else {
            this.formContext.data.save();
        }

    },

    validate: function () {

        var rule = this.getRule(this.getFunctionType());
        var errors = [];

        this.clearNotifications();

        this.validateLogicalName(this.fields.targetParentField, true, errors);
        this.validateLogicalName(this.fields.sourceChildField, rule.sourceRequired, errors);
        this.validateSeparator(rule.separatorRequired, errors);
        this.validateKnownMissingFields(errors);

        return errors;

    },

    validateLogicalName: function (fieldName, mustValidate, errors) {

        if (!mustValidate) {
            return;
        }

        var value = this.getValue(fieldName);

        if (typeof value !== "string" || !this.logicalNamePattern.test(value)) {
            this.addError(fieldName, this.t("FORM_LOGICAL_NAME_INVALID"), errors);
        }

    },

    validateSeparator: function (mustValidate, errors) {

        if (!mustValidate) {
            return;
        }

        var value = this.getValue(this.fields.separator);

        if (typeof value !== "string" || value.length === 0) {
            this.addError(this.fields.separator, this.t("FORM_SEPARATOR_REQUIRED"), errors);
        }

    },

    validateKnownMissingFields: function (errors) {

        var self = this;
        var checks = this.getExistenceChecks();

        Object.keys(checks).forEach(function (fieldName) {
            var verified = self.verifiedFields[fieldName];

            if (verified && verified.value === self.getValue(fieldName) && !verified.exists) {
                self.addError(fieldName, self.missingFieldMessage(fieldName), errors);
            }
        });

    },

    missingFieldMessage: function (fieldName) {

        return fieldName === this.fields.targetParentField
            ? this.t("FORM_FIELD_NOT_ON_PARENT")
            : this.t("FORM_FIELD_NOT_ON_CHILD");

    },

    getExistenceChecks: function () {

        var checks = {};
        var rule = this.getRule(this.getFunctionType());

        checks[this.fields.targetParentField] = "parentEntity";

        if (rule.sourceRequired) {
            checks[this.fields.sourceChildField] = "childEntity";
        }

        return checks;

    },

    getUnverifiedFields: function () {

        var self = this;
        var checks = this.getExistenceChecks();

        if (typeof Xrm === "undefined") {
            return [];
        }

        return Object.keys(checks).filter(function (fieldName) {
            var verified = self.verifiedFields[fieldName];

            return !verified || verified.value !== self.getValue(fieldName);
        });

    },

    verifyField: function (fieldName) {

        var self = this;
        var entityKey = this.getExistenceChecks()[fieldName];
        var value = this.getValue(fieldName);

        if (typeof Xrm === "undefined" || !entityKey || typeof value !== "string" || !this.logicalNamePattern.test(value)) {
            return Promise.resolve(true);
        }

        return this.getEntities().then(function (entities) {

            if (!entities || !entities[entityKey]) {
                return true;
            }

            return self.attributeExists(entities[entityKey], value).then(function (exists) {

                if (self.getValue(fieldName) === value) {
                    self.verifiedFields[fieldName] = { value: value, exists: exists };

                    if (!exists) {
                        self.addError(fieldName, self.missingFieldMessage(fieldName), []);
                    }
                }

                return exists;

            });

        }).catch(function () {
            return true;
        });

    },

    refreshConversion: function (clearWhenHidden) {

        var self = this;
        var rule = this.getRule(this.getFunctionType());
        var target = this.getValue(this.fields.targetParentField);
        var source = this.getValue(this.fields.sourceChildField);

        if (!rule.conversionAllowed || typeof Xrm === "undefined" || !this.isLogicalName(target) || !this.isLogicalName(source)) {
            this.setConversionVisible(false, clearWhenHidden);
            return Promise.resolve(false);
        }

        return this.getEntities().then(function (entities) {

            if (!entities) {
                self.setConversionVisible(false, clearWhenHidden);
                return false;
            }

            return Promise.all([
                self.hasBaseField(entities.parentEntity, target),
                self.hasBaseField(entities.childEntity, source)
            ]).then(function (results) {

                var visible = results[0] && results[1];

                if (self.getValue(self.fields.targetParentField) === target && self.getValue(self.fields.sourceChildField) === source) {
                    self.setConversionVisible(visible, clearWhenHidden);
                }

                return visible;

            });

        }).catch(function () {
            self.setConversionVisible(false, clearWhenHidden);
            return false;
        });

    },

    preloadCatalogs: function () {

        var self = this;

        if (typeof Xrm === "undefined") {
            return Promise.resolve();
        }

        return this.getEntities().then(function (entities) {

            if (!entities) {
                return null;
            }

            return Promise.all([self.getCatalog(entities.parentEntity), self.getCatalog(entities.childEntity)]);

        }).catch(function () {
            return null;
        });

    },

    getCatalog: function (entityName) {

        if (!this.catalogs[entityName]) {
            this.catalogs[entityName] = this.loadCatalog(entityName);
        }

        return this.catalogs[entityName];

    },

    loadCatalog: function (entityName) {

        if (typeof fetch !== "function" || typeof Xrm === "undefined") {
            return Promise.resolve(null);
        }

        var url = Xrm.Utility.getGlobalContext().getClientUrl()
            + "/api/data/v9.2/EntityDefinitions(LogicalName='" + encodeURIComponent(entityName) + "')"
            + "/Attributes?$select=LogicalName,AttributeType,SourceType,IsValidForUpdate,DisplayName";

        return fetch(url, { headers: { "Accept": "application/json", "OData-Version": "4.0" }, credentials: "same-origin" })
            .then(function (response) {
                return response.ok ? response.json() : null;
            })
            .then(function (json) {

                if (!json || !json.value) {
                    return null;
                }

                var catalog = {};

                json.value.forEach(function (attribute) {
                    catalog[attribute.LogicalName] = {
                        type: attribute.AttributeType,
                        sourceType: attribute.SourceType,
                        isValidForUpdate: attribute.IsValidForUpdate,
                        label: attribute.DisplayName && attribute.DisplayName.UserLocalizedLabel
                            ? attribute.DisplayName.UserLocalizedLabel.Label
                            : ""
                    };
                });

                return catalog;

            })
            .catch(function () {
                return null;
            });

    },

    hasBaseField: function (entityName, fieldName) {

        var self = this;

        return this.getCatalog(entityName).then(function (catalog) {

            if (catalog) {
                return !!catalog[fieldName] && catalog[fieldName].type === "Money" && !!catalog[fieldName + "_base"];
            }

            return self.attributeExists(entityName, fieldName + "_base");

        });

    },

    attributeExists: function (entityName, fieldName) {

        var self = this;

        return this.getCatalog(entityName).then(function (catalog) {

            if (catalog) {
                return !!catalog[fieldName];
            }

            return self.attributeExistsInMetadata(entityName, fieldName);

        });

    },

    attributeExistsInMetadata: function (entityName, fieldName) {

        var self = this;

        return Xrm.Utility.getEntityMetadata(entityName, [fieldName]).then(function (metadata) {

            if (metadata.Attributes.getLength() > 0) {
                return true;
            }

            return self.attributeExistsOnServer(entityName, fieldName);

        });

    },

    attributeExistsOnServer: function (entityName, fieldName) {

        if (typeof fetch !== "function") {
            return Promise.resolve(false);
        }

        var url = Xrm.Utility.getGlobalContext().getClientUrl()
            + "/api/data/v9.2/EntityDefinitions(LogicalName='" + encodeURIComponent(entityName) + "')"
            + "/Attributes(LogicalName='" + encodeURIComponent(fieldName) + "')?$select=LogicalName";

        return fetch(url, { headers: { "Accept": "application/json", "OData-Version": "4.0" }, credentials: "same-origin" })
            .then(function (response) {
                return response.status !== 404;
            })
            .catch(function () {
                return true;
            });

    },

    setConversionVisible: function (visible, clearWhenHidden) {

        var attribute = this.formContext.getAttribute(this.fields.convertCurrency);

        this.getControls(this.fields.convertCurrency).forEach(function (control) {
            control.setVisible(visible);
        });

        if (attribute && !visible && clearWhenHidden && attribute.getValue() === true) {
            attribute.setValue(false);
        }

    },

    isLogicalName: function (value) {

        return typeof value === "string" && this.logicalNamePattern.test(value);

    },

    getEntities: function () {

        var self = this;

        if (!this.entitiesPromise) {
            this.entitiesPromise = this.getConfigurationId().then(function (configurationId) {

                if (!configurationId) {
                    return null;
                }

                return Xrm.WebApi.retrieveRecord("ksga_rollupconfiguration", configurationId, "?$select=ksga_parententity,ksga_childentity").then(function (configuration) {
                    return {
                        parentEntity: configuration.ksga_parententity,
                        childEntity: configuration.ksga_childentity
                    };
                });

            }).catch(function () {
                self.entitiesPromise = null;
                return null;
            });
        }

        return this.entitiesPromise;

    },

    getConfigurationId: function () {

        var formType = this.formContext.ui.getFormType();

        if (formType === 1) {
            var parameters = this.formContext.context.getQueryStringParameters();

            if (parameters.parentrecordtype === "ksga_rollupconfiguration" && parameters.parentrecordid) {
                return Promise.resolve(parameters.parentrecordid);
            }

            return Promise.resolve(null);
        }

        var functionId = this.formContext.data.entity.getId().replace(/[{}]/g, "");

        return Xrm.WebApi.retrieveRecord("ksga_rollupfunction", functionId, "?$select=_ksga_rollupconfigurationid_value").then(function (record) {
            return record._ksga_rollupconfigurationid_value || null;
        });

    },

    applyUI: function (clearHiddenValues) {

        var rule = this.getRule(this.getFunctionType());

        this.setFieldState(this.fields.targetParentField, true, true, false);
        this.setFieldState(this.fields.sourceChildField, rule.sourceVisible, rule.sourceRequired, clearHiddenValues);
        this.setFieldState(this.fields.separator, rule.separatorVisible, rule.separatorRequired, clearHiddenValues);

    },

    getRule: function (functionType) {

        var types = this.functionTypes;

        var aggregatesSourceField = [types.Sum, types.Average, types.Minimum, types.Maximum, types.And, types.Or];

        var convertibleFunctions = [types.Sum, types.Average, types.Minimum, types.Maximum];

        if (aggregatesSourceField.indexOf(functionType) !== -1) {
            return {
                sourceVisible: true, sourceRequired: true, separatorVisible: false, separatorRequired: false,
                conversionAllowed: convertibleFunctions.indexOf(functionType) !== -1
            };
        }

        if (functionType === types.Concat) {
            return { sourceVisible: true, sourceRequired: true, separatorVisible: true, separatorRequired: true, conversionAllowed: false };
        }

        return { sourceVisible: false, sourceRequired: false, separatorVisible: false, separatorRequired: false, conversionAllowed: false };

    },

    getFunctionType: function () {

        return this.getValue(this.fields.functionType);

    },

    getValue: function (fieldName) {

        var attribute = this.formContext.getAttribute(fieldName);

        return attribute ? attribute.getValue() : null;

    },

    setFieldState: function (fieldName, visible, required, clearWhenHidden) {

        var attribute = this.formContext.getAttribute(fieldName);

        this.getControls(fieldName).forEach(function (control) {
            control.setVisible(visible);
        });

        if (attribute) {
            attribute.setRequiredLevel(required ? "required" : "none");

            if (!visible && clearWhenHidden) {
                attribute.setValue(null);
            }
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

    clearNotifications: function () {

        this.clearNotification(this.fields.targetParentField);
        this.clearNotification(this.fields.sourceChildField);
        this.clearNotification(this.fields.separator);

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
