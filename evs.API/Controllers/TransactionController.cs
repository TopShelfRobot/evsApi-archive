using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using evs.DAL;
using evs.Model;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Text;
using evs.Service;


namespace evs.API.Controllers
{
    [RoutePrefix("api/transaction")]
    public class TransactionController : ApiController
    {
        private evsContext db = new evsContext();

        //[AllowAnonymous]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        [HttpPost]
        [Route("transfer")]
        public HttpResponseMessage Transfer(JObject saveBundle)
        {
            try
            {
                //int numOfRegs = 0;
                //decimal totalFees = 0;

                var transferId = (Int32)saveBundle["transferId"];
                var transferNewListName = (string)saveBundle["transferNewListName"];
                var participantId = (Int32)saveBundle["partId"];

                //var log = new EventureLog();
                //log.Message = "starting transfer: " + transferId;
                //log.Caller = "transfer";
                //log.Status = "Info";
                //log.LogDate = System.DateTime.Now.ToLocalTime();
                //log.DateCreated = System.DateTime.Now.ToLocalTime();
                //db.EventureLogs.Add(log);
                //db.SaveChanges();

                var transfer = db.Transfers.Where(t => t.Id == transferId).Single();

                var order = new EventureOrder
                {
                    DateCreated = DateTime.Now,
                    HouseId = (Int32)saveBundle["houseId"],
                    Amount = (Decimal)saveBundle["amount"],
                    Token = (string)saveBundle["token"],   //is this safe??
                    OwnerId = (Int32)saveBundle["ownerId"],
                    Status = "init transfer",
                    Voided = false
                };
                db.Orders.Add(order);

                //Owner owner= db.Owners.Where(o => o.Id == order.OwnerId).SingleOrDefault();
                Owner owner = db.Owners.SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }

                //validate
                //must have transferId,
                //i could process without partId  just no email

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                string custDesc = string.Empty;
                string partEmail = string.Empty;
                var part = db.Participants.Where(p => p.Id == participantId).FirstOrDefault();
                if (part != null)
                {
                    custDesc = "_transfer" + transferId;
                    partEmail = part.Email;
                }
                else
                {
                    //this should never happen  throw exception?
                    //NO house Id
                    throw new Exception("There was an issue with submission, Not signed into account.");
                }

                // create customer
                var customerOptions = new StripeCustomerCreateOptions
                {
                    Email = partEmail,
                    Description = custDesc,
                    TokenId = order.Token,
                };

                var stripeCustomerService = new StripeCustomerService(owner.AccessToken);
                var customer = stripeCustomerService.Create(customerOptions);

                var stripeChargeService = new StripeChargeService(owner.AccessToken); //The token returned from the above method
                var stripeChargeOption = new StripeChargeCreateOptions()
                {
                    //AmountInCents = Convert.ToInt32(order.Amount * 100),
                    Amount = Convert.ToInt32(order.Amount * 100),
                    Currency = "usd",
                    CustomerId = customer.Id,
                    Description = owner.Name,   //this needs to be dynamic
                    ApplicationFee = order.LocalApplicationFee
                    //ApplicationFeeInCents = order.LocalApplicationFee
                };
                var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                {
                    // update reg
                    var reg = db.Registrations.Where(r => r.Id == transfer.RegistrationId).Single();

                    reg.EventureListId = transfer.EventureListIdTo;
                    // mjb 060914  reg.Name = transferNewListName;
                    reg.Type = "xferup";
                    transfer.IsComplete = true;

                    order.AuthorizationCode = stripeCharge.Id;
                    //stripeCharge.
                    order.CardNumber = stripeCharge.StripeCard.Last4;
                    order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                    order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                    order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                    //order.CardId = stripeCharge.StripeCard.;
                    order.CardName = stripeCharge.StripeCard.Name;
                    order.CardOrigin = stripeCharge.StripeCard.Country;
                    //mjb order.CardType = stripeCharge.StripeCard.;
                    order.Voided = false;
                    order.Status = "Complete";

                    db.SaveChanges();

                    var resp = Request.CreateResponse(HttpStatusCode.OK);
                    //resp.Content = new StringContent();
                    resp.Content = new StringContent(transferId.ToString(), Encoding.UTF8, "text/plain");    //send transferId??  just for practice??
                    return resp;

                    //var resp = Request.CreateResponse(HttpStatusCode.OK);
                    ////resp.Content = new StringContent();
                    //resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                    //return resp;
                }
                else
                {
                    order.Status = stripeCharge.FailureMessage;
                    db.SaveChanges();  //should i save here?  
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
                }

            }
            catch (Exception ex)
            {
                //send quick email

                //mjb HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException,
                    Caller = "Payment_Post",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                var returnMessage = "There was error with your transaction, please try again.";

                if (ex.Source == "Stripe.net")
                    returnMessage = ex.Message;

                if (Request != null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, returnMessage);
                //return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    //return new HttpResponseMessage(HttpStatusCode.InternalServerError,);
                }
            }

        }


        [HttpPost]
        [Route("Refund")]
        public HttpResponseMessage Refund(JObject jRefund)
        {
            //decimal amount = (Decimal)jRefund["amount"];
            //Int32 eventureOrderId = (Int32)jRefund["eventureOrderId"];
            //string description = (string)jRefund["description"];
            ////Int32 regId = (Int32)(jRefund["description"] || 0;

            var refund = new Refund();
            refund.Amount = (Decimal)jRefund["amount"];
            refund.EventureOrderId = (Int32)jRefund["eventureOrderId"];
            
            var _tranMan = new TransactionManager();
            _tranMan.ProcessRefund(refund);

            var resp = Request.CreateResponse(HttpStatusCode.OK);
            //resp.Content = new StringContent("ok", Encoding.UTF8, "text/plain");
            return resp;

        }
    }
}
