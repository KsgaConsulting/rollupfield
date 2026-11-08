var Ksga = Ksga || {};

Ksga.RollupTransfer = {

    formatName: "ksga-rollup-export",
    formatVersion: 1,

    pageName: "ksga_RollupTransfer.html",
    apiPath: "/api/data/v9.2/",
    configSet: "ksga_rollupconfigurations",
    functionSet: "ksga_rollupfunctions",

    configFields: [
        "ksga_name", "ksga_relations", "ksga_parententity", "ksga_childentity", "ksga_lookuppath",
        "ksga_childfilter", "ksga_parentexecutionfilter", "ksga_triggeroncreate", "ksga_triggerondelete",
        "ksga_additionaltriggeronupdate"
    ],

    immutableConfigFields: ["ksga_relations", "ksga_parententity", "ksga_childentity", "ksga_lookuppath"],

    editableConfigFields: [
        "ksga_name", "ksga_childfilter", "ksga_parentexecutionfilter", "ksga_triggeroncreate", "ksga_triggerondelete",
        "ksga_additionaltriggeronupdate"
    ],

    functionFields: [
        "ksga_functiontypecode", "ksga_sourcechildfield", "ksga_targetparentfield", "ksga_separator", "ksga_convertcurrency"
    ],

    booleanFields: ["ksga_triggeroncreate", "ksga_triggerondelete", "ksga_convertcurrency"],

    stringConfigFields: [
        "ksga_name", "ksga_relations", "ksga_parententity", "ksga_childentity", "ksga_lookuppath",
        "ksga_childfilter", "ksga_parentexecutionfilter", "ksga_additionaltriggeronupdate"
    ],

    stringFunctionFields: ["ksga_sourcechildfield", "ksga_targetparentfield", "ksga_separator"],

    typesWithSource: [0, 1, 2, 3, 7, 8, 9],
    concatType: 9,
    maxFunctionType: 10,

    logicalNamePattern: /^[a-z_][a-z0-9_]*$/,
    relationNamePattern: /^[A-Za-z_][A-Za-z0-9_]*$/,
    guidPattern: /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/,

    commandFallbacks: {
        CMD_SELECT_AT_LEAST_ONE: "Select at least one configuration to export.",
        CMD_TITLE_EXPORT: "Export configurations",
        CMD_TITLE_IMPORT: "Import configurations"
    },

    t: function (code, args) {

        if (typeof Ksga.Localization !== "undefined" && typeof Ksga.Localization.t === "function") {
            return Ksga.Localization.t(code, args);
        }

        return this.commandFallbacks[code] || code;

    },

    normalizeId: function (value) {

        return typeof value === "string" ? value.replace(/[{}]/g, "").trim().toLowerCase() : "";

    },

    isGuid: function (value) {

        return this.guidPattern.test(this.normalizeId(value));

    },

    normalizeValue: function (value) {

        if (value === undefined || value === null) {
            return null;
        }

        if (typeof value === "string") {
            var trimmed = value.trim();

            return trimmed.length === 0 ? null : trimmed;
        }

        return value;

    },

    splitNames: function (value) {

        return (typeof value === "string" ? value : "")
            .split(/[,;\s]+/)
            .filter(function (name) { return name.length > 0; });

    },

    normalizeNameList: function (value) {

        var names = this.splitNames(value);
        var unique = [];

        names.forEach(function (name) {
            if (unique.indexOf(name) === -1) {
                unique.push(name);
            }
        });

        unique.sort();

        return unique.length === 0 ? null : unique.join(",");

    },

    comparable: function (field, value) {

        if (field === "ksga_additionaltriggeronupdate") {
            return this.normalizeNameList(value);
        }

        if (this.booleanFields.indexOf(field) !== -1) {
            return value === true;
        }

        return this.normalizeValue(value);

    },

    diffFields: function (fields, wanted, current) {

        var self = this;

        return fields.filter(function (field) {
            return self.comparable(field, wanted[field]) !== self.comparable(field, current ? current[field] : null);
        });

    },

    requiresSource: function (typeCode) {

        return this.typesWithSource.indexOf(typeCode) !== -1;

    },

    buildExport: function (entries, environment) {

        var self = this;

        return {
            format: this.formatName,
            version: this.formatVersion,
            exportedOn: new Date().toISOString(),
            environment: environment || null,
            configs: entries.map(function (entry) {
                var config = { id: self.normalizeId(entry.config.ksga_rollupconfigurationid) };

                self.configFields.forEach(function (field) {
                    config[field] = self.normalizeValue(entry.config[field]);
                });

                config.statecode = entry.config.statecode === 1 ? 1 : 0;

                config.functions = entry.functions.map(function (source) {
                    var fn = { id: self.normalizeId(source.ksga_rollupfunctionid) };

                    self.functionFields.forEach(function (field) {
                        fn[field] = self.normalizeValue(source[field]);
                    });

                    return fn;
                });

                return config;
            })
        };

    },

    parsePayload: function (text) {

        var self = this;
        var errors = [];
        var data;

        if (typeof text !== "string" || text.trim().length === 0) {
            return { payload: null, errors: [this.t("PARSE_EMPTY")] };
        }

        try {
            data = JSON.parse(text);
        } catch (error) {
            return { payload: null, errors: [this.t("PARSE_JSON_INVALID", [error.message])] };
        }

        if (!data || typeof data !== "object" || data.format !== this.formatName) {
            return { payload: null, errors: [this.t("PARSE_NOT_EXPORT", [this.formatName])] };
        }

        if (data.version !== this.formatVersion) {
            return { payload: null, errors: [this.t("PARSE_VERSION", [data.version, this.formatVersion])] };
        }

        if (!Array.isArray(data.configs) || data.configs.length === 0) {
            return { payload: null, errors: [this.t("PARSE_NO_CONFIGS")] };
        }

        var configIds = {};
        var functionIds = {};

        var configs = data.configs.map(function (source, index) {
            return self.parseConfig(source, "configs[" + index + "]", configIds, functionIds, errors);
        });

        return {
            payload: errors.length === 0 ? { environment: data.environment || null, exportedOn: data.exportedOn || null, configs: configs } : null,
            errors: errors
        };

    },

    parseConfig: function (source, path, configIds, functionIds, errors) {

        var self = this;
        var config = {};

        if (!source || typeof source !== "object") {
            errors.push(this.t("PARSE_OBJECT_EXPECTED", [path]));
            return config;
        }

        config.id = this.normalizeId(source.id);

        if (!this.isGuid(source.id)) {
            errors.push(this.t("PARSE_GUID_INVALID", [path, source.id]));
        } else if (configIds[config.id]) {
            errors.push(this.t("PARSE_GUID_DUPLICATE", [path, config.id]));
        } else {
            configIds[config.id] = true;
        }

        this.configFields.forEach(function (field) {
            var value = source[field];

            if (value !== undefined && value !== null && self.booleanFields.indexOf(field) === -1 && typeof value !== "string") {
                errors.push(self.t("PARSE_TEXT_EXPECTED", [path, field]));
            }

            if (self.booleanFields.indexOf(field) !== -1 && value !== undefined && value !== null && typeof value !== "boolean") {
                errors.push(self.t("PARSE_BOOLEAN_EXPECTED", [path, field]));
            }

            config[field] = self.booleanFields.indexOf(field) !== -1 ? value === true : self.normalizeValue(value);
        });

        if (source.statecode !== undefined && source.statecode !== 0 && source.statecode !== 1) {
            errors.push(this.t("PARSE_STATECODE", [path]));
        }

        config.statecode = source.statecode === 1 ? 1 : 0;

        if (source.functions !== undefined && !Array.isArray(source.functions)) {
            errors.push(this.t("PARSE_ARRAY_EXPECTED", [path]));
            config.functions = [];
        } else {
            config.functions = (source.functions || []).map(function (fn, index) {
                return self.parseFunction(fn, path + ".functions[" + index + "]", functionIds, errors);
            });
        }

        return config;

    },

    parseFunction: function (source, path, functionIds, errors) {

        var self = this;
        var fn = {};

        if (!source || typeof source !== "object") {
            errors.push(this.t("PARSE_OBJECT_EXPECTED", [path]));
            return fn;
        }

        fn.id = this.normalizeId(source.id);

        if (!this.isGuid(source.id)) {
            errors.push(this.t("PARSE_GUID_INVALID", [path, source.id]));
        } else if (functionIds[fn.id]) {
            errors.push(this.t("PARSE_GUID_DUPLICATE", [path, fn.id]));
        } else {
            functionIds[fn.id] = true;
        }

        var type = source.ksga_functiontypecode;

        if (typeof type !== "number" || Math.floor(type) !== type || type < 0 || type > this.maxFunctionType) {
            errors.push(this.t("PARSE_TYPE_RANGE", [path, this.maxFunctionType]));
        }

        fn.ksga_functiontypecode = type;

        this.stringFunctionFields.forEach(function (field) {
            var value = source[field];

            if (value !== undefined && value !== null && typeof value !== "string") {
                errors.push(self.t("PARSE_TEXT_EXPECTED", [path, field]));
            }

            fn[field] = field === "ksga_separator" && typeof value === "string" && value.length > 0
                ? value
                : self.normalizeValue(value);
        });

        if (source.ksga_convertcurrency !== undefined && source.ksga_convertcurrency !== null && typeof source.ksga_convertcurrency !== "boolean") {
            errors.push(this.t("PARSE_BOOLEAN_EXPECTED", [path, "ksga_convertcurrency"]));
        }

        fn.ksga_convertcurrency = source.ksga_convertcurrency === true;

        return fn;

    },

    eachSeries: function (items, action) {

        var chain = Promise.resolve();

        items.forEach(function (item, index) {
            chain = chain.then(function () {
                return action(item, index);
            });
        });

        return chain;

    },

    describeError: function (error) {

        if (!error) {
            return this.t("UNKNOWN_ERROR");
        }

        var text = error.message || String(error);

        if (error.status) {
            text += " (HTTP " + error.status + (error.code ? ", " + error.code : "") + ")";
        }

        return text;

    },

    createApi: function (clientUrl) {

        var self = this;
        var base = clientUrl + this.apiPath;

        function send(method, path, body, headers) {

            var options = {
                method: method,
                credentials: "same-origin",
                headers: Object.assign({ "Accept": "application/json", "OData-Version": "4.0", "OData-MaxVersion": "4.0" }, headers || {})
            };

            if (body !== undefined) {
                options.headers["Content-Type"] = "application/json; charset=utf-8";
                options.body = JSON.stringify(body);
            }

            return fetch(base + path, options).then(function (response) {

                if (response.ok) {
                    return response.status === 204 ? null : response.text().then(function (text) {
                        return text ? JSON.parse(text) : null;
                    });
                }

                return response.text().then(function (text) {

                    var failure = new Error(response.statusText || self.t("HTTP_ERROR"));

                    failure.status = response.status;

                    try {
                        var parsed = JSON.parse(text);

                        failure.message = parsed.error && parsed.error.message ? parsed.error.message : failure.message;
                        failure.code = parsed.error && parsed.error.code ? parsed.error.code : undefined;
                    } catch (parseError) {
                        failure.message = text || failure.message;
                    }

                    failure.notFound = response.status === 404;

                    throw failure;

                });

            });

        }

        return {
            clientUrl: clientUrl,
            get: function (path) {
                return send("GET", path).catch(function (error) {
                    if (error.notFound) {
                        return null;
                    }

                    throw error;
                });
            },
            post: function (set, body) {
                return send("POST", set, body);
            },
            patch: function (set, id, body) {
                return send("PATCH", set + "(" + id + ")", body, { "If-Match": "*" });
            },
            describe: function (error) { return self.describeError(error); }
        };

    },

    loadExportEntries: function (api, ids) {

        var self = this;
        var entries = [];
        var configSelect = ["ksga_rollupconfigurationid"].concat(this.configFields).concat(["statecode"]).join(",");
        var functionSelect = ["ksga_rollupfunctionid"].concat(this.functionFields).join(",");

        return this.eachSeries(ids, function (id) {

            var cleanId = self.normalizeId(id);

            return api.get(self.configSet + "(" + cleanId + ")?$select=" + configSelect).then(function (config) {

                if (!config) {
                    throw new Error(self.t("CONFIG_NOT_FOUND", [cleanId]));
                }

                return api.get(
                    self.functionSet + "?$select=" + functionSelect
                    + "&$filter=_ksga_rollupconfigurationid_value eq " + cleanId
                    + "&$orderby=createdon asc"
                ).then(function (result) {
                    entries.push({ config: config, functions: (result && result.value) || [] });
                });

            });

        }).then(function () {
            return entries;
        });

    },

    loadFieldNames: function (api, entity, cache) {

        if (!entity) {
            return Promise.resolve(null);
        }

        if (!cache.fields[entity]) {
            cache.fields[entity] = api.get("EntityDefinitions(LogicalName='" + entity + "')/Attributes?$select=LogicalName").then(function (result) {

                if (!result || !result.value) {
                    return null;
                }

                var names = {};

                result.value.forEach(function (attribute) {
                    names[attribute.LogicalName] = true;
                });

                return names;

            });
        }

        return cache.fields[entity];

    },

    relationExists: function (api, name, cache) {

        if (!cache.relations[name]) {
            cache.relations[name] = api.get("RelationshipDefinitions?$select=SchemaName&$filter=SchemaName eq '" + name + "'").then(function (result) {
                return !!(result && result.value && result.value.length > 0);
            });
        }

        return cache.relations[name];

    },

    checkFilterXml: function (value) {

        if (typeof DOMParser === "undefined" || !value) {
            return null;
        }

        if (/<!DOCTYPE|<!ENTITY/i.test(value)) {
            return this.t("FILTER_DTD");
        }

        var document = new DOMParser().parseFromString("<fragment>" + value + "</fragment>", "application/xml");

        return document.getElementsByTagName("parsererror").length > 0 ? this.t("FILTER_XML_MALFORMED") : null;

    },

    planImport: function (api, payload) {

        var self = this;
        var cache = { fields: {}, relations: {} };
        var items = [];

        return this.eachSeries(payload.configs, function (config) {
            return self.planConfig(api, config, cache).then(function (item) {
                items.push(item);
            });
        }).then(function () {
            return { configs: items, summary: self.summarize(items) };
        });

    },

    configLabel: function (config) {

        return config.ksga_name || config.ksga_relations || config.id;

    },

    planConfig: function (api, wanted, cache) {

        var self = this;
        var select = ["ksga_rollupconfigurationid"].concat(this.configFields).concat(["statecode"]).join(",");
        var item = {
            kind: "config", id: wanted.id, label: this.configLabel(wanted), wanted: wanted, current: null, exists: false,
            changes: [], stateChange: false, errors: [], warnings: [], functions: [], action: "none"
        };

        return api.get(this.configSet + "(" + wanted.id + ")?$select=" + select).then(function (current) {

            item.current = current;
            item.exists = !!current;

            return self.checkConfig(api, item, cache);

        }).then(function () {

            return self.eachSeries(wanted.functions, function (fn) {
                return self.planFunction(api, fn, item, cache).then(function (functionItem) {
                    item.functions.push(functionItem);
                });
            });

        }).then(function () {

            if (item.errors.length > 0) {
                item.action = "error";
            } else if (!item.exists) {
                item.action = "create";
            } else {
                item.action = item.changes.length > 0 || item.stateChange ? "update" : "none";
            }

            if (wanted.functions.length === 0) {
                item.warnings.push(self.t("NO_FUNCTIONS_WARNING"));
            }

            return item;

        }).catch(function (error) {

            item.errors.push(self.t("ANALYSIS_FAILED", [self.describeError(error)]));
            item.action = "error";

            return item;

        });

    },

    checkConfig: function (api, item, cache) {

        var self = this;
        var wanted = item.wanted;
        var current = item.current;
        var checks = Promise.resolve();

        if (current) {
            this.immutableConfigFields.forEach(function (field) {
                if (self.comparable(field, wanted[field]) !== self.comparable(field, current[field])) {
                    item.errors.push(self.t("FIELD_IMMUTABLE", [
                        field,
                        self.normalizeValue(current[field]) || self.t("EMPTY_VALUE"),
                        self.normalizeValue(wanted[field]) || self.t("EMPTY_VALUE")
                    ]));
                }
            });

            item.changes = this.diffFields(this.editableConfigFields, wanted, current);
            item.stateChange = current.statecode !== wanted.statecode;
        } else {
            item.stateChange = wanted.statecode !== 0;

            if (wanted.ksga_relations) {
                var names = this.splitNames(wanted.ksga_relations.replace(/[,;]/g, " "));

                names.forEach(function (name) {
                    if (!self.relationNamePattern.test(name)) {
                        item.errors.push(self.t("RELATION_NAME_INVALID", [name]));
                    }
                });

                names.filter(function (name) { return self.relationNamePattern.test(name); }).forEach(function (name) {
                    checks = checks.then(function () {
                        return self.relationExists(api, name, cache).then(function (exists) {
                            if (!exists) {
                                item.errors.push(self.t("RELATION_MISSING", [name]));
                            }
                        });
                    });
                });
            } else if (!wanted.ksga_parententity || !wanted.ksga_childentity || !wanted.ksga_lookuppath) {
                item.errors.push(self.t("PATH_REQUIRED"));
            }
        }

        var parent = (current && current.ksga_parententity) || wanted.ksga_parententity;
        var child = (current && current.ksga_childentity) || wanted.ksga_childentity;

        item.parentEntity = parent;
        item.childEntity = child;

        return checks.then(function () {

            return Promise.all([self.loadFieldNames(api, parent, cache), self.loadFieldNames(api, child, cache)]);

        }).then(function (catalogs) {

            item.parentFields = catalogs[0];
            item.childFields = catalogs[1];

            if (parent && !catalogs[0]) {
                item.errors.push(self.t("PARENT_ENTITY_MISSING", [parent]));
            }

            if (child && !catalogs[1]) {
                item.errors.push(self.t("CHILD_ENTITY_MISSING", [child]));
            }

            self.checkAdditionalFields(item, catalogs[1]);

            ["ksga_childfilter", "ksga_parentexecutionfilter"].forEach(function (field) {
                var problem = self.checkFilterXml(wanted[field]);

                if (problem) {
                    item.errors.push(self.t("FILTER_PROBLEM", [field, problem]));
                }
            });

        });

    },

    checkAdditionalFields: function (item, childFields) {

        var self = this;
        var names = this.splitNames(item.wanted.ksga_additionaltriggeronupdate);

        names.forEach(function (name) {
            if (!self.logicalNamePattern.test(name)) {
                item.errors.push(self.t("ADDITIONAL_FIELD_INVALID", [name]));
            } else if (childFields && !childFields[name]) {
                item.errors.push(self.t("ADDITIONAL_FIELD_MISSING", [item.childEntity, name]));
            }
        });

    },

    planFunction: function (api, wanted, configItem, cache) {

        var self = this;
        var select = ["ksga_rollupfunctionid", "_ksga_rollupconfigurationid_value"].concat(this.functionFields).join(",");
        var item = {
            kind: "function", id: wanted.id, configId: configItem.id, wanted: wanted, current: null, exists: false,
            changes: [], errors: [], warnings: [], action: "none",
            label: (wanted.ksga_targetparentfield || "?") + " ← " + self.typeLabel(wanted.ksga_functiontypecode)
                + (wanted.ksga_sourcechildfield ? "(" + wanted.ksga_sourcechildfield + ")" : "")
        };

        return api.get(this.functionSet + "(" + wanted.id + ")?$select=" + select).then(function (current) {

            item.current = current;
            item.exists = !!current;

            if (current) {
                var owner = self.normalizeId(current._ksga_rollupconfigurationid_value);

                if (owner !== configItem.id) {
                    item.errors.push(self.t("FUNCTION_OTHER_CONFIG", [owner]));
                }

                item.changes = self.diffFields(self.functionFields, wanted, current);
            }

            self.checkFunction(item, configItem);

            if (configItem.errors.length > 0) {
                item.warnings.push(self.t("FUNCTION_CONFIG_ERROR"));
            }

            if (item.errors.length > 0) {
                item.action = "error";
            } else if (!item.exists) {
                item.action = "create";
            } else {
                item.action = item.changes.length > 0 ? "update" : "none";
            }

            return item;

        }).catch(function (error) {

            item.errors.push(self.t("ANALYSIS_FAILED", [self.describeError(error)]));
            item.action = "error";

            return item;

        });

    },

    typeLabel: function (typeCode) {

        var labels = ["Sum", "Average", "Minimum", "Maximum", "Count", "Any", "None", "And", "Or", "Concat", "Refresh"];

        return labels[typeCode] || String(typeCode);

    },

    checkFunction: function (item, configItem) {

        var wanted = item.wanted;
        var type = wanted.ksga_functiontypecode;
        var target = wanted.ksga_targetparentfield;
        var source = wanted.ksga_sourcechildfield;

        if (!target) {
            item.errors.push(this.t("TARGET_REQUIRED"));
        } else if (!this.logicalNamePattern.test(target)) {
            item.errors.push(this.t("TARGET_INVALID", [target]));
        } else if (configItem.parentFields && !configItem.parentFields[target]) {
            item.errors.push(this.t("TARGET_MISSING", [target, configItem.parentEntity]));
        }

        if (this.requiresSource(type)) {
            if (!source) {
                item.errors.push(this.t("SOURCE_REQUIRED", [this.typeLabel(type)]));
            } else if (!this.logicalNamePattern.test(source)) {
                item.errors.push(this.t("SOURCE_INVALID", [source]));
            } else if (configItem.childFields && !configItem.childFields[source]) {
                item.errors.push(this.t("SOURCE_MISSING", [source, configItem.childEntity]));
            }
        } else if (source) {
            item.warnings.push(this.t("SOURCE_IGNORED", [this.typeLabel(type)]));
        }

        if (type === this.concatType && !wanted.ksga_separator) {
            item.errors.push(this.t("SEPARATOR_REQUIRED"));
        }

    },

    summarize: function (configs) {

        var summary = { configs: { create: 0, update: 0, none: 0, error: 0 }, functions: { create: 0, update: 0, none: 0, error: 0 } };

        configs.forEach(function (config) {
            summary.configs[config.action] += 1;

            config.functions.forEach(function (fn) {
                summary.functions[fn.action] += 1;
            });
        });

        return summary;

    },

    changeBody: function (fields, wanted) {

        var body = {};
        var self = this;

        fields.forEach(function (field) {
            var value = wanted[field];

            if (self.booleanFields.indexOf(field) !== -1) {
                body[field] = value === true;
            } else {
                body[field] = field === "ksga_separator" ? (value === undefined ? null : value) : self.normalizeValue(value);
            }
        });

        return body;

    },

    execute: function (api, plan, log) {

        var self = this;
        var counts = { created: 0, updated: 0, unchanged: 0, failed: 0, skipped: 0 };
        var failedConfigs = {};
        var touchedConfigs = {};
        var stateTargets = [];

        function fail(text) {
            counts.failed += 1;
            log("error", text);
        }

        function configStep(item) {

            var label = self.t("LABEL_CONFIG", [item.label]);

            if (item.action === "error") {
                counts.skipped += 1;
                log("warn", self.t("LOG_SKIPPED_ANALYSIS", [label]));
                failedConfigs[item.id] = true;
                return Promise.resolve();
            }

            if (!item.exists) {
                return createConfig(item, label);
            }

            return updateConfig(item, label, item.current);

        }

        function createConfig(item, label) {

            var wanted = item.wanted;
            var body = { ksga_rollupconfigurationid: item.id };

            if (wanted.ksga_name) {
                body.ksga_name = wanted.ksga_name;
            }

            if (wanted.ksga_relations) {
                body.ksga_relations = wanted.ksga_relations;
            } else {
                body.ksga_parententity = wanted.ksga_parententity;
                body.ksga_childentity = wanted.ksga_childentity;
                body.ksga_lookuppath = wanted.ksga_lookuppath;
            }

            return api.post(self.configSet, body).then(function () {

                counts.created += 1;
                touchedConfigs[item.id] = true;
                log("ok", self.t("LOG_CREATED", [label, item.id]));

                return api.get(self.configSet + "(" + item.id + ")?$select=ksga_rollupconfigurationid," + self.configFields.join(",") + ",statecode");

            }).then(function (created) {

                item.current = created;

                if (item.stateChange || wanted.statecode !== 0) {
                    stateTargets.push(item);
                }

                return updateConfig(item, label, created, true);

            }).catch(function (error) {

                failedConfigs[item.id] = true;
                fail(self.t("LOG_CREATE_FAILED", [label, self.describeError(error)]));

            });

        }

        function updateConfig(item, label, current, afterCreate) {

            var fields = self.diffFields(self.editableConfigFields, item.wanted, current);

            if (!afterCreate && item.stateChange) {
                stateTargets.push(item);
            }

            if (fields.length === 0) {

                if (!afterCreate) {
                    counts.unchanged += item.stateChange ? 0 : 1;
                    log("info", item.stateChange ? self.t("LOG_NO_FIELD_STATE", [label]) : self.t("LOG_IDENTICAL", [label]));
                }

                return Promise.resolve();
            }

            return api.patch(self.configSet, item.id, self.changeBody(fields, item.wanted)).then(function () {

                if (!afterCreate) {
                    counts.updated += 1;
                    touchedConfigs[item.id] = true;
                }

                log("ok", self.t(afterCreate ? "LOG_COMPLETED" : "LOG_MODIFIED", [label, fields.join(", ")]));

            }).catch(function (error) {

                failedConfigs[item.id] = true;
                fail(self.t("LOG_UPDATE_FAILED", [label, fields.join(", "), self.describeError(error)]));

            });

        }

        function functionStep(configItem, item) {

            var label = self.t("LABEL_FUNCTION", [item.label, configItem.label]);

            if (failedConfigs[configItem.id]) {
                counts.skipped += 1;
                log("warn", self.t("LOG_SKIPPED_CONFIG", [label]));
                return Promise.resolve();
            }

            if (item.action === "error") {
                counts.skipped += 1;
                log("warn", self.t("LOG_SKIPPED_ANALYSIS", [label]));
                return Promise.resolve();
            }

            var wanted = item.wanted;

            if (!item.exists) {

                var body = { ksga_rollupfunctionid: item.id };

                body["ksga_RollupConfigurationId@odata.bind"] = "/" + self.configSet + "(" + configItem.id + ")";
                body.ksga_functiontypecode = wanted.ksga_functiontypecode;
                body.ksga_targetparentfield = wanted.ksga_targetparentfield;

                if (wanted.ksga_sourcechildfield) {
                    body.ksga_sourcechildfield = wanted.ksga_sourcechildfield;
                }

                if (wanted.ksga_separator) {
                    body.ksga_separator = wanted.ksga_separator;
                }

                if (wanted.ksga_convertcurrency) {
                    body.ksga_convertcurrency = true;
                }

                return api.post(self.functionSet, body).then(function () {
                    counts.created += 1;
                    log("ok", self.t("LOG_CREATED", [label, item.id]));
                }).catch(function (error) {
                    fail(self.t("LOG_CREATE_FAILED", [label, self.describeError(error)]));
                });

            }

            var fields = self.diffFields(self.functionFields, wanted, item.current);

            if (fields.length === 0) {
                counts.unchanged += 1;
                log("info", self.t("LOG_IDENTICAL", [label]));
                return Promise.resolve();
            }

            return api.patch(self.functionSet, item.id, self.changeBody(fields, wanted)).then(function () {
                counts.updated += 1;
                log("ok", self.t("LOG_MODIFIED", [label, fields.join(", ")]));
            }).catch(function (error) {
                fail(self.t("LOG_UPDATE_FAILED", [label, fields.join(", "), self.describeError(error)]));
            });

        }

        function stateStep(item) {

            var label = self.t("LABEL_CONFIG", [item.label]);

            if (failedConfigs[item.id]) {
                return Promise.resolve();
            }

            var wantedState = item.wanted.statecode;

            return api.get(self.configSet + "(" + item.id + ")?$select=statecode").then(function (current) {

                if (current && current.statecode === wantedState) {
                    return null;
                }

                return api.patch(self.configSet, item.id, { statecode: wantedState, statuscode: wantedState === 0 ? 1 : 2 }).then(function () {

                    if (!touchedConfigs[item.id]) {
                        counts.updated += 1;
                    }

                    log("ok", self.t(wantedState === 0 ? "LOG_ACTIVATED" : "LOG_DEACTIVATED", [label]));

                });

            }).catch(function (error) {
                fail(self.t("LOG_STATE_FAILED", [label, self.describeError(error)]));
            });

        }

        log("info", self.t("LOG_STEP1"));

        return this.eachSeries(plan.configs, configStep).then(function () {

            log("info", self.t("LOG_STEP2"));

            return self.eachSeries(plan.configs, function (configItem) {
                return self.eachSeries(configItem.functions, function (item) {
                    return functionStep(configItem, item);
                });
            });

        }).then(function () {

            if (stateTargets.length > 0) {
                log("info", self.t("LOG_STEP3"));
            }

            return self.eachSeries(stateTargets, stateStep);

        }).then(function () {

            log(counts.failed > 0 ? "error" : counts.skipped > 0 ? "warn" : "ok",
                self.t("LOG_DONE", [counts.created, counts.updated, counts.unchanged, counts.skipped, counts.failed]));

            return counts;

        });

    },

    getXrm: function () {

        var candidates = [];

        try { candidates.push(window.Xrm); } catch (error) { }
        try { candidates.push(window.parent && window.parent.Xrm); } catch (error) { }
        try { candidates.push(window.top && window.top.Xrm); } catch (error) { }

        for (var i = 0; i < candidates.length; i++) {
            if (candidates[i] && candidates[i].Utility) {
                return candidates[i];
            }
        }

        return null;

    },

    Commands: {

        openExport: function (selectedIds) {

            var ids = (selectedIds || []).map(function (id) { return Ksga.RollupTransfer.normalizeId(id); }).filter(function (id) { return id.length > 0; });

            if (ids.length === 0) {
                Ksga.RollupTransfer.alertCommand(Ksga.RollupTransfer.t("CMD_SELECT_AT_LEAST_ONE"));
                return;
            }

            Ksga.RollupTransfer.openPage("mode=export&ids=" + ids.join(","), Ksga.RollupTransfer.t("CMD_TITLE_EXPORT"));

        },

        openImport: function () {

            Ksga.RollupTransfer.openPage("mode=import", Ksga.RollupTransfer.t("CMD_TITLE_IMPORT"));

        }

    },

    alertCommand: function (text) {

        var xrm = this.getXrm();

        if (xrm && xrm.Navigation && xrm.Navigation.openAlertDialog) {
            xrm.Navigation.openAlertDialog({ text: text });
        }

    },

    openPage: function (data, title) {

        var xrm = this.getXrm();

        if (!xrm) {
            return;
        }

        xrm.Navigation.navigateTo(
            { pageType: "webresource", webresourceName: this.pageName, data: data },
            { target: 2, position: 1, width: { value: 80, unit: "%" }, height: { value: 85, unit: "%" }, title: title });

    }

};
