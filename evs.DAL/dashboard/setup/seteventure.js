(function() {
	'use strict';

	var controllerId = 'seteventure';
	angular.module('app').controller(controllerId, ['$routeParams', '$upload', '$timeout', '$location', '$scope', 'common', 'datacontext', seteventure]);

	function seteventure($routeParams, $upload, $timeout, $location, $scope, common, datacontext) {

		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.title = 'Eventure';
		vm.eventureId = $routeParams.eventureId || 0;

		//log('val is: ' + vm.eventureId);

		vm.eventure = {};
		activate();

		function activate() {
			onDestroy();
			common.activateController(getEventure(), controllerId)
				.then(function() {
					//log('Activated set eventure');
				});
		}

		function getEventure() {

			if (vm.eventureId > 0) {
				return datacontext.eventure.getEventureById(vm.eventureId)
					.then(function(data) {
						//applyFilter();
						return vm.eventure = data;
					});
			} else {
				return vm.eventure = datacontext.eventure.createEventure();
			}
		}
      
        vm.cancel = function() {
			$location.path("/eventurecenter");
        };

		function onDestroy() {
			//alert('destroy my contextttttttt!!!!');
			$scope.$on('$destroy', function () {
				//alert('destroymy contextttttttt!!!!!!!');
				//autoStoreWip(true);
				datacontext.cancel();
			});
		}

        vm.saveAndNav = function() {
            return datacontext.save(vm.eventure)
            .then(complete);

                function complete() {
                    $location.path("/eventurecenter");
                }
        };

		vm.today = function () {
			vm.eventure.dateEventure = new Date();
			vm.eventure.dateTransfer = new Date();
			vm.eventure.dateDeferral = new Date();
		};

		vm.today();

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

		//File Upload
		vm.fileReaderSupported = window.FileReader !== null;
		vm.uploadRightAway = true;
		vm.changeAngularVersion = function() {
			window.location.hash = vm.angularVersion;
			window.location.reload(true);
		};
		vm.hasUploader = function(index) {
			return vm.upload[index] !== null;
		};
		vm.abort = function(index) {
			vm.upload[index].abort();
			vm.upload[index] = null;
		};
		vm.angularVersion = window.location.hash.length > 1 ? window.location.hash.substring(1) : '1.2.0';
		vm.onFileSelect = function($files) {
			vm.selectedFiles = [];
			vm.progress = [];
			if (vm.upload && vm.upload.length > 0) {
				for (var i = 0; i < vm.upload.length; i++) {
					if (vm.upload[i] !== null) {
						vm.upload[i].abort();
					}
				}
			}
			vm.upload = [];
			vm.uploadResult = [];
			vm.selectedFiles = $files;
			vm.dataUrls = [];
			for (var i = 0; i < $files.length; i++) {
				var $file = $files[i];
				if (window.FileReader && $file.type.indexOf('image') > -1) {
					var fileReader = new FileReader();
					fileReader.readAsDataURL($files[i]);
					var loadFile = function(fileReader, index) {
						fileReader.onload = function(e) {
							$timeout(function() {
								vm.dataUrls[index] = e.target.result;
							});
						};
					}(fileReader, i);
				}
				vm.progress[i] = -1;
				if (vm.uploadRightAway) {
					vm.start(i);
				}
			}
		};

		vm.start = function(index) {
			vm.progress[index] = 0;
			vm.errorMsg = null;
			if (vm.howToSend == 1) {
				vm.upload[index] = $upload.upload({
					url: '/Content/images',
					method: PUT,
					headers: {
						'my-header': 'my-header-value'
					},
					data: {
						myModel: vm.myModel
					},
					/* formDataAppender: function(fd, key, val) {
                  if (angular.isArray(val)) {
                                angular.forEach(val, function(v) {
                                  fd.append(key, v);
                                });
                              } else {
                                fd.append(key, val);
                              }
                }, */
					/* transformRequest: [function(val, h) {
                  console.log(val, h('my-header')); return val + 'aaaaa';
                }], */
					file: vm.selectedFiles[index],
					fileFormDataName: 'myFile'
				}).then(function(response) {
					vm.uploadResult.push(response.data);
				}, function(response) {
					if (response.status > 0) vm.errorMsg = response.status + ': ' + response.data;
				}, function(evt) {
					// Math.min is to fix IE which reports 200% sometimes
					vm.progress[index] = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
				}).xhr(function(xhr) {
					xhr.upload.addEventListener('abort', function() {
						console.log('abort complete')
					}, false);
				});
			} else {
				var fileReader = new FileReader();
				fileReader.onload = function(e) {
					vm.upload[index] = $upload.http({
						url: '/Content/images',
						headers: {
							'Content-Type': vm.selectedFiles[index].type
						},
						data: e.target.result
					}).then(function(response) {
						vm.uploadResult.push(response.data);
					}, function(response) {
						if (response.status > 0) vm.errorMsg = response.status + ': ' + response.data;
					}, function(evt) {
						// Math.min is to fix IE which reports 200% sometimes
						vm.progress[index] = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
					});
				};
				fileReader.readAsArrayBuffer(vm.selectedFiles[index]);
			}
		};

		vm.resetInputFile = function() {
			var elems = document.getElementsByTagName('input');
			for (var i = 0; i < elems.length; i++) {
				if (elems[i].type == 'file') {
					elems[i].value = null;
				}
			}
		};

	}
})();

//define(['services/logger', 'services/datacontext', 'durandal/plugins/router', 'config'],  //, 'viewmodels/shared/debug'

//    function (logger, datacontext, router, config) {

//        var eventure = ko.observable();
//        var eventureId;

//        var activate = function (routeData) {
//            logger.log('test activating.', null, 'ec', true);

//            //$('#header').addClass('hidden');

//            eventureId = parseInt(routeData.id);
//            logger.log('test activating.' + eventureId, null, 'ec', true);
//            if (isNaN(eventureId))   //
//                config.wizard = true;     //we took this page out of the wizard but wizard=true still indicates a new event
//            else
//                config.wizard = false;

//            if (config.wizard)
//                return eventure(datacontext.createEventure());
//            else
//                return datacontext.getEventureById(eventureId, eventure);  //mjb this is wrong should be id passed in

//        };


//        //var clickNext = function () {
//        //    //logger.log('next', null, 'seteventure', true);
//        //    eventure().ownerId(config.ownerId);

//        //    var form = $(".form-horizontal");
//        //    form.validate();

//        //    if (form.valid())
//        //        saveAndNav();
//        //};




//        var vm = {
//            activate: activate,
//            eventure: eventure,
//            clickNext: clickNext,
//            viewAttached: viewAttached
//        };
//        return vm;
//    });
