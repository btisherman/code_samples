var Api;
(function (Api) {
    (function (ProductStudio) {
        var Metadata = (function () {
            function Metadata() {
            }
            Metadata.getMetadata = function (callback, product, fields) {
                var i, fmo, filter, url, ev;

                if (fields && fields.length > 0) {
                    filter = "";
                    fmo = fields.length - 1;
                    ev = "productstudio.metadata.filtered";
                    for (i = 0; i < fields.length; i++) {
                        filter = filter + fields[i];
                        if (i !== fmo) {
                            filter = filter + ",";
                        }
                    }

                    url = Configuration.Urls.Api.productStudioMetadata + product + "/filtered/" + filter;
                } else {
                    ev = "productstudio.metadata.all";
                    url = Configuration.Urls.Api.productStudioMetadata + product + "/all/";
                }

                return $.ajax({
                    url: url,
                    type: "GET",
                    dataType: "json",
                    error: function (j, t, e) {
                        return Analytics.logApiError(j, t, e, ev);
                    },
                    success: function (data) {
                        var blob = JSON.parse(data);
                        callback(blob);
                        Analytics.log(Configuration.Analytics.Events.Api.logInfo, ev);
                    }
                });
            };
            return Metadata;
        })();
        ProductStudio.Metadata = Metadata;
    })(Api.ProductStudio || (Api.ProductStudio = {}));
    var ProductStudio = Api.ProductStudio;
})(Api || (Api = {}));
//# sourceMappingURL=ProductStudioMetaData.js.map
