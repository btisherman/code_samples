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
                    error: function (jqXHR, textStatus, errorThrown) {
                        var payload = {
                            xhr: jqXHR,
                            status: textStatus,
                            error: errorThrown
                        };

                        Analytics.log(Configuration.Analytics.Events.Api.logError, "forms.history.add", payload);
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
                    error: function (jqXHR, textStatus, errorThrown) {
                        var payload = {
                            xhr: jqXHR,
                            status: textStatus,
                            error: errorThrown
                        };

                        Analytics.log(Configuration.Analytics.Events.Api.logError, "forms.history.add", payload);
                    },
                    success: function (data) {
                        callback(data);

                        Analytics.log(Configuration.Analytics.Events.Api.logInfo, "forms.history.add");
                    }
                });
            };
            return History;
        })();
        Forms.History = History;
    })(Api.Forms || (Api.Forms = {}));
    var Forms = Api.Forms;
})(Api || (Api = {}));
//# sourceMappingURL=History.js.map
