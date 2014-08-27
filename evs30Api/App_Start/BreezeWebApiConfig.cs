using System.Web.Http;
using evs30Api.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(evs30Api.App_Start.BreezeWebApiConfig), "RegisterBreezePreStart")]
namespace evs30Api.App_Start
{
    ///<summary>
    /// Inserts the Breeze Web API controller route at the front of all Web API routes
    ///</summary>
    ///<remarks>
    /// This class is discovered and run during startup; see
    /// http://blogs.msdn.com/b/davidebb/archive/2010/10/11/light-up-your-nupacks-with-startup-code-and-webactivator.aspx
    ///</remarks>
    public static class BreezeWebApiConfig
    {

        public static void RegisterBreezePreStart()
        {
            //GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            //      name: "BreezeCouponApi",
            //      routeTemplate: "breeze/ValidateCoupon{test}"
            //     //defaults: new { couponCode = RouteParameter.Optional, particpantId = RouteParameter.Optional, eventureListId = RouteParameter.Optional }
            //  );

            //GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            //    name: "BreezeCouponApi",
            //    routeTemplate: "breeze/breeze/ValidateCoupon/{test}",
            //    defaults: new { test = RouteParameter.Optional }
            //    //defaults: new { couponCode = RouteParameter.Optional, particpantId = RouteParameter.Optional, eventureListId = RouteParameter.Optional }
            //);

            //GlobalConfiguration.Configuration.

            GlobalConfiguration.Configuration.MessageHandlers.Add(new BreezeSimpleCorsHandler());   

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "BreezeApi",
                routeTemplate: "breeze/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                      name: "pay",
                      routeTemplate: "api/{controller}/{action}"
                  );

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                    name: "team",
                    routeTemplate: "api/Payment/PostTeam"
                );

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                     name: "kendoApi",
                     routeTemplate: "kendo/{controller}/{action}/{id}",
                     defaults: new { id = RouteParameter.Optional }
                 );
        }
    }
}