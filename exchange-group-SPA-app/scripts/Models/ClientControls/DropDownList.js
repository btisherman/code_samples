var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ClientControls;
(function (ClientControls) {
    var DropDownList = (function (_super) {
        __extends(DropDownList, _super);
        function DropDownList(psFieldName, requiredField, title, templateName, toolTip, allowedValues) {
            _super.call(this, psFieldName, requiredField, title, templateName, toolTip);

            this.allowedValues = ko.observableArray(allowedValues);
        }
        return DropDownList;
    })(ClientControls.DropdownBase);
    ClientControls.DropDownList = DropDownList;
})(ClientControls || (ClientControls = {}));
//# sourceMappingURL=DropDownList.js.map
