var Forms;
(function (Forms) {
    (function (Json) {
        var CreationTemplateFieldJson = (function () {
            function CreationTemplateFieldJson(templateText, dbFieldId, subFields) {
                this.templateText = templateText;
                this.dbFieldId = dbFieldId;
                this.subFields = subFields;
                // Data Structure
            }
            return CreationTemplateFieldJson;
        })();
        Json.CreationTemplateFieldJson = CreationTemplateFieldJson;
    })(Forms.Json || (Forms.Json = {}));
    var Json = Forms.Json;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationTemplateFieldJson.js.map
