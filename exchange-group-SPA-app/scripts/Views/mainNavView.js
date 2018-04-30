var MainNavView = (function () {
    function MainNavView(parent) {
        var _this = this;
        this.parent = parent;
        this.rootClick = function () {
            // We only want to see this if we hid the mobile menu;
            if (_this.showMobileMenu()) {
                Analytics.log(Configuration.Analytics.Events.Navigation.click, "root");
            }

            _this.showMobileMenu(false);

            // This function will be called alot. Always continue processing events.
            return true;
        };
        this.mobileMenuButtonClick = function () {
            var mmstate = _this.showMobileMenu();
            _this.showMobileMenu(!mmstate);
            Analytics.log(Configuration.Analytics.Events.Navigation.click, "mobilemenubutton");
        };
        this.appClick = function () {
            Analytics.log(Configuration.Analytics.Events.Navigation.click, "applogo");
            return true;
        };
        this.showMobileMenu = ko.observable(false);
    }
    return MainNavView;
})();
//# sourceMappingURL=mainNavView.js.map
