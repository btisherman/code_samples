module ClientControls {
    export class DropdownBase extends Field implements IField<string>, IDropdown {
        public selectedValue: KnockoutObservable<string>;
        public allowedValues: KnockoutObservableArray<string>;

        subscriptions: Subscription[];
        hasSubscriptions: boolean = false;

        constructor(
            dbFieldName: string,
            requiredField: boolean,
            title: string,
            templateName: string,
            toolTip: string) {

            super(dbFieldName, requiredField, title, templateName, toolTip);

            // Setup knockout variables
            this.selectedValue = ko.observable("");

            this.subscriptions = [];

        }

        addSubscription = (caller: string, callback: Function): void => {
            this.subscriptions.push(new Subscription(caller, callback));

            if (!this.hasSubscriptions) {
                this.selectedValue.subscribe(this.fireSubscriptions);
            }
        }

        fireSubscriptions = (): void => {
            var i, newValue = this.selectedValue();
            for (i = 0; i < this.subscriptions.length; i++) {
                this.subscriptions[i].callback(this.dbFieldName, newValue);
            }
        }

        removeSubscription = (caller: string): void => {
            for (var i = 0; i < this.subscriptions.length; i++) {
                if (this.subscriptions[i].caller == caller) {
                    this.subscriptions.splice(i, 1);
                    return;
                }
            }
        }

        getValue = (): string => {
            return this.selectedValue();
        }


    }
} 