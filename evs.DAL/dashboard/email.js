(function () {
    'use strict';

    var controllerId = 'sendemail';
    angular.module('app').controller(controllerId, ['$scope', 'config', 'common', 'datacontext', 'ExcelService', sendemail]);

    function sendemail($scope, config, common, datacontext, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Listing Detail';
        vm.ownerId = config.owner.ownerId;
        vm.eventures = [];
        vm.listings = [];
        vm.emailType = 'event';

        activate();

        function activate() {
            onDestroy();
            var promises = [getEvents(), getListings()];

            common.activateController(promises, controllerId)
                .then(function () {
                    //log('Activated Listing Detail View');
                });
        }

        function getEvents() {
            return datacontext.eventure.getEventuresByOwnerId(vm.ownerId)
                .then(function (data) {
                    return vm.eventures = data;
                });
        }

        function getListings() {
            return datacontext.eventure.getEventureListsByOwnerId(vm.ownerId)
                .then(function (data) {
                    return vm.listings = data;
                });
        }

        function onDestroy() {
            //alert('destroy my contextttttttt!!!!');
            $scope.$on('$destroy', function () {
                //alert('destroymy contextttttttt!!!!!!!');
                //autoStoreWip(true);
                datacontext.cancel();
            });
        }



    }

})();
