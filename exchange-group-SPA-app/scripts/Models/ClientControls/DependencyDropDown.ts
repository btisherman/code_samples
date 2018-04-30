module ClientControls {
    export class DependencyDropDownList extends DropdownBase implements IField<string>, IDropdown {
        constructor(
            psFieldName: string,
            requiredField: boolean,
            title: string,
            templateName: string,
            toolTip: string,
            public ancestors: string[],
            public directParent: string,
            initialParentValue: string,
            public dependencyAllowedValues: DependencyAllowedValues[]) {

            // Call Super
            super(psFieldName, requiredField, title, templateName, toolTip);

            this.allowedValues = ko.observableArray([]);
        }

        ancestorChanged = (fieldName: string, value: string) => {
            if (fieldName != this.directParent) {
                // We may not use ancestors in the future.
                return;
            }

            var i;
            for (i = 0; i < this.dependencyAllowedValues.length; i++) {
                if (this.dependencyAllowedValues[i].parentValue == value) {
                    this.allowedValues(this.dependencyAllowedValues[i].allowedValues);
                    break;
                }
            }

            // Invalidate field if required 
            var allowedValues = this.allowedValues();
            var currentValue = this.selectedValue();
            var isValid = false;

            for (i = 0; i < allowedValues.length; i++) {
                if (currentValue == allowedValues[i]) {
                    isValid = true;
                    break;
                }
            }

            if (!isValid) {
                this.selectedValue("");
            }
        }
    }
}