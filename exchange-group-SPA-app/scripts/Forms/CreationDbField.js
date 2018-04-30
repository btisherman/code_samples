var Forms;
(function (Forms) {
    var CreationDbField = (function () {
        function CreationDbField(fieldIds, position, positionChoices, dbFieldIdChangedCallback, anyChangeCallback) {
            var _this = this;
            this.dbFieldIdChangedCallback = dbFieldIdChangedCallback;
            this.anyChangeCallback = anyChangeCallback;
            this.template = "create-fieldset-direct";
            this.setupChangeListeners = function () {
                _this.dbFieldId.subscribe(function (newValue) {
                    _this.processDbFieldIdChange(newValue);
                    _this.anyChangeCallback(_this, "dbFieldId", newValue);
                });

                _this.displayTitle.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "displayTitle", newValue);
                });

                _this.displayType.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "displayType", newValue.displayText);
                });

                _this.validation.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "validation", newValue.displayText);
                });

                _this.validationFailureMessage.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "validationFailureMessage", newValue);
                });

                _this.validationRegexString.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "validationRegexString", newValue);
                });

                _this.overrideDefaultValues.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "overrideDefaultValues", newValue.displayText);
                });

                _this.overrideDefaultValuesList.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "overrideDefaultValuesList", newValue);
                });

                _this.position.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "position", newValue.toString());
                });
            };
            this.processDbFieldIdChange = function (newValue) {
                // 0 Single Value WITH allowed Values (dependant or not)
                // 1 = Single Value With No allowed Values (TextBox)
                // 2 = Large Text
                // 3 = Date Picker
                // 4 = Integer
                var restrictions = _this.dbFieldIdChangedCallback(newValue);

                var overr;
                switch (restrictions.fieldType) {
                    case 16:
                        // SingleValuedKeyword
                        if (restrictions.allowedValues != null && restrictions.allowedValues.length > 0) {
                            // Single Value WITH allowed Values (dependant or not)
                            _this.validationChoices(_this.staticValidationChoices.slice(0, 2));
                            _this.displayChoices([_this.staticDisplayChoices[3]]);

                            overr = 0;
                            _this.dbFieldTypeRestrictions(0);
                        } else {
                            // 1 = Single Value With No allowed Values (TextBox)
                            _this.validationChoices(_this.staticValidationChoices.slice(0, 3));
                            _this.displayChoices([_this.staticDisplayChoices[0], _this.staticDisplayChoices[2], _this.staticDisplayChoices[3]]);

                            // If they decide to show the field as a dopdown, we need to force them to give us a list of allowed values
                            overr = 1;
                            _this.dbFieldTypeRestrictions(1);
                        }
                        break;
                    case 64:
                        _this.validationChoices(_this.staticValidationChoices.slice(0, 3));

                        _this.displayChoices([_this.staticDisplayChoices[1]]);

                        overr = 0;
                        _this.dbFieldTypeRestrictions(2);
                        break;
                    case 48:
                        _this.validationChoices(_this.staticDisplayChoices.slice(0, 2));
                        _this.displayChoices([_this.staticDisplayChoices[2]]);

                        overr = 0;
                        _this.dbFieldTypeRestrictions(3);
                        break;
                    case 32:
                        _this.validationChoices(_this.staticDisplayChoices.slice(0, 2));
                        _this.displayChoices([_this.staticDisplayChoices[0]]);
                        _this.validationRegexString(Configuration.Strings.Regex.integersOnly);

                        overr = 0;
                        _this.dbFieldTypeRestrictions(4);
                        break;
                }

                _this.overrideDefaultValues(_this.staticOverrideDefaultValues[overr]);
            };
            this.getOverrideValuesList = function () {
                var vals = _this.overrideDefaultValuesList().split(",");
                for (var i = 0; i < vals.length; i++) {
                    vals[i] = vals[i].trim();
                }
                return vals;
            };
            this.getJsonPrep = function () {
                var i, overrideValuesSplit = _this.overrideDefaultValuesList().split(",");
                for (i = 0; i < overrideValuesSplit.length; i++) {
                    overrideValuesSplit[i] = overrideValuesSplit[i].trim();
                }

                return new Forms.Json.CreationDbFieldJson(_this.dbFieldId(), _this.displayTitle(), _this.displayType().value, _this.validation().value, _this.validationFailureMessage(), _this.validationRegexString(), _this.overrideDefaultValues().value === 1, _this.getOverrideValuesList(), _this.position());
            };
            // setup static arrays
            this.staticValidationChoices = [
                new Forms.Option(0, "Not Required"),
                new Forms.Option(1, "Required"),
                new Forms.Option(2, "Regex Validated")
            ];

            this.staticDisplayChoices = [
                new Forms.Option(0, "Text Box"),
                new Forms.Option(1, "Text Area"),
                new Forms.Option(2, "Date Picker"),
                new Forms.Option(3, "Drop Down Box")
            ];

            this.staticOverrideDefaultValues = [
                new Forms.Option(0, "No"),
                new Forms.Option(1, "Yes")
            ];

            // Observable Array Initialization
            this.validationChoices = ko.observableArray([]);
            this.displayChoices = ko.observableArray([]);
            this.dbFieldChoices = ko.observableArray(fieldIds);
            this.positionChoices = ko.observableArray(positionChoices);
            this.overrideDefaultValuesChoices = ko.observableArray(this.staticOverrideDefaultValues);

            this.dbFieldId = ko.observable("");
            this.displayTitle = ko.observable("");
            this.displayType = ko.observable(null);
            this.validation = ko.observable(null);
            this.validationFailureMessage = ko.observable("");
            this.validationRegexString = ko.observable("");
            this.overrideDefaultValues = ko.observable(null);
            this.overrideDefaultValuesList = ko.observable("");
            this.position = ko.observable(position);
            this.dbFieldTypeRestrictions = ko.observable(-1);

            // helper computeds
            this.showRegexInput = ko.computed(function () {
                var val = _this.validation();
                if (val) {
                    return val.value === 2;
                }
                return false;
            });

            this.showValidationFailureInput = ko.computed(function () {
                var val = _this.validation();
                if (val) {
                    return val.value !== 0;
                }
                return false;
            });

            this.showCustomAllowedValuesInput = ko.computed(function () {
                var odv = _this.overrideDefaultValues();
                if (odv) {
                    return odv.value === 1;
                }
                return false;
            });

            this.showOverrideAllowedValuesPicker = ko.computed(function () {
                var dt = _this.displayType();
                if (dt) {
                    return dt.value === 3;
                }
                return false;
            });

            this.enableOverrideAllowedValuesPicker = ko.computed(function () {
                var dt = _this.displayType(), dftr = _this.dbFieldTypeRestrictions();
                if (dt && dftr) {
                    return !(dt.value === 3 && dftr === 1);
                }
                return false;
            });

            this.isValid = ko.computed(function () {
                return true;
            });

            this.setupChangeListeners();
        }
        return CreationDbField;
    })();
    Forms.CreationDbField = CreationDbField;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationDbField.js.map
