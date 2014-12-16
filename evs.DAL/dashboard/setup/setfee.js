(function() {
    'use strict';

    var controllerId = 'setfee';
    angular.module('app').controller(controllerId, ['$routeParams', '$location', '$scope', 'common', 'datacontext', 'config', setfee]);

    function setfee($routeParams, $location, $scope, common, datacontext, config) {

        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Fee Setup';
        vm.listId = $routeParams.listId || 0;
        vm.eventureId = $routeParams.eventureId || 0;

        vm.ownerId = config.owner.ownerId;

        vm.fees = [];
        vm.groups = [];
        vm.listing = {};

        activate();

        function activate() {
            onDestroy();
            common.activateController(getFees(), getGroups(), getEventureList(), controllerId)
                .then(function() {
                    //log('Activated set addon');
                });
        }

        function getFees() {
            return datacontext.surcharge.getFeeSchedulesByEventureListId(vm.listId)
                .then(function(data) {
                    //applyFilter();
                    return vm.fees = data;
                });
        }

        function getGroups() {
            return datacontext.eventure.getGroupsByEventureListId(vm.listId)
                .then(function(data) {
                    //applyFilter();
                    return vm.groups = data;
                });
        }

        function getEventureList() {
            return datacontext.eventure.getEventureListById(vm.listId)
                .then(function(data) {
                    //applyFilter();
                    return vm.listing = data;
                });
        }

        vm.addFee = function() {
            vm.newFee = datacontext.surcharge.createFeeSchedule(vm.listId);
            vm.fees.push(vm.newFee);
        };

        vm.addGroup = function() {
            vm.newGroup = datacontext.eventure.createGroup(vm.listId);
            vm.groups.push(vm.newGroup);
        };

        vm.open = function($event, open) {
            $event.preventDefault();
            $event.stopPropagation();
            vm[open] = true;
        };

        vm.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };

        vm.formats = ['MM-dd-yyyy', 'yyyy/MM/dd', 'shortDate'];

        vm.format = vm.formats[0];

        function onDestroy() {
            $scope.$on('$destroy', function () {
                //autoStoreWip(true);
                datacontext.cancel();
            });
        }

        vm.saveAndNav = function() {
            return datacontext.save()
                .then(complete);

            function complete() {
                $location.path("/" + vm.eventureId + "/" + vm.listId + "/setquestion");
            }
        };

    }
})();



// ﻿define(['services/logger', 'services/datacontext', 'durandal/plugins/router', 'config'],
//
//     function (logger, datacontext, router, config) {
//
//         var fees = ko.observableArray();
//         var groups = ko.observableArray();
//         var sortOrder = 0;
//
//         var activate = function () {
//             //logger.log('test activating.' + config.wizEventureListId, null, 'sfee', true);
//             datacontext.getGroupsByEventureListId(config.wizEventureListId, groups)
//                 .then(function () {
//                     //logger.log('groups: ' + groups().length(), null, 'sfee', true);
//                 });
//             return datacontext.getFeeSchedulesByEventureListId(config.wizEventureListId, fees);
//         };
//
//         var viewAttached = function () {
//             //logger.log('view attached', null, 'setfee', true);
//             $(".feedate").datepicker();
//             //$("#feedate").datepicker();
//             //$(".feeamount").maskMoney({ symbol: '$ ', thousands: ',', decimal: '.', symbolStay: true });
//         };
//
//         var clickAddGroup = function () {
//             //logger.log('trying to add group ', null, 'sf', true);
//             //logger.log('groups: ' + groups().length, null, 'sfee', true);
//             var newGroup = ko.observable(datacontext.createGroup());
//             //logger.log('still good ', null, 'sf', true);
//             newGroup().eventureListId(config.wizEventureListId);
//             newGroup().capacity(0);
//             newGroup().active(true);
//             newGroup().name("");
//             newGroup().sortOrder(groups().length + 1);
//             groups.push(newGroup);
//         };
//
//         var clickAddFee = function () {
//             //logger.log('', null, 'sf', true);
//             var newFee = ko.observable(datacontext.createFeeSchedule());
//
//             newFee().eventureListId(config.wizEventureListId);
//             newFee().amount(0);
//             newFee().dateBegin("");
//             fees.push(newFee);
//             //fees.push({ amount: 0, dateBegin: "", active: true, eventureListId: config.wizEventureListId });
//
//             $(".feedate").datepicker();
//             //$(".feeamount").maskMoney({ symbol: '$ ', thousands: ',', decimal: '.', symbolStay: true });
//
//         };
//
//         var clickNext = function () {
//             //logger.log('next to quest', null, 'sf', true);
//             saveAndNav();
//         };
//
//         var saveAndNav = function () {
//             //isSaving(true);
//             //logger.log('called saveeeee', null, 'sf', true);
//             return datacontext.save(fees)
//                 .fin(complete);
//
//             function complete() {
//                 //isSaving(false);
//                 //logger.log('save complete', null, 'sl', true);
//                 var url = '#setquestion'; //+ eventureList().id();
//                 router.navigateTo(url);
//             }
//         };
//
//
//         var vm = {
//             activate: activate,
//             fees: fees,
//             groups: groups,
//             clickNext: clickNext,
//             clickAddGroup: clickAddGroup,
//             clickAddFee: clickAddFee,
//             viewAttached: viewAttached
//         };
//         return vm;
//     });
//
