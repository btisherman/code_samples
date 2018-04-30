var Forms;
(function (Forms) {
    (function (Json) {
        var CreationStaticFieldJson = (function () {
            function CreationStaticFieldJson(dbFieldId, value) {
                this.dbFieldId = dbFieldId;
                this.value = value;
                // Struct
            }
            return CreationStaticFieldJson;
        })();
        Json.CreationStaticFieldJson = CreationStaticFieldJson;
    })(Forms.Json || (Forms.Json = {}));
    var Json = Forms.Json;
})(Forms || (Forms = {}));
//# sourceMappingURL=CreationStaticFieldJson.js.map
