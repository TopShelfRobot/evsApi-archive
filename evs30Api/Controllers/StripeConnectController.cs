using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Stripe;
using evs.DAL;
using evs.Model;
//using Newtonsoft.Json;
using System.Configuration;

namespace evs30Api.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class StripeConnectController : ApiController
    {
        private evsContext db = new evsContext();

        [HttpPost]
        //[AllowAnonymous]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Post(JObject stripBundle)
        {
            //registration.EventureListId = regBundle.eventureListId;
            //registration.ParticipantId = regBundle.partId; //IsNullOrEmpty(planRec.approved_by) ? "" : planRec.approved_by.toString();
            //registration.Name = regBundle.displayList;

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        //[AllowAnonymous]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage StripeResponse()  //Int32 OwnerId
        {
            try
            {
                var log = new EventureLog();
                log.Message = "So far so good";
                log.Caller = "StripeResponse";
                log.Status = "Info";
                log.LogDate = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(log);
                db.SaveChanges();

                var queryValue = Request.RequestUri.ParseQueryString();

                if (queryValue["error_description"] != null)
                {
                    var logE = new EventureLog();
                    logE.Message = "Error Stripe Call 1: " + queryValue["error_description"];
                    logE.Caller = "StripeResponse_ERROR";
                    logE.Status = "ERROR";
                    logE.LogDate = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(logE);
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                string code = queryValue["code"];

                log.Message = "got the code: " + code;
                log.Caller = "StripeResponse";
                log.Status = "Info";
                log.LogDate = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(log);
                db.SaveChanges();

                var stripeService = new StripeOAuthTokenService(ConfigurationManager.AppSettings["StripeSecret"]);
                var stripeTokenOptions = new StripeOAuthTokenCreateOptions() { Code = code, GrantType = "authorization_code" };
                var response = stripeService.Create(stripeTokenOptions);

                log.Caller = "StripeResponse";
                log.Status = "Info";
                log.LogDate = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(log);
                db.SaveChanges();

                if (response.Error != null)
                {
                    var logE = new EventureLog();
                    logE.Message = "Error Stripe Call 2: " + response.ErrorDescription;
                    logE.Caller = "StripeResponse_ERROR";
                    logE.Status = "ERROR";
                    logE.LogDate = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(logE);
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.PaymentRequired);
                }

                var OwnerId = 1;  //need to pass this in from profile   mjb
                var owner = db.Owners.FirstOrDefault(o => o.Id == OwnerId);
                owner.AccessToken = response.AccessToken;
                owner.RefreshToken = response.RefreshToken;
                owner.StripePublishableKey = response.PublishableKey;
                owner.StripeUserId = response.StripeUserId;
                owner.Livemode = response.LiveMode;
                owner.Scope = response.Scope;

                db.SaveChanges();

               return Request.CreateResponse(HttpStatusCode.OK);

               //var resp = Request.CreateResponse(HttpStatusCode.Moved);     //would be cool to call owner again to show changes;  might be easier for javascript
               //resp.Headers.Location = new Uri("http://www.google.com");
               //return resp;
            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "Error Handler: " + ex.Message;
                logE.Caller = "StripeResponse_ERROR";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }


    }
}