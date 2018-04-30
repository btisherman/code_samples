var EngageView = (function () {
    function EngageView() {
        var _this = this;
        this.setupObservables = function () {
            _this.subTitleText = ko.observable("Home");
        };
        this.setupViews = function () {
            _this.create = new EngageCreateView();
            _this.home = new EngageHomeView();
            _this.nav = new EngageNavView(_this);
        };
        this.getRequestedPage = function () {
            switch (engageServerVM.page) {
                case "create":
                    _this.create.firstRun();
                    break;
                case "home":
                    _this.home.firstRun();
                    break;
            }
        };
        this.setupViews();
        this.setupObservables();

        ko.applyBindings(this);

        Analytics.log(Configuration.Analytics.Events.Page.load, "Engage" + engageServerVM.page);

        this.getRequestedPage();
    }
    return EngageView;
})();

// Initialize awesomeness
window.viewModel = new EngageView();
//# sourceMappingURL=engageView.js.map
