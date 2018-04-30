module Configuration {
    export module Analytics {
        export module Events {
            export class Page {
                static load = new EventPackage("c.page.load", "Info");
            }
            export class Navigation {
                static click = new EventPackage("c.nav.click", "Info");
            }
        }
        export class Global {
            static clientName: string = "EngageSite";
        }
    }
    export class Cookies {
        static sessionId: string = "sessid";
    }
    export class Urls {
        static root = "/";
        static apiPath = Urls.root + "api/";
        static analyticsPath = Urls.apiPath + "analytics/";
    }
}

