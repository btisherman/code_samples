var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var EngageCreateView = (function (_super) {
    __extends(EngageCreateView, _super);
    function EngageCreateView() {
        var _this = this;
        _super.call(this);
        // validation
        this.checkIsEmailListValid = function () {
            return _this.emailList().match(Configuration.InternalRegex.rfcEmailAddressList).length === 1;
        };
        //  template chooser
        this.getFieldSetTemplate = function (object) {
            return object.template;
        };
        this.addStaticClick = function (data, event) {
            var available = _this.getAvailableFormFields();

            var callback = _this.generateDbIdChangedCallback("static.change.dbFieldId");

            var newStatic = new Forms.CreationStaticField(available, callback, _this.generateAllChangeCallback("static"));
            _this.staticFieldSets.push(newStatic);

            Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldAdd, "static");
        };
        this.removeStaticClick = function (data, event) {
            // Do remove logic, this should be an object?
            var payload = {
                dbFieldId: "nullrightnow",
                value: "nullRightNow"
            };

            Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldRemove, "static", payload);
        };
        this.addDynamicClick = function (data, event) {
            // Switch onfield Type
            var availableFieldIds, newField, fieldTypeCreate = _this.fieldTypeCreate(), newPosition = _this.dynamicFieldSets().length + 1, newPositionList = _this.generatePoisitionalList(newPosition);
            if (fieldTypeCreate.value === 0) {
                // This is a field mapping
                availableFieldIds = _this.getAvailableFormFields();

                newField = new Forms.CreationDbField(availableFieldIds, newPosition, newPositionList, _this.generateDbIdChangedCallback("dynamic.dbf.change.dbFieldId"), _this.generateAllChangeCallback("dynamic.dbf"));
            } else {
                availableFieldIds = _this.getAvailableLargeTextFields();
                newField = new Forms.CreationTemplateField(availableFieldIds, newPosition, newPositionList, _this.generateDbIdChangedCallback("dynamic.template.change.dbFieldId"), _this.generateAllChangeCallback("dynamic.template"));
            }

            newField.position.subscribe(function (newValue) {
                _this.positionChange(0, newValue);
            });

            _this.dynamicFieldSets.push(newField);
        };
        this.removeDynamicDbFieldClick = function (data, event) {
            // Do remove logic, this should be an object?
            var payload = {
                dbFieldId: "nullrightnow",
                value: "nullRightNow"
            };

            Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldRemove, "dynamic", payload);
        };
        this.generateAllChangeCallback = function (analyticsContext) {
            var callback = function (sender, fieldName, newValue) {
                var payload = {
                    type: analyticsContext,
                    fieldName: fieldName,
                    newvalue: newValue
                };
                Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldChangeValue, analyticsContext, payload);
            };
            return callback;
        };
        this.generateDbIdChangedCallback = function (analyiticsContext) {
            var callback = function (newTarget) {
                var fieldDefinition = _this.getBaseFieldDefinition(newTarget), restrictions = new Forms.FieldRestrictions(fieldDefinition.type, fieldDefinition.allowedValues), payload = {
                    newValue: newTarget
                };

                // Anytime the field ID changes in an existing object, we need to recalculate the available field IDs for the rest of the form.
                _this.recalcAvailableDbFields();

                Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldChangeValue, analyiticsContext, payload);
                return restrictions;
            };

            return callback;
        };
        this.getAvailableLargeTextFields = function () {
            return _this.getAvailableFormFields(64);
        };
        this.getAvailableFormFields = function (typeFilter) {
            var i, taken = [], available = [], statics = _this.staticFieldSets(), dyns = _this.dynamicFieldSets();

            for (i = 0; i < statics.length; i++) {
                taken.push(statics[i].dbFieldId());
            }

            for (i = 0; i < dyns.length; i++) {
                taken.push(dyns[i].dbFieldId());
            }

            for (i = 0; i < _this.metadata.rawFields.length; i++) {
                if (jQuery.inArray(_this.metadata.rawFields[i].name, taken) === -1) {
                    if (typeFilter) {
                        if (_this.metadata.rawFields[i].type != typeFilter) {
                            continue;
                        }
                    }

                    available.push(_this.metadata.rawFields[i].name);
                }
            }
            available.sort();
            return available;
        };
        this.getParentValue = function (parentId) {
            var i, statics = _this.staticFieldSets();
            for (i = 0; i < statics.length; i++) {
                if (statics[i].dbFieldId() === parentId) {
                    return statics[i].staticValue();
                }
            }
            return null;
        };
        this.getBaseFieldDefinition = function (fieldName) {
            var i;
            for (i = 0; i < _this.metadata.rawFields.length; i++) {
                if (_this.metadata.rawFields[i].name === fieldName) {
                    return _this.metadata.rawFields[i];
                }
            }

            // not found
            return null;
        };
        this.recalcAvailableDbFields = function () {
            var i, tempa, available = _this.getAvailableFormFields(), statics = _this.staticFieldSets(), dyns = _this.dynamicFieldSets();

            for (i = 0; i < statics.length; i++) {
                tempa = available.slice();
                tempa.push(statics[i].dbFieldId());
                statics[i].dbFieldChoices(tempa);
            }

            for (i = 0; i < dyns.length; i++) {
                tempa = available.slice();
                tempa.push(dyns[i].dbFieldId());
                dyns[i].dbFieldChoices(tempa);
            }
        };
        this.generatePoisitionalList = function (count) {
            var i, data = [];
            for (i = 0; i < count; i++) {
                data.push(i + 1);
            }
            return data;
        };
        this.applyNewPositionChoices = function (positionList) {
            var i, dyns = _this.dynamicFieldSets();
            for (i = 0; i < dyns.length; i++) {
                dyns[i].positionChoices(positionList);
            }
        };
        this.positionChange = function (currentPosition, targetPosition) {
            if (typeof targetPosition === "undefined") { targetPosition = 0; }
            var subdyns, size, index, down, mover, dyns = _this.dynamicFieldSets;

            /* If targetposition > currentPosition then we are moving up the list
            * If targetposition < currentPosition then we are moving down the list
            * If targetPosition == current position - then something is broken, this shouldnt be evented
            * If TargetPosition == 0 the element is being removed. We need to take the currentPosition and move everything +1 to the end. Special Logic
            */
            if (currentPosition === targetPosition) {
                // probably bad
                return;
            }

            if (targetPosition == 0) {
                index = currentPosition - 1;
                size = dyns.length - index;
            } else {
                if (currentPosition > targetPosition) {
                    index = targetPosition - 1;
                    down = false;
                } else {
                    index = currentPosition - 1;
                    down = true;
                }

                size = Math.abs(currentPosition - index);
            }

            subdyns = dyns.splice(index, size);

            if (down) {
                mover = subdyns.shift();
                subdyns.push(mover);
            } else {
                mover = subdyns.pop();
                subdyns.unshift(mover);
            }

            // Notify Listeners
            _this.dynamicFieldSets.splice(index, size, subdyns);

            var payload = {
                direction: down ? "down" : "up",
                remove: targetPosition === 0
            };

            Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldChangeValue, "dynamics.position.change", payload);
        };
        this.firstRun = function () {
            if (_this.initialized) {
                return;
            }

            _this.initialized = true;

            Api.Forms.Databases.getActive(function (data) {
                _this.databaseChoices(data);
            });
            // We have more init logic here later
        };
        this.onShow = function () {
            _this.firstRun();
        };

        this.formTitle = ko.observable("");
        this.team = ko.observable(undefined);
        this.emailList = ko.observable("");
        this.database = ko.observable(undefined);
        this.enableFilePicker = ko.observable("");

        this.teamChoices = ko.observableArray([]);
        this.databaseChoices = ko.observableArray([]);

        this.metadataLoaded = ko.observable(-1);
        this.teamsLoaded = ko.observable(-1);

        // Fieldsets
        this.staticFieldSets = ko.observableArray([]);
        this.dynamicFieldSets = ko.observableArray([]);

        this.fieldTypeCreate = ko.observable(undefined);

        // Static Array Init
        this.staticYesNoChoices = ["Yes", "No"];
        this.staticFieldTypeCreate = [
            new Forms.Option(-1, "Select..."),
            new Forms.Option(0, "Field Mapping"),
            new Forms.Option(1, "Template Field")
        ];

        this.database.subscribe(function (newValue) {
            if (newValue) {
                if (newValue.databaseProvider === "PS") {
                    var md = Database.Engage.metadata[newValue.databaseName];
                    if (md) {
                        // Something has been downloaded before
                        if (md.full) {
                            // We have everything we need
                            return;
                        }
                    }

                    _this.metadataLoaded(0);

                    Api.ProductStudio.Metadata.getMetadata(function (data) {
                        _this.metadata = data;
                        Database.Engage.metadata[newValue.databaseName] = data;
                        _this.metadataLoaded(1);
                    }, newValue.databaseName);
                }
            }
        });
    }
    return EngageCreateView;
})(BaseViews.SinglePageView);
//# sourceMappingURL=engageCreateView.js.map
