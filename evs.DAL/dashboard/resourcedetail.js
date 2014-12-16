(function () {
	'use strict';
	var controllerId = 'resourcedetail';
	angular.module('app').controller(controllerId, ['$routeParams', '$location', 'common', 'datacontext', 'config', 'ExcelService', resourcedetail]);

	function resourcedetail($routeParams, $location, common, datacontext, config, excel) {
		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.title = 'Resource Detail';
		vm.ownerId = config.owner.ownerId;
		vm.resourceId = $routeParams.resourceId;
		vm.resource = {};

		activate();

		function activate() {
			var promises = [createresourceDetailGrid(), getResource()];
			common.activateController(promises, controllerId)
				.then(function () {
					//log('Activated Resource Detail View'); 
				});
		}

		function getResource() {
			//    resourceId = parseInt(routeData.id);
			return datacontext.resource.getResourceById(vm.resourceId)
				.then(function (data) {
					return vm.resource = data;
				});
		};


		vm.saveAndNav = function () {
			return datacontext.save()
				.then(complete);

			function complete() {
				$location.path("/resourcecenter/");
			}
		};

		function createresourceDetailGrid() {

			var resourceApi = config.remoteApiName + 'widget/GetResourceItemsByResourceId/' + vm.resourceId;
			//alert(ResourceApi);

			vm.resourceDetailGridOptions = {
				//toolbar: '<a download="detail.xlsx" class="k-button" ng-click="vm.excel(vm.resourcegrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
				toolbar: ['excel'],
				excel: {
					fileName: 'Resource Items.xlsx',
					filterable: true
				},
				dataSource: {
					type: "",
					transport: {
						read: resourceApi
					},
					pageSize: 10,
					serverPaging: false,
					serverFiltering: false,
					serverSorting: true
				},
				sortable: true,
				pageable: true,
				filterable: {
					mode: "row"
				},
				columns: [
					{ field: "name", title: "Item Name", width: "150px" },
					{ field: "cost", title: "Cost", width: "70px" },
					{ field: "category", title: "Category", width: "100px" }                                                                                 //:rid/:riid',
				]
			};
		}

		vm.excel = function (data) {
			var gridname = data;
			excel.export(gridname);
		};
	}
})();
