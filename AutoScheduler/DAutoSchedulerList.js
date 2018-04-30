(function () {
    'use strict';

    angular.module('AutoScheduler', ['myApp'])
        .directive('tvsmAutoScheduler', ['AutoSchedulerDataService',
            function (AutoSchedulerDataService) {
                return {
                    restrict: 'E',
                    scope: {
                      id: '@'
                    },
                    templateUrl: server + '/ngApp/AutoScheduler/partials/autoschedulerlist-partial.html',
                    controller: ['$scope', function ($scope) {
                        var vm = this;

                        function getNewData(id) {
                            AutoSchedulerDataService.getSchedule(id).then(function (res) {
                                vm.schedule = res.data;
                               console.log(res.data)
                            }, function (err) { console.log('error', err); })
                        }

                        getNewData($scope.id)
                        
                    }],
                    controllerAs: 'vm'
                }
            }]);
})();