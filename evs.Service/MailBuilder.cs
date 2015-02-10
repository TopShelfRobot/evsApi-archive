using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace evs.Service
{
    public class MailBuilder
    {

         //readonly evsContext db = new evsContext();

        public MailBuilder()  //ModelStateDictionary modelState, IOrderRepository repository
        {
            // _modelState = modelState;
            //_repository = repository;

        }

        public string BuildResetPasswordBody(Int32 OwnerId)
        {
            string Body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("/Content/EmailTemplates/reset-password.html"));

            //var x = System.Web.

            //public static string appDataFolder = HttpContext.Current.Server.MapPath("~/App_Data/");
            return Body;
        }
    }
}
