var MainView = (function () {
    function MainView() {
        var _this = this;
        this.setupViews = function () {
            _this.nav = new MainNavView(_this);
        };
        this.setupViews();

        ko.applyBindings(this);

        Analytics.log(Configuration.Analytics.Events.Page.load, "Engage");
    }
    return MainView;
})();

window.viewModel = new MainView();
//# sourceMappingURL=mainView.js.map
