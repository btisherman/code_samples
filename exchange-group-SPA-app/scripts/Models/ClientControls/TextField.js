var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
// This model will cover both TextBox and TextArea Templates. The underlying data is the same.
var ClientControls;
(function (ClientControls) {
    var TextField = (function (_super) {
        __extends(TextField, _super);
        function TextField(psFieldName, requiredField, title, templateName, toopTip, regexValidationString, regexValidatedField) {
            var _this = this;
            _super.call(this, psFieldName, requiredField, title, templateName, toopTip);
            this.regexValidationString = regexValidationString;
            this.regexValidatedField = regexValidatedField;
            this.getValue = function () {
                return _this.value();
            };

            // Setup knockout variables
            this.value = ko.observable("");
        }
        return TextField;
    })(ClientControls.Field);
    ClientControls.TextField = TextField;
})(ClientControls || (ClientControls = {}));
//# sourceMappingURL=TextField.js.map
