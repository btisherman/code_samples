var Forms;
(function (Forms) {
    var CreationTemplateField = (function () {
        function CreationTemplateField(fields, position, positionChoices, dbFieldIdChangedCallback, anyChangeCallback) {
            var _this = this;
            this.dbFieldIdChangedCallback = dbFieldIdChangedCallback;
            this.anyChangeCallback = anyChangeCallback;
            this.template = "create-fieldset-template";
            this.setupChangeListeners = function () {
                _this.dbFieldId.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "dbFieldId", newValue);
                });

                _this.templateText.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "templateText", newValue);
                });
            };
            this.validateThis = function () {
                var i, val, min = 0, max = 0, last = 0, gap = false, sfc = _this.subFields().length, dvals = _this.getTemplateReplaceSymbols(_this.templateText()).sort(function (a, b) {
                    return a - b;
                });
                var vals = Helpers.Global.distinct(dvals);

                for (i = 0; i < vals.length; i++) {
                    val = vals[i];

                    if (val < min) {
                        min = val;
                    }

                    if (val > max) {
                        max = val;
                    }

                    if ((val - last) > 1) {
                        gap = true;
                    }
                }

                // two clauses for two error messages;
                if (min !== 0) {
                    return 0;
                }

                if (sfc < vals.length) {
                    return 1;
                }

                if (sfc > vals.length) {
                    return 2;
                }

                if (gap) {
                    return 3;
                }

                return -1;
            };
            this.addSubFieldClick = function (data, event) {
                var newField = new Forms.CreationTemplateSubField(_this.anyChangeCallback);

                // Propigate validation Up.
                newField.isValid.subscribe(function (newValue) {
                    _this.subFieldsValid(_this.validateSubFields());
                });

                _this.subFields.push(newField);

                Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldAdd, "template.subfield");

                return false;
            };
            this.validateSubFields = function () {
                var subFields = _this.subFields();
                for (var i = 0; i < subFields.length; i++) {
                    if (!subFields[i].isValid) {
                        return false;
                    }
                }
                return true;
            };
            this.removeSubFieldClick = function (data, event) {
                // NEED INFO
                var x = false;

                Analytics.log(Configuration.Analytics.Events.Engage.Creation.fieldRemove, "template.subfield");

                return false;
            };
            this.getTemplateReplaceSymbols = function (template) {
                var subMatch, oParse, returnData = [], matches = template.match(Configuration.InternalRegex.templateReplaceableMatch);

                for (var i = 0; i < matches.length; i++) {
                    subMatch = matches[i].match(Configuration.InternalRegex.templateReplaceableInternalsMatch);
                    oParse = parseInt(subMatch[0]);
                    returnData.push(oParse);
                }

                return returnData;
            };
            this.getJsonPrep = function () {
                var i, subtfj = [], subtf = _this.subFields();
                for (i = 0; i < subtf.length; i++) {
                    subtfj.push(subtf[i].getJsonPrep());
                }

                return new Forms.Json.CreationTemplateFieldJson(_this.templateText(), _this.dbFieldId(), subtfj);
            };
            this.dbFieldChoices = ko.observableArray(fields);

            this.dbFieldId = ko.observable("");
            this.dbFieldId.subscribe(function (newValue) {
                _this.dbFieldIdChangedCallback(newValue);
            });

            this.position = ko.observable(position);
            this.positionChoices = ko.observableArray(positionChoices);

            this.templateText = ko.observable("");
            this.subFields = ko.observableArray([]);
            this.subFieldsValid = ko.observable(true);

            this.validationState = ko.computed(this.validateThis);

            this.isValid = ko.computed(function () {
                var mainValid = _this.validationState() === -1;
                return mainValid && _this.subFieldsValid();
            });
        }
        return CreationTemplateField;
    })();
    Forms.CreationTemplateField = CreationTemplateField;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationTemplateField.js.map
