var Forms;
(function (Forms) {
    var CreationTemplateSubField = (function () {
        function CreationTemplateSubField(anyChangeCallback) {
            var _this = this;
            this.anyChangeCallback = anyChangeCallback;
            this.setupChangeListeners = function () {
                _this.fieldTitle.subscribe(function (newValue) {
                    _this.anyChangeCallback(_this, "fieldTitle", newValue);
                });

                _this.validationRegexString.subscribe(function (newvalue) {
                    _this.anyChangeCallback(_this, "validationRegexString", newvalue);
                });
            };
            this.getJsonPrep = function () {
                return new Forms.Json.CreationTemplateSubFieldJson(_this.fieldTitle(), _this.validationRegexString());
            };
            this.fieldTitle = ko.observable("");
            this.validationRegexString = ko.observable(Configuration.Strings.Regex.anythingRequired);

            this.isValid = ko.computed(function () {
                return _this.fieldTitle().length > 0;
            });
        }
        return CreationTemplateSubField;
    })();
    Forms.CreationTemplateSubField = CreationTemplateSubField;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationTemplateSubField.js.map
