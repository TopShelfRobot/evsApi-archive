(function() {
    'use strict';

    var controllerId = 'setbundle';
    angular.module('app').controller(controllerId, ['$routeParams', '$location', '$scope', 'common', 'datacontext', 'config', setbundle]);

    function setbundle($routeParams, $location, $scope, common, datacontext, config) {

        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Eventure';
        vm.bundleId = $routeParams.bundleId || 0;

        vm.ownerId = config.owner.ownerId;

        vm.bundle = {};
        var listings = [];

        activate();

        function activate() {
            onDestroy();
            common.activateController(getEventureLists(),  controllerId)
                .then(function() {

                });
        }

        function getBundle() {

            if (vm.bundleId > 0) {
                return datacontext.surcharge.getAddonById(vm.addonId)
                    .then(function(data) {
                        //applyFilter();
                        return vm.addon = data;
                    });
            } else {
                return vm.addon = datacontext.surcharge.createAddon();
            }
        }

        function getEventureLists() {

            return datacontext.eventure.getEventureListsByOwnerId(vm.ownerId)
                .then(function(data) {
                    multiSelect(data);
                });
        }

        function multiSelect(listings) {
			// vm.multiSelect.value(listings);
			for(var i = 0; i < listings.length; i++){
				vm.multiSelect.dataSource.add({name: listings[i].name, id: listings[i].id});
			}
        }
		
		vm.selectedLists = [];
        vm.bundledListOptions = {
            placeholder: "Select listing...",
            dataTextField: "name",
            dataValueField: "id",
        };

        vm.cancel = function() {
            return datacontext.cancel()
                .then(complete);

            function complete() {
                $location.path("/#");
            }
        };

        function onDestroy() {
            //alert('destroy my contextttttttt!!!!');
            $scope.$on('$destroy', function () {
                //alert('destroymy contextttttttt!!!!!!!');
                //autoStoreWip(true);
                datacontext.cancel();
            });
        }

        vm.saveAndNav = function() {
			console.log("selectedLists:", vm.selectedLists);
            return datacontext.save(vm.addon)
                .then(complete);

            function complete() {
                $location.path("/#");
            }
        };

    }
})();