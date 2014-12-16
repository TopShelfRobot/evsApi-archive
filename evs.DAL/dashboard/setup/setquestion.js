;(function(){
	
	var questionKeys = [
		"questionText",
		"questionType",
		"active",
		"isRequired",
		"options",
		"order"
	];
	
	
	function Controller($scope, $routeParams, $location, datacontext){
		var self = this;
		
		var listId = $routeParams.listId;
		
		this.activeQuestion = null;
		this.workingQuestion = {};
		this.isEdit = false;
		
		var loadCustomQuestions = function(){
			return datacontext.question.getCustomQuestionSetByEventureListId(listId)
				.then(function(data){
					console.log("custom questions:", data);
				
					self.customQuestions = data;
					return self.customQuestions;
				});
		};
		
		onDestroy();
		loadCustomQuestions();
		
		this.editQuestion = function(id){
			self.isEdit = true;
			self.activeQuestion = null;
			for(var j = 0; j < self.customQuestions.length; j++){
				if(self.customQuestions[j].id == id){
					self.activeQuestion = j;
					break;
				}
			}
			if(self.activeQuestion !== null){
				for(var i = 0; i < questionKeys.length; i++){
					self.workingQuestion[questionKeys[i]] = self.customQuestions[self.activeQuestion][questionKeys[i]];
				}
			}
		};
		
		this.clearEdits = function(){
			self.isEdit = false;
			self.activeQuestion = null;
			self.workingQuestion = {};
		};
			
		this.saveQuestion = function(){
			self.isEdit = false;
			var question;
			if(self.activeQuestion == null){
				question = datacontext.question.createCustomQuestion(listId);
			}else{
				question = self.customQuestions[self.activeQuestion];
			}
			for(var key in self.workingQuestion){
				question[key] = self.workingQuestion[key];
			}
			datacontext.save()
				.then(function(data){
					self.clearEdits();
					console.log("save successful:", data);
					return loadCustomQuestions();
				}).catch(function(err){
					console.error("save unsuccessful:", err);
					return loadCustomQuestions();
				});
		}

		function onDestroy() {
		    $scope.$on('$destroy', function () {
		        datacontext.cancel();
		    });
		}
		
		this.saveAndNav = function(){
			datacontext.save()
			.then(function(){
				$location.path("/elistcenter/" + listId);
			}).catch(function(){
				console.log("save failed");
			});
		}
	}
	
	angular.module("app").controller("SetQuestion", ['$scope', '$routeParams', '$location', 'datacontext', Controller]);
})();