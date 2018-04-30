var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var EngageHomeView = (function (_super) {
    __extends(EngageHomeView, _super);
    function EngageHomeView() {
        var _this = this;
        _super.call(this);
        this.populateHistory = function () {
            Api.Forms.History.getVisitHistory(function (data) {
                _this.formHistory(data);
            });
        };
        this.firstRun = function () {
            if (_this.initialized) {
                return;
            }

            _this.initialized = true;
            // We have more init logic here later
        };
        this.onShow = function () {
            _this.firstRun();
            _this.populateHistory();
        };

        this.formHistory = ko.observableArray([]);
    }
    return EngageHomeView;
})(BaseViews.SinglePageView);
//# sourceMappingURL=engageHomeView.js.map
