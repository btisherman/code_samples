var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ClientControls;
(function (ClientControls) {
    var DropdownBase = (function (_super) {
        __extends(DropdownBase, _super);
        function DropdownBase(dbFieldName, requiredField, title, templateName, toolTip) {
            var _this = this;
            _super.call(this, dbFieldName, requiredField, title, templateName, toolTip);
            this.hasSubscriptions = false;
            this.addSubscription = function (caller, callback) {
                _this.subscriptions.push(new Subscription(caller, callback));

                if (!_this.hasSubscriptions) {
                    _this.selectedValue.subscribe(_this.fireSubscriptions);
                }
            };
            this.fireSubscriptions = function () {
                var i, newValue = _this.selectedValue();
                for (i = 0; i < _this.subscriptions.length; i++) {
                    _this.subscriptions[i].callback(_this.dbFieldName, newValue);
                }
            };
            this.removeSubscription = function (caller) {
                for (var i = 0; i < _this.subscriptions.length; i++) {
                    if (_this.subscriptions[i].caller == caller) {
                        _this.subscriptions.splice(i, 1);
                        return;
                    }
                }
            };
            this.getValue = function () {
                return _this.selectedValue();
            };

            // Setup knockout variables
            this.selectedValue = ko.observable("");

            this.subscriptions = [];
        }
        return DropdownBase;
    })(ClientControls.Field);
    ClientControls.DropdownBase = DropdownBase;
})(ClientControls || (ClientControls = {}));
//# sourceMappingURL=DropdownBase.js.map
