////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Web;

//using System;
//using System.Configuration;
//using System.Linq;
//using System.Web.Mvc;
//using System.Threading;
//using Microsoft.Web.WebPages.OAuth;
//using evs.DAL;

//namespace evs30Api.Controllers
//{
//    public class RegController : Controller
//    {
//        //
//        // GET: /Reg/

//        public ActionResult Index(string id)
//        {
            
//            ViewBag.owner = id;
//            ViewBag.part = Thread.CurrentPrincipal.Identity.Name;

//            var customName = ConfigurationManager.AppSettings["CustomName"];

//            if (OAuthWebSecurity.RegisteredClientData.Count == 0)
//            {
//                //var db = new evsContext();

//                //var ownerId = Convert.ToInt32(id);

//                //var owner = db.Owners.Where(o => o.Id == ownerId).SingleOrDefault();

//                ////check here to verify ownerId

//                // OAuthWebSecurity.RegisterFacebookClient(
//                //            appId: owner.FacebookAppId,
//                //            appSecret: owner.FacebookAppSecretId);
//                //ViewBag.logo = owner.LogoImageName;

//                //switch (customName)
//                //{
//                //    case "headfirst":
//                //        OAuthWebSecurity.RegisterFacebookClient(
//                //            appId: "602587876457110",
//                //            appSecret: "a30e71576b57f04e669a2f78ebaf6238");
//                //        break;
//                //    case "titan":
//                //        OAuthWebSecurity.RegisterFacebookClient(
//                //            appId: "692961870761574",
//                //            appSecret: "8bf0eb499af61e0b016fe1a0422c7718");
//                //        break;
//                //    default:
//                //        //localhost
//                //        OAuthWebSecurity.RegisterFacebookClient(
//                //            appId: "447816931997733",
//                //            appSecret: "ed72d01390147d4be82cd86e6d7106e4");
//                //        break;
//                //}
//            }

//            return View();
//        }


//    }
//}
