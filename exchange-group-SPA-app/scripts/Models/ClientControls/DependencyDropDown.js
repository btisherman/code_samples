var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ClientControls;
(function (ClientControls) {
    var DependencyDropDownList = (function (_super) {
        __extends(DependencyDropDownList, _super);
        function DependencyDropDownList(psFieldName, requiredField, title, templateName, toolTip, ancestors, directParent, initialParentValue, dependencyAllowedValues) {
            var _this = this;
            // Call Super
            _super.call(this, psFieldName, requiredField, title, templateName, toolTip);
            this.ancestors = ancestors;
            this.directParent = directParent;
            this.dependencyAllowedValues = dependencyAllowedValues;
            this.ancestorChanged = function (fieldName, value) {
                if (fieldName != _this.directParent) {
                    // We may not use ancestors in the future.
                    return;
                }

                var i;
                for (i = 0; i < _this.dependencyAllowedValues.length; i++) {
                    if (_this.dependencyAllowedValues[i].parentValue == value) {
                        _this.allowedValues(_this.dependencyAllowedValues[i].allowedValues);
                        break;
                    }
                }

                // Invalidate field if required
                var allowedValues = _this.allowedValues();
                var currentValue = _this.selectedValue();
                var isValid = false;

                for (i = 0; i < allowedValues.length; i++) {
                    if (currentValue == allowedValues[i]) {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid) {
                    _this.selectedValue("");
                }
            };

            this.allowedValues = ko.observableArray([]);
        }
        return DependencyDropDownList;
    })(ClientControls.DropdownBase);
    ClientControls.DependencyDropDownList = DependencyDropDownList;
})(ClientControls || (ClientControls = {}));
//# sourceMappingURL=DependencyDropDown.js.map
