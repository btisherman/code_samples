var BaseViews;
(function (BaseViews) {
    var NavigationView = (function () {
        function NavigationView(parent) {
            this.parent = parent;
            this.externalClick = function (data, event) {
                var href = $(event.target).attr("href");
                Analytics.log(Configuration.Analytics.Events.Navigation.external, href);

                // always process the href
                return true;
            };
        }
        return NavigationView;
    })();
    BaseViews.NavigationView = NavigationView;
})(BaseViews || (BaseViews = {}));
//# sourceMappingURL=NavigationView.js.map
