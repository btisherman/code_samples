(function () {
    angular.module('AutoScheduler')

        .factory('AutoSchedulerDataService', [ '$http', function ($http) {
            var tools = [];
            var getSchedule = function (id) {
                return $http.get(server + '/api/AutoScheduler/GetSchedule/' + id);
            }

            return {
              getSchedule: getSchedule
            };
        }]);
})();