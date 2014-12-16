(function() {
    'use strict';

    var controllerId = 'setaddon';
    angular.module('app').controller(controllerId, ['$q', '$routeParams', '$upload', '$http', '$timeout', '$location', 'common', 'datacontext', 'config', setaddon]);

    function setaddon($q, $routeParams, $upload, $http, $timeout, $location, common, datacontext, config) {

        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Eventure';
        vm.addonId = $routeParams.addonId || 0;

        vm.ownerId = 1;

        vm.addon = {};
        vm.eventures = [];
        vm.listings = [];

        activate();

        function activate() {
            common.activateController(getAddon(), getEventures(), getEventureLists(), controllerId)
                .then(function() {
                    //log('Activated set addon');
                });
        }

        function getAddon() {

            if (vm.addonId > 0) {
                return datacontext.surcharge.getAddonById(vm.addonId)
                    .then(function(data) {
                        //applyFilter();
                        return vm.addon = data;
                    });
            } else {
                return vm.addon = datacontext.surcharge.createAddon();
            }
        }

        function getEventures() {
            return datacontext.eventure.getEventuresByOwnerId(vm.ownerId)
                .then(function(data) {
                    //applyFilter();
                    return vm.eventures = data;
                });
        }

        function getEventureLists() {
            return datacontext.eventure.getEventureListsByOwnerId(vm.ownerId)
                .then(function(data) {
                    //applyFilter();
                    return vm.listings = data;
                });
        }
      
        vm.cancel = function() {
          return datacontext.cancel()
            .then(complete);
          
            function complete() {
              $location.path("/#");
            }
        };

        vm.saveAndNav = function() {
            return datacontext.save(vm.addon)
                .then(complete);

            function complete() {
                $location.path("/#");
            }
        };

    }
})();