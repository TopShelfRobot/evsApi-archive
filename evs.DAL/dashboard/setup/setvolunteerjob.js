(function () {
	'use strict';

	var controllerId = 'setvolunteerjob';
	angular.module('app').controller(controllerId, ['$routeParams', '$location', '$scope', 'common', 'datacontext', setvolunteerjob]);

	function setvolunteerjob($routeParams, $location, $scope, common, datacontext) {
		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.job = {};
		vm.jobId = $routeParams.jobId;
		vm.eventureId = $routeParams.eventureId;
		vm.shifts = [];
		activate();

		function activate() {
			onDestroy();
			common.activateController(getVolunteerJob(), getVolunteerShifts(), controllerId)
				.then(function () { 
				  
				  //log('Activated Volunteer Schedule Edit'); 
				});
		}

		function getVolunteerJob() {
		  if(vm.jobId > 0) {
				return datacontext.volunteer.getVolunteerJobById(vm.jobId)
				  .then(function(data) {
						//applyFilter();
						return vm.job = data;
					});
		  }
		  else {
			  return vm.job = datacontext.volunteer.createVolunteerJob(vm.eventureId);
		  }
					
		}
	  
		function getVolunteerShifts() {
		  if(vm.jobId > 0) {
			return datacontext.volunteer.getVolunteerShiftsByVolunteerJobId(vm.jobId)
			  .then(function(data) {
				return vm.shifts = data;
				});
		  }
		}
	  
		vm.addNewShift = function () {
		   vm.shifts.push(datacontext.volunteer.createVolunteerShift(vm.jobId));
		};
	  
		vm.today = function () {
		   vm.shifts.dateShift = new Date();
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

		function onDestroy() {
			$scope.$on('$destroy', function () {
				datacontext.cancel();
			});
		}

		vm.formats = ['MM-dd-yyyy', 'yyyy/MM/dd', 'shortDate'];

		vm.format = vm.formats[0];

	  
		vm.cancel = function() {
			$location.path("/eventuredetail/" + vm.eventureId);
		};

		vm.saveAndNav = function() {
			return datacontext.save()
				.then(complete);

			function complete() {
				$location.path("/eventuredetail/" + vm.eventureId);
			}
		};

	}
})();
