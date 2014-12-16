(function () {
    'use strict';
    var controllerId = 'teamcenter';
    angular.module('app').controller(controllerId, ['$http', 'common', 'datacontext', 'config', 'ExcelService', teamcenter]);

    function teamcenter($http, common, datacontext, config, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.participantGridOptions = {};

        vm.ownerId = 1;


        activate();

        function activate() {
            common.activateController(teamGrid(), controllerId)
                .then(function () {
                    //log('Activated Team Center View');
                });
        }

        function teamGrid() {

          var teamapi = config.remoteApiName + 'widget/GetTeamRegistrationsByOwnerId/' + vm.ownerId;

          vm.teamGridOptions = {
            //toolbar: '<a download="Teams.xlsx" class="k-button" ng-click="vm.excel(vm.teamgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Teams.xlsx',
                  filterable: true
              },
              dataSource: {
                type: "json",
                transport: {
                    read: teamapi
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
                field: "name",
                title: "Team Name",
                width: "200px"
            },{
                field: "eventName",
                title: "Eventure",
                width: "200px"
            },{
                field: "listName",
                title: "Listing",
                width: "220px"
            },{
                field: "coachName",
                title: "Coach Name",
                width: "220px"
            }, {
                field: "amount",
                title: "Total Paid",
                width: "120px",
                format: "{0:c}",
                filterable: false
            }, {
                field: "balance",
                title: "Balance",
                width: "120px",
                format: "{0:c}",
                filterable: false,
                template: kendo.template($("#balanceTemplate").html())
            }, {
                title: "",
                width: "120px",
                template:'<a class="btn btn-default btn-block" href="\\\#/editteam/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
            }]
          };

          vm.detailGridOptions = function(e) {

            var teamapi = config.remoteApiName + 'widget/GetTeamMembersByTeamId/' + e.id;
            vm.remove = function() {
                alert('Removing: ' + e.Id );
                //datacontext.team.removeTeamMemberById(e.Id);
                vm.teamgrid.refresh();

            };

            vm.resend = function (memberId) {
                $http.post(config.apiPath + "/breeze/breeze/SendSoccerTryoutInviteMail/" + memberId)
                    .success(function(result) {
                        alert('Invitation has been sent');    //toast mjb
                    })
                    .error(function(err) {
                        console.error("ERROR:", err.toString());
                    });
            };

            return {
                //toolbar: '<a download="detailexport.xlsx" class="k-button" ng-click="vm.excel(vm.detailgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
                toolbar: ['excel'],
                excel: {
                    fileName: 'Team Participants.xlsx',
                    filterable: true
                },
                dataSource: {
                    type: "json",
                    transport: {
                        read: teamapi
                    },
                    schema: {
                        model: {
                            fields: {
                                id: { type: "number" }
                            }
                        }
                    },
                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 15    //mjb fix this just for lcfc??
                },
                sortable: true,
                pageable: true,
                detailTemplate: kendo.template($("#paymenttemplate").html()),
                columns: [{
                        field: "name",
                        title: "Name"
                    }, {
                        field: "email",
                        title: "Email"
                    }, {
                        field: "amount",
                        title: "Paid",
                        width: 100,
                        format: "{0:c}"
                    },{
                        field: '',
                        title: '',
                        template: '<button ng-click="vm.resend(#=id#)" class="btn btn-success btn-block"><em class="glyphicon glyphicon-send"></em>&nbsp;Resend Invitation</button>',
                        width: 210
                    },{
                        field: '',
                        title: '',
                        template: '<button ng-click="vm.remove()" class="btn btn-danger btn-block"><em class="glyphicon glyphicon-remove"></em>&nbsp;Remove</button>',
                        width: 120
                    }]
            };
          };

            vm.paymentDetailGridOptions = function(e) {

                var paymentapi = config.remoteApiName + 'widget/GetPaymentsByTeamMemberId/' + e.Id;

                vm.refund = function() {
                    alert('Refunding: ' + e.Id );
                    //datacontext.team.removeTeamMemberById(e.Id);
                    vm.teamgrid.refresh();

                };

                vm.resendReceipt = function() {
                    alert('Resending: ' + e.Id);
                };

                return {
                    dataSource: {
                        type: "json",
                        transport: {
                            read: paymentapi
                        },
                        serverPaging: false,
                        serverSorting: false,
                        serverFiltering: false,
                        pageSize: 5
                    },
                    sortable: true,
                    pageable: true,
                    columns: [{
                        field: "id",
                        title: "Confirmation Number"
                    }, {
                        field: "amount",
                        title: "Amount",
                        format: "{0:c}"
                    },{
                        field: '',
                        title: '',
                        template: '<button ng-click="vm.resendReceipt()" class="btn btn-success btn-block"><em class="glyphicon glyphicon-send"></em>&nbsp;Resend Receipt</button>',
                        width: 180
                    },{
                        field: '',
                        title: '',
                        template: '<button ng-click="vm.remove()" class="btn btn-danger btn-block"><em class="glyphicon glyphicon-usd"></em>&nbsp;Refund</button>',
                        width: 100
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
