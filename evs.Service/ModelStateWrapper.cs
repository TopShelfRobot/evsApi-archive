using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Service
{
    public class ModelStateWrapper : IValidationDictionary
    {

        //private ModelStateDictionary _modelState;

        //public ModelStateWrapper(ModelStateDictionary modelState)
        //{
        //    _modelState = modelState;
        //}

        #region IValidationDictionary Members

        public void AddError(string key, string errorMessage)
        {
            //_modelState.AddModelError(key, errorMessage);
        }

        public bool IsValid
        {
            //get { return _modelState.IsValid; }
            get { return true; }
        }

        #endregion
    }

    public static class ModelStateHelpers
    {

        //public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<RuleViolation> errors)
        //{

        //    foreach (RuleViolation issue in errors)
        //    {
        //        modelState.AddModelError(issue.PropertyName, issue.ErrorMessage);
        //    }
        //}
    }

    public class RuleViolation
    {
        public string ErrorMessage { get; private set; }
        public string PropertyName { get; private set; }

        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
