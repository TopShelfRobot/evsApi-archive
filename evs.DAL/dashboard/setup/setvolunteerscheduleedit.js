(function () {
    'use strict';

    var controllerId = 'setvolunteerschedule';
    angular.module('app').controller(controllerId, ['$routeParams', '$location', '$scope', 'common', 'datacontext', setvolunteerschedule]);

    function setvolunteerschedule($routeParams,$location, $scope, common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.volunteerschedule = {};
        vm.scheduleId = $routeParams.scheduleId || 0;

        activate();

        function activate() {
            onDestroy();
            common.activateController(getVolunteerSchedule(), controllerId)
                .then(function () { 
                  //log('Activated Volunteer Schedule Edit'); 
                });
        }

        function getVolunteerSchedule() {
            return datacontext.volunteer.getVolunteerScheduleById(vm.scheduleId)
                    .then(function(data) {
                        //applyFilter();
                        console.log(data);
                        return vm.volunteerschedule = data;
                    });
        }

        function onDestroy() {
            $scope.$on('$destroy', function () {
                datacontext.cancel();
            });
        }
      
        vm.cancel = function() {
            $location.path("/volunteercenter");
        };

        vm.saveAndNav = function() {
            return datacontext.save()
                .then(complete);

            function complete() {
                $location.path("/volunteercenter");
            }
        };

    }
})();
