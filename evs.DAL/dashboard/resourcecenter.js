(function () {
    'use strict';
    var controllerId = 'resourcecenter';
    angular.module('app').controller(controllerId, ['common', 'config', 'ExcelService', resourcecenter]);

    function resourcecenter(common, config, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'app';
        vm.ownerId = config.owner.ownerId;

        activate();

        function activate() {
            //var promises = [getMessageCount(), getPeople()];
            common.activateController(createResourceGrid(), controllerId)
                .then(function() { 
                  //log('Activated reporting View'); 
                });
        }

        function createResourceGrid() {

            //var resourceApi = '/kendo/Resources/GetResourcesByOwnerId/' + vm.ownerId;
            var resourceApi = config.remoteApiName + 'widget/GetResourcesByOwnerId/' + vm.ownerId;

            vm.resourceGridOptions = {
              //toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.resourcegrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
                toolbar: ['excel'],
                excel: {
                    fileName: 'Resources.xlsx',
                    filterable: true
                },
                dataSource: {
                    type: "json",
                    transport: {
                        read: resourceApi
                    },
                    pageSize: 6,
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
                columns: [//{ field: "Name", title: "Resource", width: "325px" },
                    { field: "name", title: "Resource", width: "325px" },
                    { field: "email", title: "Email", width: "225px" },
                    { field: "phone", title: "Phone", width: "175px", filterable: false },
                    { field: "resourceType", title: "Type", width: "175px" },
                    { title: "", width: "120px", template: '<a class="btn btn-default btn-block" href="\\\#resourcedetail/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
            }
                ]
            };
          
            vm.detailGridOptions = function(e) {
                var resourceApi = config.remoteApiName + 'widget/GetResourceItemsByResourceId/' + e.id;

                return {
                  toolbar: '<a download="download.xlsx" class="k-button" ng-click="vm.excel(vm.detailgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
                    dataSource: {
                        type: "json",
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
                    dataBound: function() {
                    },
                    columns: [
                        { field: "name", title: "Item Name", width: "150px" },
                        { field: "cost", title: "Cost", width: "70px", format: "{0:c}" },
                        { field: "category", title: "Category", width: "100px" },
                        { title: "", width: 100, template: '<a class="btn btn-default btn-block" href="\\\#setresourceitem/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>' }
                    ]
                };
            };
        }
      
        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };
    }
})();
