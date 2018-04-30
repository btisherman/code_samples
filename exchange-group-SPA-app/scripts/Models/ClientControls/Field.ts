module ClientControls {
    export class Field {
        constructor(
            public dbFieldName: string,
            public requiredField: boolean,
            public title: string,
            public templateName: string,
            public toolTip: string) {

        }
    }
}