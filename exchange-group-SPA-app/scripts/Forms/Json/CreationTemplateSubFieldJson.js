var Forms;
(function (Forms) {
    (function (Json) {
        var CreationTemplateSubFieldJson = (function () {
            function CreationTemplateSubFieldJson(fieldTitle, validationRegexString) {
                this.fieldTitle = fieldTitle;
                this.validationRegexString = validationRegexString;
                //Data structure
            }
            return CreationTemplateSubFieldJson;
        })();
        Json.CreationTemplateSubFieldJson = CreationTemplateSubFieldJson;
    })(Forms.Json || (Forms.Json = {}));
    var Json = Forms.Json;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationTemplateSubFieldJson.js.map
