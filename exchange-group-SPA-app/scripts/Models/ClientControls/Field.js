var ClientControls;
(function (ClientControls) {
    var Field = (function () {
        function Field(dbFieldName, requiredField, title, templateName, toolTip) {
            this.dbFieldName = dbFieldName;
            this.requiredField = requiredField;
            this.title = title;
            this.templateName = templateName;
            this.toolTip = toolTip;
        }
        return Field;
    })();
    ClientControls.Field = Field;
})(ClientControls || (ClientControls = {}));
//# sourceMappingURL=Field.js.map
