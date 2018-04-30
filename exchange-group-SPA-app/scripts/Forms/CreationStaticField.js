var Forms;
(function (Forms) {
    var CreationStaticField = (function () {
        function CreationStaticField(fieldIds, dbFieldIdChangedCallback, anyChangeCallback) {
            var _this = this;
            this.dbFieldIdChangedCallback = dbFieldIdChangedCallback;
            this.anyChangeCallback = anyChangeCallback;
            this.setupChangeListeners = function () {
                _this.dbFieldId.subscribe(function (newValue) {
                    _this.processFieldChange(newValue);
                    _this.anyChangeCallback(_this, "dbFieldId", newValue);
                });

                _this.staticValue.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "staticValue", newValue);
                });
            };
            this.processFieldChange = function (newValue) {
                var restrictions = _this.dbFieldIdChangedCallback(newValue);
                _this.restrictions(restrictions);
                _this.allowedValues(restrictions.allowedValues);
            };
            this.getJsonPrep = function () {
                return new Forms.Json.CreationStaticFieldJson(_this.dbFieldId(), _this.staticValue());
            };
            this.allowedValues = ko.observableArray([]);

            this.dbFieldChoices = ko.observableArray(fieldIds);
            this.dbFieldId = ko.observable("");
            this.staticValue = ko.observable("");
            this.restrictions = ko.observable(null);

            this.isValueValid = ko.computed(function () {
                return _this.staticValue() !== "";
            });

            this.isFieldValid = ko.computed(function () {
                return _this.dbFieldId() !== "";
            });

            this.isValid = ko.computed(function () {
                return (_this.isValueValid() && _this.isFieldValid());
            });

            this.inputTemplate = ko.computed(function () {
                // 0 = Single Value WITH allowed Values (dependant or not)
                // 1 = Single Value With No allowed Values (TextBox)
                // 2 = Large Text
                // 3 = Date Picker
                // 4 = Integer
                var r = _this.restrictions();
                if (r) {
                    switch (r.fieldType) {
                        case 16:
                            if (r.allowedValues && r.allowedValues.length > 0) {
                                return "creation-static-dropdown";
                            }
                            return "creation-static-textbox";
                        case 64:
                            return "creation-static-textarea";
                        case 48:
                            return "creation-static-datepicker";
                        default:
                            return "creation-static-textbox";
                    }
                }
                return "creation-static-textbox";
            });

            this.setupChangeListeners();
        }
        return CreationStaticField;
    })();
    Forms.CreationStaticField = CreationStaticField;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationStaticField.js.map
