
// This model will cover both TextBox and TextArea Templates. The underlying data is the same. 
module ClientControls {
    export class TextField extends Field implements IField<string> {

        value: KnockoutObservable<string>;

        constructor(
            psFieldName: string,
            requiredField: boolean,
            title: string,
            templateName: string,
            toopTip: string,
            public regexValidationString: string,
            public regexValidatedField: boolean) {

            super(psFieldName, requiredField, title, templateName, toopTip);

            // Setup knockout variables
            this.value = ko.observable("");
        }

        getValue = (): string => {
            return this.value();
        }
    }
}