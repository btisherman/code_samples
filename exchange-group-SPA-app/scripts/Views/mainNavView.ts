class MainNavView implements INavigationView<MainView> {
    public showMobileMenu: KnockoutObservable<boolean>;

    constructor(public parent: MainView) {
        this.showMobileMenu = ko.observable(false);
    }

    rootClick = () => {
        // We only want to see this if we hid the mobile menu;
        if (this.showMobileMenu()) {
            Analytics.log(Configuration.Analytics.Events.Navigation.click, "root");
        }

        this.showMobileMenu(false);

        // This function will be called alot. Always continue processing events.
        return true;
    }

    mobileMenuButtonClick = () => {
        var mmstate = this.showMobileMenu();
        this.showMobileMenu(!mmstate);
        Analytics.log(Configuration.Analytics.Events.Navigation.click, "mobilemenubutton");
    }

    appClick = () => {
        Analytics.log(Configuration.Analytics.Events.Navigation.click, "applogo");
        return true;
    }
}