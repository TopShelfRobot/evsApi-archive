(function () {
    'use strict';
    var controllerId = 'eventurecenter';
    angular.module('app').controller(controllerId, ['common', 'config', 'ExcelService', eventurecenter]);

    function eventurecenter(common, config, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.EventureGridOptions = {};

        vm.ownerId = 1;


        activate();

        function activate() {
            common.activateController(EventureGrid(), controllerId)
                .then(function () { 
                  //log('Activated Eventure Center View'); 
                });
        }

        function EventureGrid() {

          var status = [{
            "value": true,
            "text": "Active",
          },{
            "value": false,
            "text": "Inactive"
          }];

            // var eventureapi = config.remoteServiceName + 'GetAllEventuresByOwnerId/' + vm.ownerId;
          var eventureapi = config.remoteApiName + 'widget/GetAllEventuresByOwnerId/' + vm.ownerId;
          vm.eventureGridOptions = {
            //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.eventuregrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Eventures.xlsx',
                  filterable: true
              },
              dataSource: {
                type: "json",
                transport: {
                    read: eventureapi
                },
                schema: {
                    model: {
                        fields: {
                            active: { type: "boolean" },
                            displayDate: { type: "text" },
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
            filterable: {
                mode: "row"
            },
            columns: [{
                field: "name",
                title: "Event",
                template: '<a href="\\\#eventuredetail/#=id#">#=name#</a>',
                width: "500px",
            },{
                field: "displayDate",
                title: "Date",
            },{
                field: "active",
                values: status
            },{
                title: "",
                template:'<a class="btn btn-default btn-block" href="\\\#seteventure/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
            }]
          };

        }
      
        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };

    }
})();
