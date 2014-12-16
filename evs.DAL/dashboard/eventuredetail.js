(function () {
	'use strict';

	var controllerId = 'eventuredetail';
	angular.module('app').controller(controllerId, ['$routeParams', 'common', 'datacontext', 'config', 'ExcelService', 'Cloner', eventuredetail]);

	function eventuredetail($routeParams, common, datacontext, config, excel, cloner) {
		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.title = 'Eventure Detail';
		vm.eventure = {};
		vm.registrations = {};
		vm.capacity = {};
		vm.gauge = {};
		vm.eventureId = $routeParams.eventureId;

		activate();
		
		function activate() {
		  var promises = [getEventure(), Registrations(), Capacity(), ListingsGrid(), ExpenseGrid(), EventPlanGrid(), ParticipantGrid(), VolunteerGrid()];

		  common.activateController(promises, controllerId)
			  .then(function () {
				  //log('Activated Eventure Detail View');
			  });
		}

		function getEventure() {
		  return datacontext.eventure.getEventureById(vm.eventureId)
			.then(function (data) {
				return vm.eventure = data;
			});
		}

		function Registrations() {
		  var regapi = config.remoteApiName +'widget/GetEventureGraph/' + vm.eventureId;

		  vm.registrations = {
			theme: "bootstrap",
			dataSource: {
			  transport: {
				  read: {
					  url: regapi,
					  dataType: "json"
				  }
			  }
			},
			title: {
			  text: "Registrations YTD"
			},
			legend: {
			  position: "bottom"
			},
			series: [{
			  name: "Registrations",
			  field: "Regs",
			  colorField: "userColor",
			  axis: "registrations",
			  tooltip: { visible: true }
			}],
			valueAxis: {
			  name: "registrations",
			  labels: {
				  format: "{0:n0}"
			  }
			},
			categoryAxis: {
			  baseUnit: "months",
			  field: "Month",
			  majorGridLines: {
				  visible: false
			  }
			}
		  };
		}

		function Capacity() {
		  return datacontext.analytic.getCapacityByEventureId(vm.eventureId)
			.then(function (data) {
			  return vm.capacity = data;
			});
		}

		function ListingsGrid() {

		  var status = [{
			"value": true,
			"text": "Active",
		  },{
			"value": false,
			"text": "Inactive"
		  }];

		  var eventurelistapi = config.remoteApiName + 'widget/getEventureListsByEventureId/' + vm.eventureId;
		  vm.eventureListGridOptions = {
			//toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.listinggrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
			  toolbar: ['excel'],
			  excel: {
				  fileName: 'Listings.xlsx',
				  filterable: true
			  },
			  dataSource: {
				type: "json",
				transport: {
					read: eventurelistapi
				},
				schema: {
					model: {
						fields: {
							active: { type: "boolean" },
							dateEventureList: { type: "date" },
							dateBeginReg: { type: "date" },
							dateEndReg: { type: "date" },
							id: { type: "number" },
							name: { type: "string" }
						}
					}
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false
			},
			sortable: true,
			pageable: true,
			columns: [{
				title: "Listing",
				template: '<a href="\\\#elistcenter/#=id#">#=name#</a>'
			},{
				field: "dateEventureList",
				title: "Date",
				width: "220px",
				format: "{0:MM/dd/yyyy}"
			},{
				field: "dateBeginReg",
				title: "Registration Begins",
				width: "220px",
				format: "{0:MM/dd/yyyy}"
			},{
				field: "dateEndReg",
				title: "Registration Ends",
				width: "220px",
				format: "{0:MM/dd/yyyy}"
			},{
				field: "active",
				width: "100px",
				values: status
			},{
				title: "",
				width: "120px",
				template:'<a class="btn btn-default btn-block" href="\\\#setlist/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
			}]
		  };
		}

		function ExpenseGrid() {

		  var expenseapi = config.remoteApiName + 'widget/GetExpensesByEventureId/' + vm.eventureId;
		  vm.expenseGridOptions = {
			//toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.expensegrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
			  toolbar: ['excel'],
			  excel: {
				  fileName: 'Expenses.xlsx',
				  filterable: true
			  },
			  dataSource: {
				type: "json",
				transport: {
					read: expenseapi
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false
			},
			sortable: true,
			pageable: true,
			columns: [{
						field: "item",
						title: "Item",
						width: 200
					},
					{
						field: "category",
						title: "Category",
						width: 140,
						filterable: false
					},
					{
						field: "Cost",
						title: "Cost",
						width: 140,
						filterable: false
					},
					{
						field: "CostType",
						title: "Type",
						width: 140,
						filterable: false
					},
					{
						field: "PerRegNumber",
						title: "Formula",
						width: 140,
						filterable: false
			}]
		  };
		}

		function EventPlanGrid() {

		  var status = [{
				"value": true,
				"text": "Yes"
			  },
			  {
				"value": false,
				"text": "No"
			  }
		  ];

		  var eventplanapi = config.remoteApiName + 'widget/GetNotificationsByEventureId/' + vm.eventureId;
		  vm.eventPlanGridOptions = {
			//toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.plangrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
			  toolbar: ['excel'],
			  excel: {
				  fileName: 'Event Plan.xlsx',
				  filterable: true
			  },
			  dataSource: {
				type: "json",
				transport: {
					read: eventplanapi
				},
			  schema: {
					model: {
						fields: {
							dateDue: { type: "date" }
						}
					}
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false
			},
			sortable: true,
			pageable: true,
			columns: [{
						field: "task",
						title: "Task"
					},
					{
						field: "resource",
						title: "Resouce"
					},
					{
						field: "dateDue",
						title: "Due Date",
						width: "140px",
						format: "{0:MM/dd/yyyy}"
					  
					},
					{
						field: "isCompleted",
						title: "Completed",
						width: "140px",
						values: status,
						filterable: false
					},
					{
						title: "",
						width: "100px",
						filterable: false,
						template: '<a class="btn btn-default btn-block" ng-href="\\#/seteventplan/#=Id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'    //\\\#seteventplan//#=Id#
			}]
		  };
		}

		function ParticipantGrid() {

		  var participantapi = config.remoteApiName + 'widget/GetRegisteredParticipantsByEventureId/' + vm.eventureId;
		  vm.participantGridOptions = {
			//toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.partgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
			  toolbar: ['excel'],
			  excel: {
				  fileName: 'Participants.xlsx',
				  filterable: true
			  },
			  dataSource: {
				type: "json",
				transport: {
					read: participantapi
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false
			},
			sortable: true,
			pageable: true,
			columns: [{
						field: "firstName",
						title: "First Name",
					},
					{
						field: "lastName",
						title: "Last Name",
					},
					{
						field: "email",
						title: "Email Address",
					},
					{
						field: "city",
						title: "City",
						width: 200
					},
					{
						field: "state",
						title: "State",
						width: 80
					},
					{
						title: "",
						width: 100,
						filterable: false,
						template: '<a href="\\\#partedit/#=id#" class="btn btn-default btn-block "><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
			}]
		  };
		}

		function VolunteerGrid() {
			var volJobApi = config.remoteApiName + 'widget/GetVolunteerDataByEventureId/' + vm.eventureId;

			vm.volunteerGridOptions = {
				//toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.volunteergrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
				toolbar: ['excel'],
				excel: {
					fileName: 'Volunteers.xlsx',
					filterable: true
				},
				dataSource: {
					type: "json",
					transport: {
						read: volJobApi
					},
					pageSize: 10,
					serverPaging: true,
					serverSorting: true
				},
				selectable: "single cell",
				sortable: true,
				pageable: true,
				serverFiltering: true,
				detailTemplate: kendo.template($("#template").html()),
				filterable: {
					mode: "row"
				},
				dataBound: function() {
				},
				columns:[{
					field: "name",
					title: "Job Name",
					width: 350
				}, {
					field: "shifts",
					title: "Shifts",
					width: 200
				}, {
					field: "capacity",
					title: "Capacity",
					width: 200
				}, {
					field: "maxCapacity",
					title: "MaxCapacity",
					width: 200
				}, {
					field: '', title: '',
					template: '<a href="\\\#setvolunteerjob/#=Id#" class="btn btn-primary btn-small btn-block"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
				}]
			};
		  
			vm.volunteerDetailGridOptions = function(e) {

				var volunteerApi = config.remoteApiName + 'widget/GetVolunteersByVolunteerJobId/' + e.Id;

				return {
				  //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.detailgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
					toolbar: ['excel'],
					excel: {
						fileName: 'Registered Volunteer Jobs.xlsx',
						filterable: true
					},
					dataSource: {
						type: "json",
						transport: {
							read: volunteerApi
						},
						pageSize: 10,
						serverPaging: false,
						serverFiltering: false,
						serverSorting: true
					},
					sortable: true,
					pageable: true,
					dataBound: function() {
					},
					columns: [{
					field: "firstName",
					title: "First Name",
					width: 150
				}, {
					field: "lastName",
					title: "Last Name",
					width: 150
				}, {
					field: "email",
					title: "Email Address",
					width: 275
				}, {
					field: "timeBegin",
					title: "Start Time",
					format: "{0:h:mm tt}"
				}, {
					field: "timeEnd",
					title: "End Time",
					format: "{0:h:mm tt}"
				}, { title: "",
					 width: 100,
					 template: '<a class="btn btn-primary btn-small btn-block" href="\\\#setvolunteerscheduleedit/#=scheduleId#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>' }
					]
				};
			};
		  
		}
	  
		vm.excel = function(data) {
		  var gridname = data;
		  excel.export(gridname);
		};
		
		vm.clone = function(){
			$.blockUI({ message: "Cloning the Eventure..." });
			cloner.cloneEventure(vm.eventure)
				.then(function(out){
					console.log("done:", out);
					$.unblockUI();
				})
				.catch(function(err){
					console.log("err:", err);
				});
		}
	}

})();
