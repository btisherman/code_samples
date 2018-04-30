var Api;
(function (Api) {
    (function (Forms) {
        var Databases = (function () {
            function Databases() {
            }
            Databases.getActive = function (callback) {
                return $.ajax({
                    url: Configuration.Urls.Api.databaseList,
                    type: "GET",
                    dataType: "json",
                    error: function (j, t, e) {
                        return Analytics.logApiError(j, t, e, "forms.databases.get");
                    },
                    success: function (data) {
                        var i, convert = [];

                        for (i = 0; i < data.length; i++) {
                            convert[i] = new Models.DatabaseListItem(data[i].databaseProvider, data[i].databaseName);
                        }

                        callback(convert);
                        Analytics.log(Configuration.Analytics.Events.Api.logInfo, "forms.databases.get");
                    }
                });
            };
            return Databases;
        })();
        Forms.Databases = Databases;
    })(Api.Forms || (Api.Forms = {}));
    var Forms = Api.Forms;
})(Api || (Api = {}));
//# sourceMappingURL=Forms.Databases.js.map
