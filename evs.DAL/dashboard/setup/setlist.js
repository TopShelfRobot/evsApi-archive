(function() {
	'use strict';

	var controllerId = 'setlist';
	angular.module('app').controller(controllerId, ['$routeParams', '$upload', '$timeout', '$location', '$scope', 'common', 'datacontext', 'config', setlist]);

	function setlist($routeParams, $upload, $timeout, $location, $scope, common, datacontext, config) {

		var getLogFn = common.logger.getLogFn;
		var log = getLogFn(controllerId);

		var vm = this;
		vm.title = 'Eventure Listing';
		vm.listId = $routeParams.listId || 0;
		vm.eventureId = $routeParams.eventureId;
	    vm.ownerId = config.owner.ownerId;

	    vm.list = {};
	    vm.listTypes = [];
	    activate();

		function activate() {
			onDestroy();
		    common.activateController(getEventureList(), getListTypes(), controllerId)
				.then(function() {
				    //log('Activated list typessssss');
				    //console.log(vm.listTypes);
				    //console.log(vm.list);
				});
		}

		function getListTypes() {
		    return datacontext.eventure.getEventureListTypesByOwnerId(vm.ownerId)
					.then(function (data) {
					    vm.listTypes = data;
					    return vm.listTypes;
					});
	    }

		function getEventureList() {

			if (vm.listId > 0) {
				return datacontext.eventure.getEventureListById(vm.listId)
					.then(function(data) {
						vm.list = data;
						getEventureListsByEventureId(vm.list.eventureId);
						return vm.list;
					});
			} else {
				vm.list = datacontext.eventure.createEventureList();
				vm.list.eventureId = vm.eventureId;
				getEventureListsByEventureId(vm.list.eventureId);
				return vm.list;
			}
		}

		function getEventureListsByEventureId() {
			return datacontext.eventure.getEventureListsByEventureId(vm.list.eventureId)
				.then(function(data) {
					multiSelect(data);
				});
		}

		function multiSelect(listings) {
			// vm.multiSelect.value(listings);
			for(var i = 0; i < listings.length; i++){
				vm.multiSelect.dataSource.add({name: listings[i].name, id: listings[i].id});
			}
		}

		vm.selectedLists = [];
		vm.bundledListOptions = {
			placeholder: 'Select listing...',
			dataTextField: 'name',
			dataValueField: 'id'
		};

		vm.today = function () {
		   vm.list.dateEventureList = new Date();
           vm.list.dateBeginReg = new Date();
           vm.list.dateEndReg = new Date();
		};

		vm.today();

		vm.open = function($event, open) {
			$event.preventDefault();
			$event.stopPropagation();
			vm[open] = true;
		};

		vm.dateOptions = {
			'year-format': '"yy"',
			'starting-day': 1
		};

		vm.formats = ['MM-dd-yyyy', 'yyyy/MM/dd', 'shortDate'];

		vm.format = vm.formats[0];

		//File Upload
		//vm.fileReaderSupported = window.FileReader != null;
		//vm.uploadRightAway = true;
		//vm.changeAngularVersion = function() {
		//	window.location.hash = vm.angularVersion;
		//	window.location.reload(true);
		//};
		//vm.hasUploader = function(index) {
		//	return vm.upload[index] != null;
		//};
		//vm.abort = function(index) {
		//	vm.upload[index].abort();
		//	vm.upload[index] = null;
		//};
		//vm.angularVersion = window.location.hash.length > 1 ? window.location.hash.substring(1) : '1.2.0';
		//vm.onFileSelect = function($files) {
		//	vm.selectedFiles = [];
		//	vm.progress = [];
		//	if (vm.upload && vm.upload.length > 0) {
		//		for (var i = 0; i < vm.upload.length; i++) {
		//			if (vm.upload[i] != null) {
		//				vm.upload[i].abort();
		//			}
		//		}
		//	}
		//	vm.upload = [];
		//	vm.uploadResult = [];
		//	vm.selectedFiles = $files;
		//	vm.dataUrls = [];
		//	for (var i = 0; i < $files.length; i++) {
		//		var $file = $files[i];
		//		if (window.FileReader && $file.type.indexOf('image') > -1) {
		//			var fileReader = new FileReader();
		//			fileReader.readAsDataURL($files[i]);
		//			var loadFile = function(fileReader, index) {
		//				fileReader.onload = function(e) {
		//					$timeout(function() {
		//						vm.dataUrls[index] = e.target.result;
		//					});
		//				}
		//			}(fileReader, i);
		//		}
		//		vm.progress[i] = -1;
		//		if (vm.uploadRightAway) {
		//			vm.start(i);
		//		}
		//	}
		//};
        //
		//vm.start = function(index) {
		//	vm.progress[index] = 0;
		//	vm.errorMsg = null;
		//	if (vm.howToSend == 1) {
		//		vm.upload[index] = $upload.upload({
		//			url: '/Content/images',
		//			method: PUT,
		//			headers: {
		//				'my-header': 'my-header-value'
		//			},
		//			data: {
		//				myModel: vm.myModel
		//			},
		//			/* formDataAppender: function(fd, key, val) {
         //         if (angular.isArray(val)) {
         //                       angular.forEach(val, function(v) {
         //                         fd.append(key, v);
         //                       });
         //                     } else {
         //                       fd.append(key, val);
         //                     }
         //       }, */
		//			/* transformRequest: [function(val, h) {
         //         console.log(val, h('my-header')); return val + 'aaaaa';
         //       }], */
		//			file: vm.selectedFiles[index],
		//			fileFormDataName: 'myFile'
		//		}).then(function(response) {
		//			vm.uploadResult.push(response.data);
		//		}, function(response) {
		//			if (response.status > 0) vm.errorMsg = response.status + ': ' + response.data;
		//		}, function(evt) {
		//			// Math.min is to fix IE which reports 200% sometimes
		//			vm.progress[index] = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
		//		}).xhr(function(xhr) {
		//			xhr.upload.addEventListener('abort', function() {
		//				console.log('abort complete')
		//			}, false);
		//		});
		//	} else {
		//		var fileReader = new FileReader();
		//		fileReader.onload = function(e) {
		//			vm.upload[index] = $upload.http({
		//				url: '/Content/images',
		//				headers: {
		//					'Content-Type': vm.selectedFiles[index].type
		//				},
		//				data: e.target.result
		//			}).then(function(response) {
		//				vm.uploadResult.push(response.data);
		//			}, function(response) {
		//				if (response.status > 0) vm.errorMsg = response.status + ': ' + response.data;
		//			}, function(evt) {
		//				// Math.min is to fix IE which reports 200% sometimes
		//				vm.progress[index] = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
		//			});
		//		}
		//		fileReader.readAsArrayBuffer(vm.selectedFiles[index]);
		//	}
		//};
        //
		//vm.resetInputFile = function() {
		//	var elems = document.getElementsByTagName('input');
		//	for (var i = 0; i < elems.length; i++) {
		//		if (elems[i].type == 'file') {
		//			elems[i].value = null;
		//		}
		//	}
		//};
      
        vm.cancel = function() {
			$location.path('/eventuredetail/' + vm.list.eventureId);
        };

		function onDestroy() {
			$scope.$on('$destroy', function () {
				datacontext.cancel();
			});
		}

		vm.saveAndNav = function() {

			return datacontext.save(vm.list)
				.then(complete);

			function complete() {
				$location.path('/' + vm.list.eventureId + '/' + vm.list.id + '/setfee/');
			}
		};

	}
})();