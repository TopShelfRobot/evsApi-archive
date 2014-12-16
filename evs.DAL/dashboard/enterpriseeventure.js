(function () {
    'use strict';
    var controllerId = 'enterpriseeventure';
    angular.module('app').controller(controllerId, ['$routeParams', 'common', 'datacontext', 'config', 'ExcelService', enterpriseeventure]);

    function enterpriseeventure($routeParams, common, datacontext, config, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.eventureId = $routeParams.eventureId;

        vm.ownerId = 1;


        activate();

        function activate() {
            common.activateController(getEventure(), ListingGrid(), PieChart(), Overview(), ServicesGrid(), controllerId)
                .then(function () {
                  //log('Activated Eventure Center View');
                });
        }

        function getEventure() {
          return datacontext.eventure.getEventureById(vm.eventureId)
            .then(function (data) {
                return vm.eventure = data;
            });
        }

        function ListingGrid() {

          var status = [{
            "value": true,
            "text": "Active",
          },{
            "value": false,
            "text": "Inactive"
          }];

          var listapi = config.remoteApiName + 'widget/getEventureListsByEventureId/' + vm.eventureId;
          vm.listGridOptions = {
            //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.listGrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Listings.xlsx',
                  filterable: true
              },
              dataSource: {
                type: "json",
                transport: {
                    read: listapi
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

        function PieChart() {

          var revapi = config.remoteApiName + 'widget/GetRevenuePerEvent/' + vm.ownerId;
          vm.revByList = {
            theme: "flat",
            dataSource: {
                transport: {
                    read: {
                        url: revapi,
                        dataType: "json"
                    }
                }
            },
            title: {
                position: "top",
                text: "Revenue By Listing",
                font: 14,
            },
            legend: {
                visible: false
            },
            chartArea: {
                height: 200
            },
            seriesDefaults: {
                type: "pie",
                labels: {
                    visible: false,
                    background: "transparent",
                    template: "#= category #: $#= value#"
                }
            },
            series: [{
                field: "revenuePercent",
                data: [20, 40, 45, 33],
                padding: 0,
                categoryField: "Listing"
            }],
            tooltip: {
                visible: true,
                template: "#= category #: $#= value#"
            }
          };

        }

        function Overview() {
          var overviewapi = config.remoteApiName +'widget/GetEventureGraph/' + vm.ownerId;

          vm.overviewByOwner = {
            theme: "flat",
                title: {
                    text: "Eventure Overview"
                },
                legend: {
                    position: "top"
                },
                series: [{
                    type: "bar",
                    data: [20, 40, 45, 33],
                    stack: true,
                    name: "Profit"
                }, {
                    type: "bar",
                    data: [20, 30, 35, 22],
                    stack: true,
                    name: "Expense"
                }, {
                    type: "line",
                    data: [30, 38, 40, 33],
                    name: "Registrations"
                }],
                valueAxes: [{
                    title: { text: "Profit" }
                }, {
                    name: "expense",
                    title: { text: "Expense" }
                }, {
                    name: "revenue",
                    title: { text: "Revenue" }
                }, {
                    name: "registrations",
                    title: { text: "Registrations" }
                }],
                categoryAxis: {
                    categories: ["Glow Go 5k", "Republic Bank Big...", "Run For...", "Buckhead Border..."],
                    axisCrossingValues: [0, 0, 10, 10]
                }
          };
        }

        vm.overview = function() {
          vm.overviewChart.redraw();
        };

        function ServicesGrid() {
          var serviceApi = config.remoteApiName + 'widget/GetEventureServiceByEventureId/' + vm.eventureId;
          vm.servicesGridOptions = {
            //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.serviceGrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Services.xlsx',
                  filterable: true
              },
              dataSource: {
                    transport: {
                        read: serviceApi
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
                    serverFiltering: false,
                    serverSorting: true
                },
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            contains: "Contains",
                            startswith: "Starts with",
                            eq: "Equal to"
                        }
                    }
                },
                sortable: true,
                pageable: true,
                dataBound: function () {
                },
                columns: [
                  { field: "resourceServiceText", title: "Service", width: "225px" },
                  { field: "amount", title: "Amount", format: "{0:c}", width: "175px" },
                  { field: "isVariable", title: "Variable Cost", width: "275px" }
                ]
          };
        }

        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };

    }
})();
