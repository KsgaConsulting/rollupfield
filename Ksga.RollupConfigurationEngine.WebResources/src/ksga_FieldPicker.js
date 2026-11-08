var Ksga = Ksga || {};

Ksga.FieldPicker = {

    registrations: {},
    catalogs: {},
    listenersInstalled: false,
    panel: null,
    state: null,
    selecting: false,
    hostDocument: null,

    maxResults: 12,
    numericTypes: ["Integer", "BigInt", "Decimal", "Double", "Money"],
    excludedTypes: ["Virtual", "EntityName", "ManagedProperty"],

    register: function (formContext, fieldName, options) {

        this.registrations[fieldName] = {
            formContext: formContext,
            multi: !!options.multi,
            getEntries: options.getEntries
        };

        this.installListeners();

    },

    entriesFromCatalog: function (catalog, accept) {

        var entries = [];

        Object.keys(catalog || {}).forEach(function (name) {
            var field = catalog[name];

            if (Ksga.FieldPicker.excludedTypes.indexOf(field.type) !== -1) {
                return;
            }

            if (accept && !accept(field, name)) {
                return;
            }

            entries.push({ name: name, label: field.label || "", type: field.type || "" });
        });

        entries.sort(function (a, b) {
            return (a.label || a.name).toLowerCase() < (b.label || b.name).toLowerCase() ? -1 : 1;
        });

        return entries;

    },

    typesForFunction: function (functionType, role) {

        var numeric = this.numericTypes;
        var types = { Sum: 0, Average: 1, Minimum: 2, Maximum: 3, Count: 4, Any: 5, None: 6, And: 7, Or: 8, Concat: 9, Refresh: 10 };

        switch (functionType) {
            case types.Sum:
            case types.Average:
                return numeric;
            case types.Minimum:
            case types.Maximum:
                return numeric.concat(["DateTime"]);
            case types.Count:
                return role === "target" ? numeric : null;
            case types.Any:
            case types.None:
                return role === "target" ? ["Boolean"] : null;
            case types.And:
            case types.Or:
                return ["Boolean"];
            case types.Concat:
                return ["String", "Memo"];
            default:
                return null;
        }

    },

    currentToken: function (value, multi) {

        var text = typeof value === "string" ? value : "";

        if (!multi) {
            return text.trim();
        }

        var parts = text.split(/[,;\s]+/);

        return parts[parts.length - 1];

    },

    search: function (entries, token, excluded) {

        var needle = (token || "").toLowerCase();
        var skip = excluded || [];

        return (entries || []).filter(function (entry) {
            if (skip.indexOf(entry.name) !== -1) {
                return false;
            }

            return needle.length === 0
                || entry.name.toLowerCase().indexOf(needle) !== -1
                || entry.label.toLowerCase().indexOf(needle) !== -1;
        }).sort(function (a, b) {
            var aStarts = a.name.toLowerCase().indexOf(needle) === 0 || a.label.toLowerCase().indexOf(needle) === 0 ? 0 : 1;
            var bStarts = b.name.toLowerCase().indexOf(needle) === 0 || b.label.toLowerCase().indexOf(needle) === 0 ? 0 : 1;

            return aStarts - bStarts;
        }).slice(0, this.maxResults);

    },

    applySelection: function (value, name, multi) {

        if (!multi) {
            return name;
        }

        var text = typeof value === "string" ? value : "";
        var cut = Math.max(text.lastIndexOf(","), text.lastIndexOf(";"), text.lastIndexOf(" "));
        var head = cut >= 0 ? text.substring(0, cut + 1) : "";

        return head + name;

    },

    selectedNames: function (value, multi) {

        if (!multi || typeof value !== "string") {
            return [];
        }

        var parts = value.split(/[,;\s]+/).filter(function (part) { return part.length > 0; });

        parts.pop();

        return parts;

    },

    doc: function () {

        if (this.hostDocument) {
            return this.hostDocument;
        }

        var host = null;

        try {
            if (typeof window !== "undefined" && window.parent && window.parent !== window
                && window.parent.document && window.parent.document.body) {
                host = window.parent.document;
            }
        } catch (error) {
            host = null;
        }

        if (!host && typeof document !== "undefined") {
            host = document;
        }

        this.hostDocument = host;

        return host;

    },

    installListeners: function () {

        var self = this;
        var doc = this.doc();

        if (this.listenersInstalled || !doc) {
            return;
        }

        this.listenersInstalled = true;

        var hostWindow = doc.defaultView;

        if (hostWindow && typeof hostWindow.ksgaFieldPickerCleanup === "function") {
            try {
                hostWindow.ksgaFieldPickerCleanup();
            } catch (error) {
            }
        }

        var handlers = [
            ["focusin", function (event) { self.onInput(event); }],
            ["input", function (event) { self.onInput(event); }],
            ["keydown", function (event) { self.onKeyDown(event); }],
            ["focusout", function (event) {
                if (self.fieldNameOf(event.target)) {
                    self.close();
                }
            }],
            ["scroll", function () { self.close(); }],
            ["mousedown", function (event) { self.onMouseDown(event); }]
        ];

        handlers.forEach(function (handler) {
            doc.addEventListener(handler[0], handler[1], true);
        });

        if (hostWindow) {
            hostWindow.ksgaFieldPickerCleanup = function () {
                handlers.forEach(function (handler) {
                    doc.removeEventListener(handler[0], handler[1], true);
                });

                if (self.panel && self.panel.parentNode) {
                    self.panel.parentNode.removeChild(self.panel);
                }

                self.panel = null;
                self.state = null;
            };
        }

    },

    fieldNameOf: function (element) {

        if (!element || typeof element.getAttribute !== "function") {
            return null;
        }

        var id = element.getAttribute("data-id") || "";
        var match = /^(.+)\.fieldControl-text-box-text$/.exec(id);

        return match && this.registrations[match[1]] ? match[1] : null;

    },

    onInput: function (event) {

        var self = this;
        var fieldName = this.fieldNameOf(event.target);

        if (!fieldName || this.selecting) {
            return;
        }

        var registration = this.registrations[fieldName];
        var input = event.target;
        var value = input.value;
        var token = this.currentToken(value, registration.multi);

        Promise.resolve(registration.getEntries()).then(function (entries) {

            if (self.doc().activeElement !== input) {
                return;
            }

            var found = self.search(entries, token, self.selectedNames(value, registration.multi));

            if (found.length === 0) {
                self.close();
                return;
            }

            self.open(input, fieldName, found);

        }).catch(function () {
            self.close();
        });

    },

    onMouseDown: function (event) {

        if (!this.state || !this.panel || !this.panel.contains(event.target)) {
            return;
        }

        var element = event.target;

        while (element && element.parentNode !== this.panel) {
            element = element.parentNode;
        }

        event.preventDefault();
        event.stopPropagation();

        var index = element ? parseInt(element.getAttribute("data-index"), 10) : -1;

        if (index >= 0 && this.state.items[index]) {
            this.select(this.state.items[index].name);
        }

    },

    onKeyDown: function (event) {

        var fieldName = this.fieldNameOf(event.target);

        if (!fieldName || !this.state || this.state.fieldName !== fieldName) {
            return;
        }

        var state = this.state;

        if (event.key === "ArrowDown" || event.key === "ArrowUp") {
            event.preventDefault();
            state.index = (state.index + (event.key === "ArrowDown" ? 1 : -1) + state.items.length) % state.items.length;
            this.render();
        } else if (event.key === "Enter" || event.key === "Tab") {
            if (state.index >= 0) {
                event.preventDefault();
                this.select(state.items[state.index].name);
            }
        } else if (event.key === "Escape") {
            this.close();
        }

    },

    open: function (input, fieldName, items) {

        this.state = { input: input, fieldName: fieldName, items: items, index: -1 };

        if (!this.panel) {
            this.panel = this.doc().createElement("div");
            this.panel.setAttribute("data-ksga-field-picker", "true");
            this.panel.style.cssText = "position:fixed;z-index:2147483000;background:#fff;color:#201f1e;border:1px solid #c8c6c4;"
                + "box-shadow:0 4px 12px rgba(0,0,0,.2);font:14px 'Segoe UI',sans-serif;max-height:320px;overflow-y:auto;";
            this.doc().body.appendChild(this.panel);
        }

        var rect = input.getBoundingClientRect();

        this.panel.style.left = rect.left + "px";
        this.panel.style.top = rect.bottom + "px";
        this.panel.style.minWidth = Math.max(rect.width, 320) + "px";
        this.panel.style.display = "block";

        this.render();

    },

    render: function () {

        var self = this;
        var state = this.state;

        this.panel.innerHTML = "";

        state.items.forEach(function (item, index) {
            var doc = self.doc();
            var row = doc.createElement("div");
            var name = doc.createElement("span");
            var type = doc.createElement("span");

            row.setAttribute("data-index", String(index));
            row.style.cssText = "display:flex;justify-content:space-between;gap:16px;padding:6px 10px;cursor:pointer;"
                + (index === state.index ? "background:#e1dfdd;" : "");

            name.textContent = item.label ? item.name + " (" + item.label + ")" : item.name;
            type.textContent = item.type;
            type.style.cssText = "color:#605e5c;font-size:12px;";

            row.appendChild(name);
            row.appendChild(type);

            row.addEventListener("mouseenter", function () {
                state.index = index;
                self.highlight();
            });

            self.panel.appendChild(row);
        });

    },

    highlight: function () {

        var state = this.state;

        Array.prototype.forEach.call(this.panel.children, function (row, index) {
            row.style.background = index === state.index ? "#e1dfdd" : "";
        });

    },

    select: function (name) {

        var state = this.state;

        if (!state) {
            return;
        }

        var registration = this.registrations[state.fieldName];
        var attribute = registration.formContext.getAttribute(state.fieldName);
        var value = this.applySelection(state.input.value, name, registration.multi);

        this.close();

        this.selecting = true;

        try {
            this.writeInput(state.input, value);

            if (attribute) {
                attribute.setValue(value);
                attribute.fireOnChange();
            }
        } finally {
            this.selecting = false;
        }

    },

    writeInput: function (input, value) {

        var descriptor = Object.getOwnPropertyDescriptor(Object.getPrototypeOf(input), "value");

        if (descriptor && typeof descriptor.set === "function") {
            descriptor.set.call(input, value);
        } else {
            input.value = value;
        }

        var EventType = (input.ownerDocument && input.ownerDocument.defaultView && input.ownerDocument.defaultView.Event) || Event;

        input.dispatchEvent(new EventType("input", { bubbles: true }));
        input.dispatchEvent(new EventType("change", { bubbles: true }));

    },

    close: function () {

        this.state = null;

        if (this.panel) {
            this.panel.style.display = "none";
        }

    },

    loadCatalog: function (entityName) {

        if (!entityName || typeof fetch !== "function" || typeof Xrm === "undefined") {
            return Promise.resolve(null);
        }

        if (!this.catalogs[entityName]) {
            var url = Xrm.Utility.getGlobalContext().getClientUrl()
                + "/api/data/v9.2/EntityDefinitions(LogicalName='" + encodeURIComponent(entityName) + "')"
                + "/Attributes?$select=LogicalName,AttributeType,SourceType,IsValidForUpdate,DisplayName";

            this.catalogs[entityName] = fetch(url, { headers: { "Accept": "application/json", "OData-Version": "4.0" }, credentials: "same-origin" })
                .then(function (response) {
                    return response.ok ? response.json() : null;
                })
                .then(function (json) {

                    if (!json || !json.value) {
                        return null;
                    }

                    var catalog = {};

                    json.value.forEach(function (attribute) {
                        catalog[attribute.LogicalName] = Ksga.FieldPicker.mapAttribute(attribute);
                    });

                    return catalog;

                })
                .catch(function () {
                    return null;
                });
        }

        return this.catalogs[entityName];

    },

    mapAttribute: function (attribute) {

        var label = attribute.DisplayName && attribute.DisplayName.UserLocalizedLabel
            ? attribute.DisplayName.UserLocalizedLabel.Label
            : "";

        return {
            type: attribute.AttributeType,
            sourceType: attribute.SourceType,
            isValidForUpdate: attribute.IsValidForUpdate,
            label: label
        };

    }

};
