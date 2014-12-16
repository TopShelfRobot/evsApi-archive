(function () {
    'use strict';
    var controllerId = 'enterpriselaunchpad';
    angular.module('app').controller(controllerId, ['common', 'datacontext', 'config', enterpriselaunchpad]);

    function enterpriselaunchpad(common, datacontext, config) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'app';
        vm.EventureGridOptions = {};
        vm.ownerId = 1;
        vm.treeData = [];
        vm.eventures = [];

        activate();

        function activate() {
            var promises = [getEvents(), NotificationGrid(),  Overview()];
            common.activateController(promises, controllerId)
                .then(function () {
                    //log('Activated Eventure Center View');
                });
        }


        function getEvents() {
            return datacontext.eventure.getEventuresGroupedByYearByOwnerId(vm.ownerId)
                .then(function (data) {
                    vm.eventures = data;

                    vm.treeviewOptions = {
                        dataSource: {
                            data: data
                        },
                        template: kendo.template($('#treeview-template').html())
                    };

                    return vm.eventures;
                });
        }



        function Overview() {
            var overviewapi = config.remoteApiName +'widget/GetOwnerGraph/' + vm.ownerId;

            vm.overviewByOwner = {
                theme: "material",
                dataSource: {
                    transport: {
                        read: {
                            url: overviewapi,
                            dataType: "json"
                        }
                    }
                },
                title: {
                    text: "Eventure Overview"
                },
                legend: {
                    position: "bottom"
                },
                series: [{
                    type: "column",
                    field: "Rev",
                    name: "Revenue",
                    axis: "Revenue"
                }, {
                    type: "line",
                    field: "Regs",
                    name: "Registrations",
                    axis: "Registrations"
                }],
                valueAxis: [{
                    name: "Revenue",
                    title: { text: "Revenue" }
                }, {
                    name: "Registrations",
                    title: { text: "Registrations" }
                }],
                categoryAxis: {
                    field: "Month",
                    majorGridLines: {
                        visible: false
                    }
                },
                tooltip: {
                    visible: true,
                    template: "#= series.name #: #= value #"
                }
            };
        }

        vm.overview = function() {
            vm.overviewChart.redraw();
        };

        function NotificationGrid() {
          var notifApi = config.remoteApiName + 'widget/GetNotificationsByOwnerId/' + vm.ownerId;
          vm.notificationGridOptions = {
            //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.notificationGrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'To Do.xlsx',
                  filterable: true
              },
              dataSource: {
                    transport: {
                        read: notifApi
                    },
                    schema: {
                        model: {
                            fields: {
                                DateDue: { type: "date" }
                            }
                        }
                    },
                    pageSize: 10,
                    serverPaging: false,
                    serverFiltering: false,
                    serverSorting: true
                },
                filterable: {
                  mode: "row"
                },
                sortable: true,
                pageable: true,
                //rowTemplate: kendo.template($("#rowTemplate").html()),
                dataBound: function () {
                },
                columns: [
                    { field: "task", width: "100px" },    //template is controlling this now //mjb
                    { field: "resource", width: "80px" },
                    { field: "eventure", width: "80px" },
                    { field: "dateDue", format: "{0:MM/dd/yyyy}", width: "70px" }
                ]
          };
        }

        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };

    }
})();
