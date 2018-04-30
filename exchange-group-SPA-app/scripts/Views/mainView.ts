class MainView implements IRootPageView<MainView> {
    public nav: MainNavView;

    constructor() {
        this.setupViews();

        ko.applyBindings(this);

        Analytics.log(Configuration.Analytics.Events.Page.load, "Engage");
    }

    setupViews = () => {
        this.nav = new MainNavView(this);
    }
}

window.viewModel = new MainView();