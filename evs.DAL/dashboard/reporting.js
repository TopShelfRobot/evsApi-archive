(function () {
    'use strict';
    var controllerId = 'reporting';
    angular.module('app').controller(controllerId, ['common', 'datacontext', reporting]);

    function reporting(common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.reports = [];


        activate();

        function activate() {
            //var promises = [getMessageCount(), getPeople()];
            common.activateController(getReports(), controllerId)
                .then(function () {
                    //log('Activated reporting View');
                });
        }

      function getReports() {
          return datacontext.analytic.getReportsByOwnerId(1)
          //return datacontext.getPeople()
              .then(function (data) {
                  //alert(data)
                  vm.reports = data;
                  return vm.reports;
              });
      }
    }
})();
