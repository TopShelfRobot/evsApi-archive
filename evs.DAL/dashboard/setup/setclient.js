define(['services/logger', 'services/datacontext','durandal/plugins/router', 'config', 'viewmodels/shared/debug'],  //, 'viewmodels/shared/debug'

    function (logger, datacontext, router, config) {

        var clients = ko.observableArray();
        var ResourceServices = ko.observableArray();
        var eventureServices = ko.observableArray();
        var selectedClient = ko.observable();

        var activate = function () {
            //logger.log('test activating.', null, 'test', true);
            //datacontext.getEventureServicesByEventureId(config.wizEventureId, eventureServices);
            //datacontext.getResourceServicesByOwnerId(config.ownerId, ResourceServices);
            //return datacontext.getClientsByOwnerId(config.ownerId, clients);
            
            return datacontext.getClientResourcesByOwnerId(config.ownerId,'Client', clients);
        };
        
        var clickNext = function () {
            //logger.log('next', null, 'test', true);
            save();
            var url = '#setlist';
            router.navigateTo(url);
        };
        var clickAddService = function() {
            var newService = ko.observable(datacontext.createEventureService(config.wizEventureId));
            //logger.log('still good ', null, 'sf', true);
            //newService().eventureListId(config.wizEventureListId);
            //newService().capacity(0);
            newService().active(true);
            newService().amount(0);
            //newService().sortOrder(groups().length + 1);
            eventureServices.push(newService);

        };
        
        var save = function () {
            //isSaving(true);
            //logger.log('called save', null, 'test', true);
            return datacontext.save()
                .fin(complete);

            function complete() {
                //isSaving(false);
                //logger.log('saved!', null, 'test', true);
            }
        };
        
        var viewAttached = function () {
            //logger.log('view attached', null, 'test', true);
           // bindEventToList(view, '.events', gotoDetails);
        };
        
       //var addItemToCartfromtest = function () {
        //    //logger.log('addingt', null, 'test', true);
        //    carttest.addItemToCart();
        //    //cartItems.push()
        //};

        var vm = {
            activate: activate,
            clients: clients,
            eventureServices: eventureServices,
            ResourceServices: ResourceServices,
            clickAddService: clickAddService,
            clickNext: clickNext,
            //addItemToCartfromtest: addItemToCartfromtest,
            selectedClient: selectedClient,
            viewAttached: viewAttached
        };
        return vm;
   });

