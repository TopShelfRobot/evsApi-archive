(function() {
	'use strict';

	var controllerId = 'seteventplanitem';
	angular.module('app').controller(controllerId, ['$routeParams', '$location', '$scope', 'common', 'datacontext', 'config', seteventplanitem]);

	function seteventplanitem($routeParams, $location, $scope, common, datacontext, config) {

		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.title = 'Event Plan Item';
	  
		vm.planItemId = $routeParams.itemId || 0;

		//alert('planItemId: ' + vm.planItemId);
		//if (vm.planItemId === parseInt(vm.planItemId))
		//    alert("11111111111data is integer");
		//else
		//    alert("11111111data is not an integer");

		vm.eventureId = $routeParams.eventureId;
		vm.ownerId = config.owner.ownerId;

		vm.planItem = {};
		vm.resources = [];

		activate();

		function activate() {
			onDestroy();
			common.activateController(getPlanItem(), getResources(), controllerId)
				.then(function() {
					//log('Activated set addon');
				});
		}

		function getPlanItem() {

			if (vm.planItemId > 0) {
				return datacontext.resource.getPlanItemById(parseInt(vm.planItemId))
					.then(function(data) {
						//applyFilter();
						return vm.planItem = data;
					});
			} else {
				return vm.planItem = datacontext.resource.createPlanItem(vm.eventureId);
			}

		}

		function getResources() {
			return datacontext.resource.getResourcesByOwnerId(vm.ownerId)
				.then(function(data) {
					//applyFilter();
					return vm.resources = data;
				});
		}
	  
		vm.today = function () {
		   vm.planItem.dateDue = new Date();
		};

		vm.today();

		vm.open = function($event, open) {
			$event.preventDefault();
			$event.stopPropagation();
			vm[open] = true;
		};

		vm.dateOptions = {
			'year-format': "'yy'",
			'starting-day': 1
		};

		vm.formats = ['MM-dd-yyyy', 'yyyy/MM/dd', 'shortDate'];

		vm.format = vm.formats[0];
		
		vm.cancel = function() {
			$location.path("/eventuredetail/" + vm.eventureId);
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
			return datacontext.save(vm.planItem)
				.then(complete);

			function complete() {
				$location.path("/eventuredetail/" + vm.eventureId);
			}
		};

	}
})();

//define(['services/logger', 'services/datacontext', 'config'],
//
//    function (logger, datacontext, config) {
//
//        var planItem = ko.observable();
//        var resources = ko.observableArray();
//        var eventureId = 0;
//        var planItemId;
//
//        var activate = function (routeData) {
//            planItemId = parseInt(routeData.id);
//            eventureId = parseInt(routeData.eid);
//            logger.log('create plan planItemId: ' + planItemId, null, 'plan', true);
//            logger.log('create plan eventureId: ' + eventureId, null, 'plan', true);
//
//
//           
//            datacontext.getResourcesByOwnerId(config.ownerId, resources);
//            
//            if (isNaN(planItemId)) {
//                logger.log('should nto go here', null, 'plan', true);
//                return planItem(datacontext.createPlanItem(eventureId));
//            }
//            else {
//                logger.log('int here', null, 'plan', true);
//                return datacontext.getPlanItemById(planItemId, planItem);
//            }
//        };
//
//        var clickSave = function () {
//            save();
//        };
//
//        var save = function () {
//            //isSaving(true);
//            //logger.log('called save', null, 'test', true);
//            return datacontext.saveChanges(planItem)
//                .then(complete);
//            //.fin(fin1);
//
//            function complete() {
//                //isSaving(false);
//                parent.$.fancybox.close(true);
//            }
//        };
//
//        var viewAttached = function (view) {
//            //logger.log('view attached', null, 'test', true);
//            // bindEventToList(view, '.events', gotoDetails);
//        };
//
//        var vm = {
//            activate: activate,
//            clickSave: clickSave,
//            planItem: planItem,
//            resources: resources,
//            viewAttached: viewAttached
//        };
//        return vm;
//    });
//
