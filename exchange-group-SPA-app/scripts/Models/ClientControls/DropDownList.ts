module ClientControls {
    export class DropDownList extends DropdownBase implements IField<string>, IDropdown {

        constructor(
            psFieldName: string,
            requiredField: boolean,
            title: string,
            templateName: string,
            toolTip: string,
            allowedValues: string[]) {
            super(psFieldName, requiredField, title, templateName, toolTip);

            this.allowedValues = ko.observableArray(allowedValues);
        }
    }
}