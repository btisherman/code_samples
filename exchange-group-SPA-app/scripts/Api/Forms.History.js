var Api;
(function (Api) {
    (function (Forms) {
        var History = (function () {
            function History() {
            }
            History.saveVisit = function (id) {
                return $.ajax({
                    url: Configuration.Urls.Api.formsHistory + id,
                    type: "POST",
                    error: function (j, t, e) {
                        return Analytics.logApiError(j, t, e, "forms.history.add");
                    },
                    success: function () {
                        Analytics.log(Configuration.Analytics.Events.Api.logInfo, "forms.history.add");
                    }
                });
            };

            History.getVisitHistory = function (callback) {
                return $.ajax({
                    url: Configuration.Urls.Api.formsHistory,
                    type: "GET",
                    dataType: "json",
                    error: function (j, t, e) {
                        return Analytics.logApiError(j, t, e, "forms.history.get");
                    },
                    success: function (data) {
                        callback(data);

                        Analytics.log(Configuration.Analytics.Events.Api.logInfo, "forms.history.get");
                    }
                });
            };
            return History;
        })();
        Forms.History = History;
    })(Api.Forms || (Api.Forms = {}));
    var Forms = Api.Forms;
})(Api || (Api = {}));
//# sourceMappingURL=Forms.History.js.map
