var Configuration;
(function (Configuration) {
    (function (Analytics) {
        var Global = (function () {
            function Global() {
            }
            Global.clientName = "EngageSite";
            return Global;
        })();
        Analytics.Global = Global;
    })(Configuration.Analytics || (Configuration.Analytics = {}));
    var Analytics = Configuration.Analytics;

    var Cookies = (function () {
        function Cookies() {
        }
        Cookies.sessionId = "sessid";
        return Cookies;
    })();
    Configuration.Cookies = Cookies;

    (function (Urls) {
        var Global = (function () {
            function Global() {
            }
            Global.root = "/";
            return Global;
        })();
        Urls.Global = Global;

        var Api = (function () {
            function Api() {
            }
            Api.apiPath = Urls.Global.root + "api/";

            Api.analytics = Api.apiPath + "analytics/";
            Api.forms = Api.apiPath + "forms/";
            Api.productStudio = Api.apiPath + "ps/";

            Api.formsHistory = Api.forms + "history/";
            Api.databaseList = Api.forms + "databases/";

            Api.productStudioMetadata = Api.productStudio + "fielddefinitions/";
            return Api;
        })();
        Urls.Api = Api;
    })(Configuration.Urls || (Configuration.Urls = {}));
    var Urls = Configuration.Urls;

    (function (Strings) {
        var Regex = (function () {
            function Regex() {
            }
            Regex.anythingRequired = "(.+)";
            Regex.anythingNotRequired = "(.*)";
            Regex.integersOnly = "^(-)?([0-9]+)$";
            Regex.numbersOnly = "^(-)?([0-9])+((\.)([0-9]+))*$";
            return Regex;
        })();
        Strings.Regex = Regex;
    })(Configuration.Strings || (Configuration.Strings = {}));
    var Strings = Configuration.Strings;

    var InternalRegex = (function () {
        function InternalRegex() {
        }
        InternalRegex.templateReplaceableMatch = /(\{[0-9]+\}){1}/g;
        InternalRegex.templateReplaceableInternalsMatch = /([0-9]+)/g;
        InternalRegex.rfcEmailAddress = /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/g;
        InternalRegex.rfcEmailAddressList = /^([a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)*((;){1}\s{0,2}(([a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)))*$/g;
        return InternalRegex;
    })();
    Configuration.InternalRegex = InternalRegex;
})(Configuration || (Configuration = {}));
//# sourceMappingURL=Configuration.js.map
