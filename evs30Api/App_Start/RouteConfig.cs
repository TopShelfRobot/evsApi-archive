using System.Web.Mvc;
using System.Web.Routing;

namespace evs30Api.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


           // routes.MapRoute(
           //    name: "manreg",
           //    url: "manreg/{id}/{email}",
           //    defaults: new { controller = "Manreg", action = "Index" }
           //);

           // routes.MapRoute(
           //      name: "reg",
           //      url: "reg/{id}",
           //      defaults: new { controller = "Reg", action = "Index" }
           //  );


            // config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

          //  routes.MapRoute(
          //    name: "kendo",
          //    url: "kendo/{controller}/{id}" //{action}/{id}",
          //      //defaults: new { action = UrlParameter.Optional, id = UrlParameter.Optional }
          //);

         //   routes.MapRoute(
         //    name: "dash",
         //    url: "dash/{id}",
         //    defaults: new { controller = "Dash", action = "Index" }
         //);




            //  routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
             name: "Payment",
             url: "api/Payment/{action}",
             defaults: new { controller = "Payment" }
              );

            routes.MapRoute(
            name: "Transfer",
            url: "api/Registrations/Transfer",
            defaults: new { controller = "Registrations", action = "Transfer" }
        );

            routes.MapRoute(
          name: "dataconv",
          url: "api/Account/{id}",
          defaults: new { controller = "Payment", action = "Post", id = UrlParameter.Optional }
      );

            //  routes.MapRoute(
            //    name: "ControllerOnly",
            //    url: "xlt/{controller}",
            //    defaults: new { controller = "Payment", action = "Index" }
            //);

            // routes.MapRoute(
            //    name: "pay",
            //    url: "pay/buy/",
            //    defaults: new { controller = "Payment", action = "Pay", eid = UrlParameter.Optional, pid = UrlParameter.Optional }
            //);

          //  routes.MapRoute(
          //    name: "es",
          //    url: "es/{pid}",
          //    defaults: new { controller = "Reg", action = "es", pid = UrlParameter.Optional }
          //);

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

           // routes.MapRoute(
           //    name: "stripeconnect",
           //    url: "api/stripeConnect/StripeResponse/{id}",
           //    defaults: new { controller = "stripeConnect", action = "StripeResponse" }
           //);

            routes.MapRoute(
                name: "mail",
                url: "api/mail/{action}",
                defaults: new { controller = "Mail", action = "Mail" }
            );

         //   GlobalConfiguration.Configuration.Routes.MapHttpRoute(
         //name: "BreezeApi",
         //routeTemplate: "api/{controller}/{action}"
     //);


        }
    }
}