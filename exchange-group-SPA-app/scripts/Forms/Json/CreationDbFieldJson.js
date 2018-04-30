var Forms;
(function (Forms) {
    (function (Json) {
        var CreationDbFieldJson = (function () {
            function CreationDbFieldJson(dbFieldId, displayTitle, displayType, validation, validationFailureMessage, validationRegexString, overrideDefaultValues, overrideDefaultValuesList, position) {
                this.dbFieldId = dbFieldId;
                this.displayTitle = displayTitle;
                this.displayType = displayType;
                this.validation = validation;
                this.validationFailureMessage = validationFailureMessage;
                this.validationRegexString = validationRegexString;
                this.overrideDefaultValues = overrideDefaultValues;
                this.overrideDefaultValuesList = overrideDefaultValuesList;
                this.position = position;
                // No content - DataType Struct
            }
            return CreationDbFieldJson;
        })();
        Json.CreationDbFieldJson = CreationDbFieldJson;
    })(Forms.Json || (Forms.Json = {}));
    var Json = Forms.Json;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationDbFieldJson.js.map
