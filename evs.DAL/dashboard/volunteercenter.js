(function () {
    'use strict';
    var controllerId = 'volunteercenter';
    angular.module('app').controller(controllerId, ['common', 'datacontext', 'config', 'ExcelService', volunteercenter]);

    function volunteercenter(common, datacontext, config, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.participantGridOptions = {};

        vm.ownerId = 1;


        activate();

        function activate() {
            common.activateController(volunteerGrid(), controllerId)
                .then(function () {
                    //log('Activated Team Center View');
                });
        }

        function volunteerGrid() {

          var volunteerapi = config.remoteApiName + 'widget/GetVolunteersByOwnerId/' + vm.ownerId;

          vm.volunteerGridOptions = {
            //toolbar: '<a download="Teams.xlsx" class="k-button" ng-click="vm.excel(vm.volunteergrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              excel: {
                  fileName: 'Volunteers.xlsx',
                  filterable: true
              },
              dataSource: {
                type: "json",
                transport: {
                    read: volunteerapi
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
                    width: 225
                },
                    {
                        field: "lastName",
                        title: "Last Name",
                        width: 225
                    }, {
                        field: "email",
                        title: "Email Address",
                        width: 300
                    }, {
                        field: "phoneMobile",
                        title: "Phone",
                        width: 300,
                        filterable: false
                    }]
          };

          vm.detailGridOptions = function(e) {

            var volunteerapi = config.remoteApiName + 'widget/GetVolunteerScheduleByVolunteerId' + e.Id;
            
            return {
                //toolbar: '<a download="detailexport.xlsx" class="k-button" ng-click="vm.excel(vm.detailgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
                toolbar: ['excel'],
                excel: {
                    fileName: 'Volunteer Schedule.xlsx',
                    filterable: true
                },
                dataSource: {
                    type: "json",
                    transport: {
                        read: volunteerapi
                    },
                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 5
                },
                schema: {
                  model: {
                    fields: {
                        TimeBegin: { type: "date" },
                        TimeEnd: { type: "date" }
                    }
                  }
                },
                sortable: true,
                pageable: true,
                columns: [{
                        field: "jobName",
                        title: "Job Name",
                        width: 200
                   }, {
                       field: "eventName",
                       title: "Event",
                       width: 300
                    }, {
                        field: "timeBegin",
                        title: "Shift Begin",
                        type: "date",
                        //format: "{0:h:mm tt}",
                        template: "#=moment(TimeBegin).format('h:mm a')#",
                        width: 125
                    }, {
                        field: "timeEnd",
                        title: "Shift End",
                        type: "date",
                        format: "{0:h:mm tt}"
                        , width: 125
                    }, {
                        title: "",
                        width: "120px",
                        template:'<a class="btn btn-default btn-block" href="\\\#/volunteerscheduleedit/#=Id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
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
