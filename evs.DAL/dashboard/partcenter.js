(function () {
    'use strict';
    var controllerId = 'partcenter';
    angular.module('app').controller(controllerId, ['common', 'config', 'ExcelService', partcenter]);

    function partcenter(common, config, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.participantGridOptions = {};

        vm.ownerId = 1;


        activate();

        function activate() {
            common.activateController(ParticipantGrid(), controllerId)
                .then(function () { 
                  //log('Activated Participant Center View'); 
                });
        }

        function ParticipantGrid() {

            var partapi = config.remoteApiName + 'widget/GetParticipantsByOwnerId/' + vm.ownerId;
            //var partapi = config.remoteApiName + 'widget/parts/' + vm.ownerId;

          vm.participantGridOptions = {
            //toolbar: '<a download="detail.xlsx" class="k-button" ng-click="vm.excel(vm.partgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Participants.xlsx',
                  filterable: true
              },
              dataSource: {
                type: "json",
                transport: {
                    read: partapi
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
            detailTemplate: kendo.template($("#template").html()),
            columns: [{
                field: "firstName",
                title: "First Name",
                width: "200px"
            },{
                field: "lastName",
                title: "Last Name",
                width: "200px"
            },{
                field: "email",
                title: "Email Address",
                width: "220px"
            },{
                title: "",
                width: "120px",
                template:'<a class="btn btn-default btn-block" href="\\\#user-profile/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
            }]
          };

          vm.detailGridOptions = function(e) {

            var regapi = config.remoteApiName + 'widget/GetRegistrationsByPartId/' + e.id;

            return {
                //toolbar: '<a download="detail.xlsx" class="k-button" ng-click="vm.excel(vm.detailgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
                toolbar: ['excel'],
                excel: {
                    fileName: 'Participant Registrations.xlsx',
                    filterable: true
                },
                dataSource: {
                    type: "json",
                    transport: {
                        read: regapi
                    },
                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 5
                },
                sortable: true,
                pageable: true,
                columns: [{
                    field: "displayName",
                    title: "Listing",
                    width: 300
                }, {
                    field: "totalAmount",
                    title: "Amount",
                    format: "{0:c}",
                    width: 150
                }, {
                    field: "quantity",
                    title: "Quantity",
                    width: 125
                }, {
                    field: "dateCreated",
                    title: "Registration Date",
                    type: "date",
                    format: "{0:MM/dd/yyyy}",
                    width: 200
                },{
                    field: '',
                    title: '',
                    template: '<a href="\\\#receipt/#=eventureOrderId#" class="btn btn-success btn-block"><em class="glyphicon glyphicon-tags"></em>&nbsp;&nbsp;Receipt</a>'
                }, {
                    field: '',
                    title: '',
                    template: '<a href="\\\#registration/#=id#" class="btn btn-default btn-block"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
                }]
            };
          };
        }

        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };

    }
})();
