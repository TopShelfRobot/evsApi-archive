(function () {
    'use strict';

    var controllerId = 'listingdetail';
    angular.module('app').controller(controllerId, ['$routeParams', 'config', 'common', 'datacontext', 'ExcelService', listingdetail]);

    function listingdetail($routeParams, config, common, datacontext, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Listing Detail';
        vm.listing = {};
        vm.registrations = {};
        vm.capacity = {};
        vm.fees = {};

        vm.listingId = $routeParams.listingId || 0;

        activate();

        function activate() {
          var promises = [getListing(), Registrations(), Capacity(), FeeSchedule(), ParticipantGrid()];

          common.activateController(promises, controllerId)
              .then(function () {
                  //log('Activated Listing Detail View');
              });
          }

      //  vm.resize = function setChartSize() {
      //      if($("#groupchart") && $("#groupchart").data && $("#groupchart").data("kendoChart")){
      //          $("#groupchart").data("kendoChart").resize();
      //      }
      //  }

        function getListing() {
          return datacontext.eventure.getEventureListById(vm.listingId)
            .then(function (data) {
                return vm.listing = data;
            });
        }

        function Capacity() {
          return datacontext.analytic.getCapacityByEventureListId(vm.listingId)
            .then(function (data) {
              return vm.capacity = data;
            });
        }
        function FeeSchedule() {
          return datacontext.surcharge.getFeeSchedulesByEventureListId(vm.listingId)
            .then(function (data) {
              return vm.fees = data;
            });
        }

        function Registrations() {
          var regapi = config.remoteApiName + 'widget/GetEventureListGraph/' + vm.listingId;

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
              text: "Registrations by Month"
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
            },
            tooltip: {
                visible: true,
                format: "{0}%",
                template: "#= series.name #: #= value #"
            }
          };
        }

        vm.Groups = function() {
          var groupapi = config.remoteApiName + 'widget/GetEventureGroupGraphByList/' + vm.listingId;

          vm.groups = {
            theme: "bootstrap",
            dataSource: {
              transport: {
                  read: {
                      url: groupapi,
                      dataType: "json"
                  }
              }
            },
            title: {
              text: "Registrations by Group"
            },
            legend: {
              position: "bottom"
            },
            series: [{
              name: "Group Count",
              field: "regCount",
              colorField: "userColor",
              tooltip: { visible: true }
            }],
            seriesClick: function(e) {
              var url = '#partcenter/group/' + e.dataItem.id;
              log('url' + url);
            },
            valueAxis: {
              name: "regCount",
              labels: {
                  format: "{0}"
              }
            },
            categoryAxis: {
              field: "groupName",
              majorGridLines: {
                  visible: false
              }
            },
            tooltip: {
                visible: true,
                format: "{0}%",
                template: "#= series.name #: #= value #"
            }
          };
        }

        function ParticipantGrid() {

          var participantapi = config.remoteApiName +  'widget/GetRegisteredParticipantsByEventureListId/' + vm.listingId;
          vm.participantGridOptions = {
            //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.partgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Registered Participants.xlsx',
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
              filterable: {
                  mode: "row"
              },
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

        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };



    }

})();
