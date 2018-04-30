var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var EngageNavView = (function (_super) {
    __extends(EngageNavView, _super);
    function EngageNavView(parent) {
        var _this = this;
        _super.call(this, parent);
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
        // ViewSpecific
        this.createClick = function () {
            if (_this.page() != "create") {
                _this.page("create");
                Helpers.Navigation.PseudoNavigate("/engage/create", true, "Create a new Form");
                _this.parent.create.onShow();
                _this.showMobileMenu(false);
                Analytics.log(Configuration.Analytics.Events.Navigation.click, "engage.create");
            }
        };

        this.page = ko.observable(engageServerVM.page);
        this.showMobileMenu = ko.observable(false);
    }
    return EngageNavView;
})(BaseViews.NavigationView);
//# sourceMappingURL=engageNavView.js.map
