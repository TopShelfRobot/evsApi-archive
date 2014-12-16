(function () {
    'use strict';
    var controllerId = 'discounts';
    angular.module('app').controller(controllerId, ['config', 'common', 'datacontext', 'ExcelService', discounts]);

    function discounts(config, common, datacontext, excel) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;

        vm.title = 'app';

        vm.couponGridOptions = {};

        vm.ownerId = config.owner.ownerId;

        vm.owner = {};

        activate();

        function activate() {
            var promises = [couponGrid(), getOwner(), getAmountTypes()];
            common.activateController(promises, controllerId)
                .then(function () {
                    //log('Activated Coupon Addon Center View');
                });
        }

        function couponGrid() {

          var status = [{
            "value": true,
            "text": "Active",
          },{
            "value": false,
            "text": "Inactive"
          }];

          var couponapi = config.remoteApiName + 'widget/GetCouponsByOwnerId/' + config.owner.ownerId;
          vm.couponGridOptions = {
            //toolbar: '<a download="Coupons.xlsx" class="k-button" ng-click="vm.excel(vm.coupongrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
              toolbar: ['excel'],
              dataSource: {
                type: "json",
                transport: {
                    read: couponapi
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
                field: "code",
                title: "Coupon",
                width: "400px"
            },{
                field: "amount",
                title: "Amount",
                width: "220px"
            },{
                field: "active",
                width: "100px",
                values: status
            },{
                title: "",
                width: "120px",
                template:'<a class="btn btn-default btn-block" href="\\\#setcoupon/#=id#"><em class="glyphicon glyphicon-edit"></em>&nbsp;Edit</a>'
            }]
          };

          vm.detailGridOptions = function(e) {
            var couponuseapi = config.remoteApiName + 'widget/GetCouponUseByCouponId/' + e.Id;

            return {
                //toolbar: '<a download="detailexport.xlsx" class="k-button" ng-click="vm.detailexcel(vm.detailgrid)"><em class="glyphicon glyphicon-save"></em>&nbsp;Export</a>',
                toolbar: ['pdf', 'excel'],
                dataSource: {
                    type: "json",
                    transport: {
                        read: couponuseapi
                    },
                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 5
                },
                sortable: true,
                pageable: true,
                columns: [
                   {
                       field: "name",
                       title: "List",
                       width: 300
                   }, {
                       field: "amount",
                       title: "Amount",
                       format: "{0:c}",
                       width: 150
                   }, {
                       field: "description",
                       title: "Coupon",
                       width: 225
                   }, {
                       field: "firstName",
                       title: "First Name"
                   }, {
                       field: "lastName",
                       title: "Last Name"
                   }]
            };
          };
        }
        
        vm.excel = function(data) {
          var gridname = data;
          excel.export(gridname);
        };

        function getOwner() {
            return datacontext.participant.getOwnerById(vm.ownerId)
                .then(function(data) {
                   vm.owner = data;
                   return vm.owner;
                });
        }

        function getAmountTypes() {
            return datacontext.participant.getAmountTypes()
                .then(function(data) {
                    vm.amountTypes = data;
                    return vm.amountTypes;
                });
        }

    }
})();
