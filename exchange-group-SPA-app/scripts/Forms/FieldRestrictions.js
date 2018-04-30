var Forms;
(function (Forms) {
    var FieldRestrictions = (function () {
        function FieldRestrictions(fieldType, allowedValues) {
            this.fieldType = fieldType;
            this.allowedValues = allowedValues;
            // Struct
        }
        return FieldRestrictions;
    })();
    Forms.FieldRestrictions = FieldRestrictions;
})(Forms || (Forms = {}));
//# sourceMappingURL=FieldRestrictions.js.map
