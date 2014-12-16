(function() {
	'use strict';

	var controllerId = 'setresourceitem';
	angular.module('app').controller(controllerId, ['$routeParams', '$location', '$scope', 'common', 'datacontext', 'config', setresourceitem]);

	function setresourceitem($routeParams, $location, $scope, common, datacontext, config) {

		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.title = 'Resource Item';
		vm.itemId = $routeParams.itemId || 0;
        vm.resourceId = $routeParams.resourceId;
      
        vm.ownerId = config.owner.ownerId;

        vm.item = {};
        vm.categories = [];

		activate();

		function activate() {
		    onDestroy();
			common.activateController(getResourceItem(), getResourceCategories(), getResources(), controllerId)
				.then(function() {
					//log('Activated set coupon');
				});
		}

		function getResourceItem() {

			if (vm.itemId > 0) {
				return datacontext.resource.getResourceItemById(vm.itemId)
					.then(function(data) {
						//applyFilter();
						return vm.item = data;
					});
			} else {
				return vm.item = datacontext.resource.createResourceItem(vm.resourceId);
			}
		}

        function getResourceCategories() {
            return datacontext.resource.getResourceItemCategoriesByOwnerId(vm.ownerId)
                .then(function(data) {
                    //applyFilter();
                    return vm.categories = data;
                });
        }

        function getResources() {
            return datacontext.resource.getResourcesByOwnerId(vm.ownerId)
                .then(function(data) {
                    //applyFilter();
                    return vm.resources = data;
                });
        }

        function onDestroy() {
            $scope.$on('$destroy', function () {
                datacontext.cancel();
            });
        }
      
        vm.cancel = function() {
            $location.path("/resourcedetail/" + vm.resourceId);
        };
      
        vm.saveAndNav = function() {
            return datacontext.save()
                .then(complete);

            function complete() {
                $location.path("/resourcedetail/" + vm.resourceId);
            }
        };

	}
})();