var Models;
(function (Models) {
    var DatabaseListItem = (function () {
        function DatabaseListItem(databaseProvider, databaseName) {
            this.databaseProvider = databaseProvider;
            this.databaseName = databaseName;
            this.displayText = this.databaseName + " [" + databaseProvider + "]";
        }
        return DatabaseListItem;
    })();
    Models.DatabaseListItem = DatabaseListItem;
})(Models || (Models = {}));
//# sourceMappingURL=DatabaseListItem.js.map
