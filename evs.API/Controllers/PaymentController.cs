using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using System.Net.Http;
//using Amazon.SimpleEmail.Model;
using Newtonsoft.Json.Linq;
using Stripe;
using evs.DAL;
using evs.Model;
//using evs30Api.Filters;

using evs.Service;
using System.Configuration;

namespace evs.API.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    [RoutePrefix("api/payment")]
    public class PaymentController : ApiController
    {
        private evsContext db = new evsContext();

        [HttpPost]
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Post(JObject saveBundle)
        {
            //Uow.Sessions.Add(session);
            //Uow.Commit();
            //int rowsAdded = Convert.ToInt32((string) saveBundle["d"][0]["numberOfRowsAdded"]);
            //JsonConvert.DeserializeObject<Cart>(saveBundle);

            try
            {
                //int numOfRegs = 0;
                decimal totalFees = 0;

                //Int32 teamId = 0;
                //string teamMemberGuid = string.Empty;    // = 0;
                // Int32 paymentId = 0;

                //public static EventureOrder populateOrderFromBundle(JObject saveBundle)
                var order = new EventureOrder
                    {
                        DateCreated = DateTime.Now,
                        HouseId = (Int32)saveBundle["orderHouseId"],
                        Amount = (Decimal)saveBundle["orderAmount"],
                        Token = (string)saveBundle["stripeToken"],   //is this safe??
                        OwnerId = (Int32)saveBundle["ownerId"],
                        Status = "Init",
                        Voided = false
                    };
                db.Orders.Add(order);

                //validate order
                //if !validate
                //return  order.Status = validate messge;
                //return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //db.SaveChanges();

                var custDesc = string.Empty;
                var partEmail = string.Empty;
                var partName = string.Empty;
                var part = db.Participants.Where(p => p.Id == order.HouseId).FirstOrDefault();
                if (part != null)
                {
                    custDesc = part.FirstName + " " + part.LastName + "_ord" + order.Id;
                    partEmail = part.Email;
                    partName = part.FirstName + " " + part.LastName;
                }
                else
                {
                    //this should never happen  throw exception?
                    throw new Exception("couldn't find that houseId");
                }


                //var eventureListTypeId = 0;
                dynamic bundle = saveBundle;
                foreach (var regBundle in bundle.regs)
                {

                    var registration = new Registration
                        {
                            EventureListId = regBundle.eventureListId,
                            ParticipantId = regBundle.partId,
                            ListAmount = regBundle.fee,
                            Quantity = regBundle.quantity,
                            //EventureOrderId = order.Id,    
                            EventureOrder = order,
                            DateCreated = DateTime.Now,
                            TotalAmount = Convert.ToDecimal(regBundle.fee),
                            Type = "reg",
                        };
                    var eventureListTypeId = regBundle.eventureListTypeId;
                    db.Registrations.Add(registration);

                    foreach (var answerBundle in regBundle.answers)
                    {
                        var answer = new Answer
                        {
                            AnswerText = answerBundle.answer,
                            QuestionId = answerBundle.questionId,
                            Registration = registration
                        };
                        //registration.Answers.Add(answer);
                        db.Answer.Add(answer);
                    }

                    //need to handle groups here  //mjb

                    //numOfRegs += registration.Quantity;
                    //if (regBundle.groupId != "")
                    //    registration.GroupId = regBundle.groupId;   //this could be null
                    //if (regBundle.group2Id != "")
                    //    registration.Group2Id = regBundle.group2Id;   //this could be null


                    if (eventureListTypeId != 1)
                    {
                        var team = new Team
                            {
                                Name = regBundle.teamName, // (string)saveBundle["teamName"];
                                RegistrationId = registration.Id,
                                CoachId = order.HouseId,
                                OwnerId = order.OwnerId
                            };
                        db.Teams.Add(team);
                        //db.SaveChanges();
                        //mjbteamId = team.Id; //this sucks

                        //add coach to teamMember
                        var teamCoach = new TeamMember
                            {
                                Name = partName,
                                Email = partEmail,
                                //TeamId = team.Id,
                                Team = team,
                                ParticipantId = order.HouseId,
                                Active = true
                            };
                        db.TeamMembers.Add(teamCoach);
                        db.SaveChanges();
                        //mjbteamMemberGuid = teamCoach.TeamMemberGuid.ToString().ToUpper();
                        //this is returned to app in response

                        var payment = new TeamMemberPayment();
                        //{
                        payment.TeamId = team.Id;
                        payment.Amount = order.Amount;
                        payment.TeamMemberId = teamCoach.Id;
                        //};
                        db.TeamMemberPayments.Add(payment);

                        //db.SaveChanges();
                        //paymentId = payment.Id; //this is returned to app in response

                        foreach (dynamic teamBundle in regBundle.teamMembers)
                        //this is not nessessary now because only 1 reg in team
                        {
                            var teamMember = new TeamMember();
                            teamMember.Name = teamBundle.name;
                            teamMember.Email = teamBundle.email;
                            teamMember.Team = team;
                            teamMember.Active = true;
                            db.TeamMembers.Add(teamMember);
                        }
                    }
                }

                //populate surcharge
                if (bundle.charges != null)  //if no surcharges skip this
                {
                    foreach (dynamic surchargeBundle in bundle.charges)
                    {
                        var surcharge = new Surcharge
                            {
                                Amount = surchargeBundle.amount,
                                EventureListId = surchargeBundle.listId,
                                ChargeType = surchargeBundle.chargeType,
                                Description = surchargeBundle.desc,
                                ParticipantId = surchargeBundle.partId,
                                //EventureOrderId = order.Id,
                                EventureOrder = order,
                                DateCreated = DateTime.Now,
                                CouponId = surchargeBundle.couponId
                            };
                        totalFees = totalFees + Convert.ToDecimal(surchargeBundle.amount);
                        db.Surcharges.Add(surcharge);
                    }
                }

                //db.SaveChanges();


                Owner owner = db.Owners.Where(o => o.Id == 1).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                if (order.LocalApplicationFee < 0)
                    order.LocalApplicationFee = 0;

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
                                                        Amount = Convert.ToInt32(order.Amount * 100),
                                                        Currency = "usd",
                                                        CustomerId = customer.Id,
                                                        Description = owner.Name,
                                                        ApplicationFee = order.LocalApplicationFee
                                                    };
                var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                {
                    order.AuthorizationCode = stripeCharge.Id;
                    //stripeCharge.
                    order.CardNumber = stripeCharge.StripeCard.Last4;
                    order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                    order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                    order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                    //order.CardId = stripeCharge.StripeCard.;
                    order.CardName = stripeCharge.StripeCard.Name;
                    order.CardOrigin = stripeCharge.StripeCard.Country;
                    //mjb fixorder.CardType = stripeCharge.StripeCard.Type;
                    order.Voided = false;
                    order.Status = "Complete";
                    //mjb fix order.PaymentType = "credit";
                    //db.Orders.Add(order);
                    db.SaveChanges();

                    //adjust reg.TotalAmount to for surcharge
                    //mjb this might br going through entire db.reg
                    //foreach (var reg in db.Registrations) //order id = order.Id
                    //{
                    //    if (reg.EventureListId == surcharge.EventureListId &&
                    //        reg.ParticipantId == surcharge.ParticipantId)
                    //        reg.TotalAmount = reg.TotalAmount + surcharge.Amount;
                    //}
                    ////if coupon adust Redeemed  //mjb
                    //if (surcharge.ChargeType == "coupon")
                    //{
                    //    Coupon coupon = db.Coupons.Single(c => c.Id == surcharge.CouponId);
                    //    coupon.Redeemed++;
                    //    //db.SaveChanges(coupon);
                    //}

                    //call mail
                    HttpResponseMessage result = new MailController().SendConfirmMail(order.Id);

                    //return Request.CreateResponse(HttpStatusCode.OK, stripeCharge);
                    var resp = Request.CreateResponse(HttpStatusCode.OK);
                    //resp.Content = new StringContent();
                    resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                    return resp;

                }
                else
                {
                    order.Status = stripeCharge.FailureMessage;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
                }

            }
            catch (Exception ex)
            {
                //send quick email
                //mjb fixHttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

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
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage PostManual(JObject saveBundle)
        {
            //Uow.Sessions.Add(session);
            //Uow.Commit();
            //int rowsAdded = Convert.ToInt32((string) saveBundle["d"][0]["numberOfRowsAdded"]);
            //JsonConvert.DeserializeObject<Cart>(saveBundle);

            var log = new EventureLog();
            log.Message = "starting manual payment";
            log.Caller = "Manual Payment";
            log.Status = "Info";
            log.LogDate = System.DateTime.Now.ToLocalTime();
            log.DateCreated = System.DateTime.Now.ToLocalTime();
            db.EventureLogs.Add(log);
            db.SaveChanges();

            try
            {
                //int numOfRegs = 0;
                decimal totalFees = 0;

                //public static EventureOrder populateOrderFromBundle(JObject saveBundle)
                var order = new EventureOrder
                {
                    DateCreated = DateTime.Now,
                    HouseId = (Int32)saveBundle["orderHouseId"],
                    Amount = (Decimal)saveBundle["orderAmount"],
                    //Email = (string)saveBundle["orderEmail"],
                    //Name = (string)saveBundle["orderName"],
                    Token = (string)saveBundle["stripeToken"],   //is this safe??
                    OwnerId = (Int32)saveBundle["ownerId"],
                    Status = "Init",
                    //mjb fix PaymentType = (string)saveBundle["paymentType"],
                    Voided = false
                };
                db.Orders.Add(order);

                //validate order
                //if !validate
                //return  order.Status = validate messge;
                //return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //db.SaveChanges();

                dynamic bundle = saveBundle;
                foreach (dynamic regBundle in bundle.regs)
                {
                    //check that this exists?
                    var answer = new StockAnswerSet
                   {

                   };
                    //db.StockAnswerSets.Add(answer);

                    var registration = new Registration
                    {
                        EventureListId = regBundle.eventureListId,
                        ParticipantId = regBundle.partId,
                        //IsNullOrEmpty(planRec.approved_by) ? "" : planRec.approved_by.toString();
                        //Name = regBundle.displayList,   //mjb this will go away
                        ListAmount = regBundle.fee,
                        Quantity = regBundle.quantity,
                        EventureOrderId = order.Id,    //mjb1 this one seems ok
                        DateCreated = DateTime.Now,
                        //StockAnswerSetId = answer.Id
                        StockAnswerSet = answer,
                        TotalAmount = Convert.ToDecimal(regBundle.fee),
                        Type = "manreg"
                    };

                    //numOfRegs += registration.Quantity;
                    if (regBundle.groupId != "")
                        registration.GroupId = regBundle.groupId;   //this could be null
                    if (regBundle.group2Id != "")
                        registration.Group2Id = regBundle.group2Id;   //this could be null
                    db.Registrations.Add(registration);
                }

                //populate surcharge
                if (bundle.charges != null)  //if no surcharges skip this
                {
                    foreach (dynamic surchargeBundle in bundle.charges)
                    {
                        var surcharge = new Surcharge
                        {
                            Amount = surchargeBundle.amount,
                            EventureListId = surchargeBundle.listId,
                            ChargeType = surchargeBundle.chargeType,
                            Description = surchargeBundle.desc,
                            ParticipantId = surchargeBundle.partId,
                            EventureOrderId = order.Id,
                            DateCreated = DateTime.Now,
                            CouponId = surchargeBundle.couponId
                        };
                        totalFees = totalFees + Convert.ToDecimal(surchargeBundle.amount);
                        db.Surcharges.Add(surcharge);
                    }
                }

                //db.SaveChanges();

                Owner owner = db.Owners.Where(o => o.Id == 1).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                string custDesc = string.Empty;
                string partEmail = string.Empty;
                var part = db.Participants.Where(p => p.Id == order.HouseId).FirstOrDefault();
                if (part != null)
                {
                    custDesc = part.FirstName + " " + part.LastName + "_ord" + order.Id;
                    partEmail = part.Email;
                }
                else
                {
                    //this should never happen  throw exception?
                    custDesc = "participant" + "_ord" + order.Id + "_id:" + order.HouseId;
                    partEmail = "email" + "_ord" + order.Id + "_id:" + order.HouseId;
                }

                // create customer
                //var customerOptions = new StripeCustomerCreateOptions
                //{
                //    Email = partEmail,
                //    Description = custDesc,
                //    TokenId = order.Token,
                //};

                //var stripeCustomerService = new StripeCustomerService(owner.AccessToken);
                //var customer = stripeCustomerService.Create(customerOptions);

                //var stripeChargeService = new StripeChargeService(owner.AccessToken); //The token returned from the above method
                //var stripeChargeOption = new StripeChargeCreateOptions()
                //{
                //    AmountInCents = Convert.ToInt32(order.Amount * 100),
                //    Currency = "usd",
                //    CustomerId = customer.Id,
                //    Description = "HeadFirst",   //this needs to be dynamic
                //    ApplicationFeeInCents = order.LocalApplicationFee
                //};
                //var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                //if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                //{
                order.AuthorizationCode = ""; // stripeCharge.Id;
                //stripeCharge.
                order.CardNumber = ""; // stripeCharge.StripeCard.Last4;
                order.CardCvcCheck = ""; // stripeCharge.StripeCard.CvcCheck;
                order.CardExpires = ""; // stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                order.CardFingerprint = ""; // stripeCharge.StripeCard.Fingerprint;
                //order.CardId = stripeCharge.StripeCard.;
                order.CardName = ""; // stripeCharge.StripeCard.Name;
                order.CardOrigin = ""; // stripeCharge.StripeCard.Country;
                order.CardType = ""; // stripeCharge.StripeCard.Type;
                order.Voided = false;
                order.Status = "Complete";
                //switch here

                //order.PaymentType = "credit";
                //db.Orders.Add(order);
                db.SaveChanges();

                //adjust reg.TotalAmount to for surcharge
                //mjb this might br going through entire db.reg
                //foreach (var reg in db.Registrations) //order id = order.Id
                //{
                //    if (reg.EventureListId == surcharge.EventureListId &&
                //        reg.ParticipantId == surcharge.ParticipantId)
                //        reg.TotalAmount = reg.TotalAmount + surcharge.Amount;
                //}
                ////if coupon adust Redeemed  //mjb
                //if (surcharge.ChargeType == "coupon")
                //{
                //    Coupon coupon = db.Coupons.Single(c => c.Id == surcharge.CouponId);
                //    coupon.Redeemed++;
                //    //db.SaveChanges(coupon);
                //}

                //call mail
                HttpResponseMessage result = new MailController().SendConfirmMail(order.Id);

                //return Request.CreateResponse(HttpStatusCode.OK, stripeCharge);



                var resp = Request.CreateResponse(HttpStatusCode.OK);
                //resp.Content = new StringContent();
                resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                return resp;

                //}
                //else
                //{
                //    order.Status = stripeCharge.FailureMessage;
                //    db.SaveChanges();
                //    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
                //}

            }
            catch (Exception ex)
            {
                //send quick email
                //mjb fixHttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException,
                    Caller = "Manual_Payment_Post",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //[AcceptVerbs("OPTIONS")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        [Route("PostTeamPayment")]
        public HttpResponseMessage PostTeamPayment(JObject saveBundle)       //for the team member payment
        {
            try
            {
                decimal totalFees = 0;
                Int32 teamMemberId = (Int32)saveBundle["teamMemberId"];
                Int32 teamId = (Int32)saveBundle["teamId"];
                Int32 participantId = (Int32)saveBundle["participantId"];
                //public static EventureOrder populateOrderFromBundle(JObject saveBundle)
                //var order = new EventureOrder
                //{
                //    DateCreated = DateTime.Now,
                //    HouseId = 2,      //(Int32)saveBundle["participantId"],
                //    Amount = (Decimal)saveBundle["orderAmount"],
                //    Token = (string)saveBundle["stripeToken"],
                //    OwnerId = (Int32)saveBundle["ownerId"],  //need to send
                //    Status = "Init",
                //    Voided = false
                //};
                //db.Orders.Add(order);

                dynamic bundle = saveBundle;

                var member = db.TeamMembers.Where(m => m.Id == teamMemberId).FirstOrDefault();
                member.ParticipantId = participantId;


                //populate surcharge    //todo 
                //if (bundle.charges != null)  //if no surcharges skip this
                //{
                //    foreach (dynamic surchargeBundle in bundle.charges)
                //    {
                //        var surcharge = new Surcharge
                //        {
                //            Amount = surchargeBundle.amount,
                //            EventureListId = surchargeBundle.listId,
                //            ChargeType = surchargeBundle.chargeType,
                //            Description = surchargeBundle.desc,
                //            ParticipantId = surchargeBundle.partId,
                //            EventureOrderId = order.Id,
                //            DateCreated = DateTime.Now,
                //            CouponId = surchargeBundle.couponId
                //        };
                //        totalFees = totalFees + Convert.ToDecimal(surchargeBundle.amount);
                //        db.Surcharges.Add(surcharge);
                //    }
                //}

                //var payment = new TeamMemberPayment();
                ////{
                //payment.TeamId = teamId;
                //payment.Amount = 0;   // order.Amount;
                //payment.TeamMemberId = teamMemberId;
                //payment.DateCreated = DateTime.Now;

                ////};
                //db.TeamMemberPayments.Add(payment);

                //Owner owner = db.Owners.Where(o => o.Id == 1).SingleOrDefault();
                //if (owner == null)
                //{
                //    throw new Exception("Owner Setup is Not Configured Correctly");
                //}

                //calulate
                //order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                //order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                //order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                //if (order.LocalApplicationFee < 0)
                //    order.LocalApplicationFee = 0;

                //string custDesc = string.Empty;
                //string partEmail = string.Empty;
                //var part = db.Participants.Where(p => p.Id == order.HouseId).FirstOrDefault();
                //if (part != null)
                //{
                //    custDesc = part.FirstName + " " + part.LastName + "_ord" + order.Id;
                //    partEmail = part.Email;
                //}
                //else
                //{
                //    //this should never happen  throw exception?
                //    //custDesc = "participant" + "_ord" + order.Id + "_id:" + order.HouseId;
                //    //partEmail = "email" + "_ord" + order.Id + "_id:" + order.HouseId;
                //    throw new Exception("No Partipant found to match houseId ");
                //}

                //// create customer
                //var customerOptions = new StripeCustomerCreateOptions
                //{
                //    Email = partEmail,
                //    Description = custDesc,
                //    TokenId = order.Token,
                //};

                //var stripeCustomerService = new StripeCustomerService(owner.AccessToken);
                //var customer = stripeCustomerService.Create(customerOptions);

                //var stripeChargeService = new StripeChargeService(owner.AccessToken); //The token returned from the above method
                //var stripeChargeOption = new StripeChargeCreateOptions()
                //{
                //    Amount = Convert.ToInt32(order.Amount * 100),
                //    Currency = "usd",
                //    CustomerId = customer.Id,
                //    Description = owner.Name,
                //    ApplicationFee = order.LocalApplicationFee
                //};
                //var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                //if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                //{
                //    order.AuthorizationCode = stripeCharge.Id;
                //    order.CardNumber = stripeCharge.StripeCard.Last4;
                //    order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                //    order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                //    order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                //    order.CardName = stripeCharge.StripeCard.Name;
                //    order.CardOrigin = stripeCharge.StripeCard.Country;
                //    order.Voided = false;
                //    order.Status = "Complete";
                db.SaveChanges();

                var resp = Request.CreateResponse(HttpStatusCode.OK);
                //resp.Content = new StringContent(payment.Id.ToString(), Encoding.UTF8, "text/plain");
                return resp;

                //}
                //else
                //{
                //    order.Status = stripeCharge.FailureMessage;
                //    db.SaveChanges();
                //    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
                //}

            }
            catch (Exception ex)
            {
                //send quick email
                //mjb fixHttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

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
        [AllowAnonymous]
        //[AcceptVerbs("OPTIONS")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        [Route("PostTeam")]
        public HttpResponseMessage PostTeam(JObject saveBundle)
        {
            try
            {
                decimal totalFees = 0;
                //Int32 teamId = 0;
                //string teamMemberGuid;

                var order = new EventureOrder
                {
                    DateCreated = DateTime.Now,
                    HouseId = (Int32)saveBundle["orderHouseId"],
                    Amount = (Decimal)saveBundle["orderAmount"],
                    Token = (string)saveBundle["stripeToken"],
                    OwnerId = (Int32)saveBundle["ownerId"],
                    Status = "Init",
                    Voided = false
                };
                //db.Orders.Add(order);

                var custDesc = string.Empty;
                var partEmail = string.Empty;
                var partName = string.Empty;
                var part = db.Participants.Where(p => p.Id == order.HouseId).FirstOrDefault();
                if (part != null)
                {
                    custDesc = part.FirstName + " " + part.LastName + "_ord" + order.Id;
                    partEmail = part.Email;
                    partName = part.FirstName + " " + part.LastName;
                }
                else
                {
                    throw new Exception("couldn't find that houseId");
                }

                dynamic bundle = saveBundle;
                foreach (dynamic regBundle in bundle.regs) //this is not nessessary now because only 1 reg in team
                {
                    var registration = new Registration
                    {
                        EventureListId = regBundle.eventureListId,
                        ParticipantId = regBundle.partId,
                        ListAmount = regBundle.fee,
                        Quantity = regBundle.quantity,
                        EventureOrder = order,
                        //EventureOrderId = order.Id, //mjb1 this one seems ok
                        DateCreated = DateTime.Now,
                        TotalAmount = Convert.ToDecimal(regBundle.fee),
                        Type = "reg"
                    };

                    var team = new Team
                    {
                        Name = regBundle.teamName, // (string)saveBundle["teamName"];
                        //RegistrationId = registration.Id,
                        Registration = registration,
                        CoachId = order.HouseId,
                        OwnerId = order.OwnerId,
                        IsPaidInFull = false,
                        DateCreated = DateTime.Now,
                        TimeFinish = regBundle.timeFinish,
                        Division = regBundle.division,
                        Active = true
                    };
                    //db.Teams.Add(team);
                    //db.SaveChanges();
                    //teamId = team.Id;      //this sucks

                    //add coach to teamMember
                    var teamCoach = new TeamMember
                    {
                        Name = partName,
                        Email = partEmail,
                        //TeamId = team.Id,
                        Team = team,
                        ParticipantId = order.HouseId,
                        DateCreated = DateTime.Now,
                        Active = true
                    };
                    team.TeamMembers.Add(teamCoach);
                    //db.TeamMembers.Add(teamCoach);
                    //db.SaveChanges();
                    //teamMemberGuid = teamCoach.TeamMemberGuid.ToString().ToUpper();     //this is returned to app in response

                    //var payment = new TeamMemberPayment
                    //{
                    //    //payment.TeamId = team.Id;
                    //    Team = team,
                    //    Amount = order.Amount,
                    //    TeamMemberId = teamCoach.Id,
                    //    EventureOrder = order,
                    //    DateCreated = DateTime.Now,
                    //    Active = true
                    //};
                    ////db.TeamMemberPayments.Add(payment);
                    //team.TeamMemberPayments.Add(payment);

                    //db.SaveChanges();
                    //paymentId = payment.Id;     //this is returned to app in response

                    foreach (dynamic teamBundle in regBundle.teamMembers)
                    //this is not nessessary now because only 1 reg in team
                    {
                        var teamMember = new TeamMember
                        {
                            Name = teamBundle.name,
                            Email = teamBundle.email,
                            //teamMember.TeamId = team.Id;
                            Team = team,
                            Active = true,
                            DateCreated = DateTime.Now
                            //db.TeamMembers.Add(teamMember);
                        };
                        //team.TeamMembers.Add(teamMember);
                        db.TeamMembers.Add(teamMember);
                    }

                    //}
                    //populate surcharge
                    if (bundle.charges != null)  //if no surcharges skip this
                    {
                        foreach (dynamic surchargeBundle in bundle.charges)
                        {
                            var surcharge = new Surcharge
                            {
                                Amount = surchargeBundle.amount,
                                EventureListId = surchargeBundle.listId,
                                ChargeType = surchargeBundle.chargeType,
                                Description = surchargeBundle.desc,
                                ParticipantId = surchargeBundle.partId,
                                EventureOrderId = order.Id,
                                DateCreated = DateTime.Now,
                                CouponId = surchargeBundle.couponId
                            };
                            totalFees = totalFees + Convert.ToDecimal(surchargeBundle.amount);
                            //db.Surcharges.Add(surcharge);
                            order.Surcharges.Add(surcharge);
                        }
                    }
                }

                Owner owner = db.Owners.Where(o => o.Id == order.OwnerId).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }




                if (order.Amount > 0)
                {


                    //calulate
                    order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                    order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                    order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                    if (order.LocalApplicationFee < 0)
                        order.LocalApplicationFee = 0;

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
                        Amount = Convert.ToInt32(order.Amount * 100),
                        Currency = "usd",
                        CustomerId = customer.Id,
                        Description = owner.Name,
                        ApplicationFee = order.LocalApplicationFee
                    };
                    var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                    if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                    {
                        //    order.AuthorizationCode = stripeCharge.Id;
                        //    //stripeCharge.
                        //    order.CardNumber = stripeCharge.StripeCard.Last4;
                        //    order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                        //    order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                        //    order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                        //    //order.CardId = stripeCharge.StripeCard.;
                        //    order.CardName = stripeCharge.StripeCard.Name;
                        //    order.CardOrigin = stripeCharge.StripeCard.Country;
                        //    order.CardType = stripeCharge.StripeCard.Type;
                        //    order.Voided = false;
                        //    order.Status = "Complete";
                        //    order.PaymentType = PaymentType.credit;
                        //    //db.Orders.Add(order);
                        //    db.SaveChanges();



                        //    //return Request.CreateResponse(HttpStatusCode.OK, stripeCharge);

                        //    var resp = Request.CreateResponse(HttpStatusCode.OK);
                        //    //order.Id.ToString()  we are using teamGuid for demo because we already have 
                        //    //                     a getbyteamguid
                        //    resp.Content = new StringContent(paymentId.ToString(), Encoding.UTF8, "text/plain");
                        //    return resp;


                        /////////////////////////////////////////////////////////////

                        //orderService.CompleteOrder(stripeCharge)
                        order.AuthorizationCode = stripeCharge.Id;
                        //stripeCharge.
                        order.CardNumber = stripeCharge.StripeCard.Last4;
                        order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                        order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                        order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                        //order.CardId = stripeCharge.StripeCard.;
                        order.CardName = stripeCharge.StripeCard.Name;
                        order.CardOrigin = stripeCharge.StripeCard.Country;
                        //mjb fixorder.CardType = stripeCharge.StripeCard.Type;
                        order.Voided = false;
                        order.Status = "Complete";
                        //mjb fix order.PaymentType = "credit";
                        //db.Orders.Add(order);
                        //db.SaveChanges();

                        //not good return reason
                        //OrderService _orderService = new OrderService();
                        //var x = _orderService.CreateOrder(order);

                        db.Orders.Add(order);
                        db.SaveChanges();

                        var onlyRegId = 0;
                        foreach (var reg in order.Registrations)
                        {
                            onlyRegId = reg.Id; ;
                        }


                        foreach (var member in db.TeamMembers.Where(m => m.Team.RegistrationId == onlyRegId))
                        {
                            //if member.partId is null
                            if (member.ParticipantId == null)
                            {
                                HttpResponseMessage result = new MailController().SendTeamPlayerInviteMail(member.Id);
                                //teamGuid = member.Team.TeamGuid.ToString().ToUpper(); //this sucks too!!
                            }
                        }

                        //foreach (var member in db.TeamMembers.Where(m => m.TeamId == teamId))
                        //{
                        //    //if member.partId is null
                        //    if (member.ParticipantId == null)
                        //    {
                        //        HttpResponseMessage result = new MailController().SendTeamPlayerInviteMail(member.Id);
                        //        //teamGuid = member.Team.TeamGuid.ToString().ToUpper(); //this sucks too!!
                        //    }
                        //}


                        //call mail
                        //HttpResponseMessage result = new MailController().SendConfirmMail(order.Id);

                        //db.TeamMembers.Where(m => m.re)
                        //foreach (var reg in order.Registrations)
                        //{

                        //    foreach(var team in reg.te)
                        //    //if member.partId is null
                        //    //if (member.ParticipantId == null)
                        //    //{
                        //        HttpResponseMessage result = new MailController().SendTeamPlayerInviteMail(reg.               //(member.Id);
                        //        //teamGuid = member.Team.TeamGuid.ToString().ToUpper(); //this sucks too!!
                        //    //}
                        //}

                        var resp = Request.CreateResponse(HttpStatusCode.OK);
                        //resp.Content = new StringContent();
                        resp.Content = new StringContent(onlyRegId.ToString(), Encoding.UTF8, "text/plain");
                        return resp;

                    }
                    else
                    {
                        order.Status = stripeCharge.FailureMessage;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
                    }

                }
                else
                {
                    order.Voided = false;
                    order.Status = "Complete";
                    //mjb fix order.PaymentType = "credit";
                    //db.Orders.Add(order);
                    db.SaveChanges();

                    var onlyRegId = 0;
                    foreach (var reg in order.Registrations)
                    {
                        onlyRegId = reg.Id; ;
                    }

                    foreach (var member in db.TeamMembers.Where(m => m.Team.RegistrationId == onlyRegId))
                    {
                        //if member.partId is null
                        if (member.ParticipantId == null)
                        {
                            //HttpResponseMessage result = new MailController().SendTeamPlayerInviteMail(member.Id);   //bourbon chase doesn't want email
                            //teamGuid = member.Team.TeamGuid.ToString().ToUpper(); //this sucks too!!
                        }
                    }

                    HttpResponseMessage confirmResult = new MailController().SendConfirmMail(order.Id);

                    //return Request.CreateResponse(HttpStatusCode.OK, stripeCharge);
                    var resp = Request.CreateResponse(HttpStatusCode.OK);
                    //resp.Content = new StringContent();
                    resp.Content = new StringContent(onlyRegId.ToString(), Encoding.UTF8, "text/plain");
                    return resp;
                }


                //}
            }
            catch (Exception ex)
            {
                //send quick email
                //HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException + " -- bundle: " + saveBundle,
                    Caller = "Payment_PostTeam",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                var returnMessage = "There was error with your transaction, please try again.";

                if (ex.Source == "Stripe.net")
                    returnMessage = ex.Message;

                //if (Request != null)
                //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, returnMessage);
                ////return Request.CreateResponse(HttpStatusCode.InternalServerError);
                //else
                //{
                //    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //    //return new HttpResponseMessage(HttpStatusCode.InternalServerError,);
                //}

                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError);  //, returnMessage);   //this is temp
                //var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError, returnMessage);
                //return resp;

                //var x = ex.InnerException;
                var badResponse = Request.CreateResponse(HttpStatusCode.BadRequest, returnMessage);
                return badResponse;

            }
        }

        [HttpPost]
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage SendTeamPaymentConfirmMail(Int32 id)      //had to put this here temporarily because i was having cross origin issues 
        {
            //mjb fixHttpResponseMessage result = new MailController().SendTeamPaymentConfirmMail(id);

            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            //resp.Headers.Add("Access-Control-Allow-Origin", "*");
            //resp.Headers.Add("Access-Control-Allow-Methods", "POST");
            return resp;

        }

        [HttpPost]
        [Route("PostTeamBalance")]
        public HttpResponseMessage PostTeamCaptainPayment(JObject saveBundle)       //this is when we have
        {
            try
            {
                decimal totalFees = 0;
                //Int32 teamMemberId = (Int32)saveBundle["teamMemberId"];
                Int32 teamId = (Int32)saveBundle["teamId"];
                Int32 participantId = (Int32)saveBundle["participantId"];
                decimal amount = (decimal)saveBundle["amount"];

                var order = new EventureOrder
                {
                    DateCreated = DateTime.Now,
                    HouseId = participantId,      //(Int32)saveBundle["participantId"],
                    Amount = amount,
                    Token = (string)saveBundle["stripeToken"],
                    OwnerId = (Int32)saveBundle["ownerId"], 
                    Status = "Init",
                    Voided = false
                };
                //db.Orders.Add(order);

                int place = 0;

                TeamMember member = db.TeamMembers.Where(m => m.ParticipantId == participantId && m.TeamId == teamId).SingleOrDefault();
                var TeamMemberId = member.Id;

                place = 1;


                Owner owner = db.Owners.Where(o => o.Id == 1).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }


                var part = db.Participants.Where(p => p.Id == order.HouseId).FirstOrDefault();
                if (part != null)
                {
                    //custDesc = part.FirstName + " " + part.LastName + "_ord" + order.Id;
                    //partEmail = part.Email;
                    //partName = part.FirstName + " " + part.LastName;
                }
                else
                {
                    throw new Exception("couldn't find that houseId");
                }

                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                if (order.LocalApplicationFee < 0)
                    order.LocalApplicationFee = 0;
               
                place = 3;

                var customerOptions = new StripeCustomerCreateOptions
                {
                    Email = part.Email, //Email,
                    Description = part.FirstName + " " + part.LastName,
                    TokenId = order.Token,
                };

                var stripeCustomerService = new StripeCustomerService(owner.AccessToken);   //owner.AccessToken
                var customer = stripeCustomerService.Create(customerOptions);

                place = 4;

                //int err = place / (place - place);

                var stripeChargeService = new StripeChargeService(owner.AccessToken); //The token returned from the above method
                var stripeChargeOption = new StripeChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(order.Amount * 100),
                    Currency = "usd",
                    CustomerId = customer.Id,
                    Description = owner.Name,
                    ApplicationFee = order.LocalApplicationFee
                };

                place = 5;

                var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                place = 6;


                if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                {
                    //orderService.CompleteOrder(stripeCharge)
                    order.AuthorizationCode = stripeCharge.Id;
                    //stripeCharge.
                    order.CardNumber = stripeCharge.StripeCard.Last4;
                    order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                    order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                    order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                    //order.CardId = stripeCharge.StripeCard.;
                    order.CardName = stripeCharge.StripeCard.Name;
                    order.CardOrigin = stripeCharge.StripeCard.Country;
                    //mjb fixorder.CardType = stripeCharge.StripeCard.Type;
                    order.Voided = false;
                    order.Status = "Complete";
                    order.OrderStatus = OrderStatus.Completed;
                    //mjb fix order.PaymentType = "credit";
                    //db.Orders.Add(order);
                    //db.SaveChanges();

                    place = 7;

                    //not good return reason
                    OrderService _orderService = new OrderService();
                    var x = _orderService.CreateOrder(order);

                    place = 8;

                    var payment = new TeamMemberPayment
                    {
                        TeamId = teamId,
                        Amount = amount,
                        TeamMemberId = TeamMemberId,    //member.Id,
                        Active = true,
                        DateCreated = DateTime.Now,
                        EventureListId = (Int32)saveBundle["eventureListId"],
                        //EventureOrder = order
                        EventureOrderId = order.Id
                    };
                    db.TeamMemberPayments.Add(payment);

                    //HttpResponseMessage result;

                    //if (ConfigurationManager.AppSettings["CustomName"] == "bourbonchase")
                    //    //result = new MailController().SendBourbonLotteryConfirm(order.Id);
                    //    result = new MailController().SendConfirmMail(order.Id);   //change back to bourbon chase??
                    //else
                    //    result = new MailController().SendConfirmMail(order.Id);


                    //HttpResponseMessage result = new MailController().SendTestEmail();

                    //var resp = Request.CreateResponse(HttpStatusCode.OK);
                    ////resp.Content = new StringContent();
                    //resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                    //return resp;

                    db.SaveChanges();

                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
                else
                {
                    //order.Status = stripeCharge.FailureMessage;
                    //db.SaveChanges();
                    //return 
                    //var badResponse = Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);  //stripeCharge.FailureCode
                    //return badResponse;

                    var logE = new EventureLog();
                    logE.Message = "Stripe Exception: " + stripeCharge.FailureMessage + " -- place: " + place + " -- bundle: " + saveBundle;
                    logE.Caller = "Order_ERROR_stripe";
                    logE.Status = "Warning";
                    logE.LogDate = System.DateTime.Now.ToLocalTime();
                    logE.DateCreated = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(logE);
                    db.SaveChanges();

                    var badResp = Request.CreateResponse(HttpStatusCode.BadRequest);
                    badResp.Content = new StringContent(stripeCharge.FailureMessage, Encoding.UTF8, "text/plain");
                    return badResp;
                }
            }
            catch (Exception ex)
            {
                var returnMessage = "error: " + ex.Message + " -- " + ex.InnerException;
                var badResponse = Request.CreateResponse(HttpStatusCode.BadRequest, returnMessage);
                return badResponse;

            }
        }






        /////

    }  //api controller



}


//[HttpPost]
// [AllowAnonymous]
// [AcceptVerbs("OPTIONS")]
// [System.Web.Mvc.ValidateAntiForgeryToken]
// public HttpResponseMessage PostTest(JObject saveBundle)
// {
//     //Uow.Sessions.Add(session);
//     //Uow.Commit();
//     //int rowsAdded = Convert.ToInt32((string) saveBundle["d"][0]["numberOfRowsAdded"]);
//     //JsonConvert.DeserializeObject<Cart>(saveBundle);

//     try
//     {
//         var log = new EventureLog();
//         log.Message = "starting payment -- bundle: " + saveBundle;
//         log.Caller = "StripePayment";
//         log.Status = "Info";
//         log.LogDate = System.DateTime.Now.ToLocalTime();
//         db.EventureLogs.Add(log);
//         db.SaveChanges();

//         //return Request.CreateResponse(HttpStatusCode.OK);

//         var resp = new HttpResponseMessage(HttpStatusCode.OK);
//         //resp.Headers.Add("Access-Control-Allow-Origin", "*");
//         //resp.Headers.Add("Access-Control-Allow-Methods", "POST");

//         return resp;


//     }
//     catch (Exception ex)
//     {
//         //send quick email
//         //HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_PostTest: " + ex.Message + "\n\n" + ex.InnerException);

//         //regular log
//         var logE = new EventureLog
//         {
//             Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException + "-- bundle: " + saveBundle,
//             Caller = "Payment_PostTest",
//             Status = "ERROR",
//             LogDate = System.DateTime.Now.ToLocalTime()
//         };
//         db.EventureLogs.Add(logE);
//         db.SaveChanges();

//         var returnMessage = "There was error with your transaction, please try again.";

//         if (ex.Source == "Stripe.net")
//             returnMessage = ex.Message;

//         if (Request != null)
//             return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, returnMessage);
//         //return Request.CreateResponse(HttpStatusCode.InternalServerError);
//         else
//         {
//             return new HttpResponseMessage(HttpStatusCode.InternalServerError);
//             //return new HttpResponseMessage(HttpStatusCode.InternalServerError,);
//         }
//     }
// }


//[HttpPost]
//      [AllowAnonymous]
//      [AcceptVerbs("OPTIONS")]
//      [System.Web.Mvc.ValidateAntiForgeryToken]
//      public HttpResponseMessage PostTeam(JObject saveBundle)
//      {
//          try
//          {
//              //TODO:  add a team member payment
//              //int numOfRegs = 0;
//              decimal totalFees = 0;
//              Int32 teamId = 0;
//              string teamMemberGuid = string.Empty;    // = 0;
//              Int32 paymentId = 0;

//              //public static EventureOrder populateOrderFromBundle(JObject saveBundle)
//              var order = new EventureOrder
//              {
//                  DateCreated = DateTime.Now,
//                  HouseId = (Int32)saveBundle["orderHouseId"],
//                  Amount = (Decimal)saveBundle["orderAmount"],
//                  Token = (string)saveBundle["stripeToken"],
//                  OwnerId = (Int32)saveBundle["ownerId"],
//                  Status = "Init",
//                  Voided = false
//              };
//              db.Orders.Add(order);

//              var custDesc = string.Empty;
//              var partEmail = string.Empty;
//              var partName = string.Empty;
//              var part = db.Participants.Where(p => p.Id == order.HouseId).FirstOrDefault();
//              if (part != null)
//              {
//                  custDesc = part.FirstName + " " + part.LastName + "_ord" + order.Id;
//                  partEmail = part.Email;
//                  partName = part.FirstName + " " + part.LastName;
//              }
//              else
//              {
//                  //this should never happen  throw exception?
//                  //custDesc = "participant" + "_ord" + order.Id + "_id:" + order.HouseId;
//                  //partEmail = "email" + "_ord" + order.Id + "_id:" + order.HouseId;
//                  throw new Exception("couldn't find that houseId");
//              }

//              dynamic bundle = saveBundle;
//              foreach (dynamic regBundle in bundle.regs) //this is not nessessary now because only 1 reg in team
//              {
//                  var registration = new Registration
//                      {
//                          EventureListId = regBundle.eventureListId,
//                          ParticipantId = regBundle.partId,
//                          ListAmount = regBundle.fee,
//                          Quantity = regBundle.quantity,
//                          EventureOrderId = order.Id, //mjb1 this one seems ok
//                          DateCreated = DateTime.Now,
//                          TotalAmount = Convert.ToDecimal(regBundle.fee),
//                          Type = "reg"
//                      };
//                  db.Registrations.Add(registration);
//                  //db.SaveChanges();

//                  var team = new Team
//                      {
//                          Name = regBundle.teamName, // (string)saveBundle["teamName"];
//                          RegistrationId = registration.Id,
//                          CoachId = order.HouseId,
//                          OwnerId = order.OwnerId
//                      };
//                  db.Teams.Add(team);
//                  db.SaveChanges();
//                  teamId = team.Id;      //this sucks

//                  //add coach to teamMember
//                  var teamCoach = new TeamMember
//                      {
//                          Name = partName,
//                          Email = partEmail,
//                          TeamId = team.Id,
//                          ParticipantId = order.HouseId,
//                          Active = true
//                      };
//                  db.TeamMembers.Add(teamCoach);
//                  db.SaveChanges();
//                  teamMemberGuid = teamCoach.TeamMemberGuid.ToString().ToUpper();     //this is returned to app in response

//                  var payment = new TeamMemberPayment();
//                  //{
//                  payment.TeamId = team.Id;
//                  payment.Amount = order.Amount;
//                  payment.TeamMemberId = teamCoach.Id;
//                  //};
//                  db.TeamMemberPayments.Add(payment);

//                  db.SaveChanges();
//                  paymentId = payment.Id;     //this is returned to app in response

//                  foreach (dynamic teamBundle in regBundle.teamMembers)
//                  //this is not nessessary now because only 1 reg in team
//                  {
//                      var teamMember = new TeamMember();
//                      teamMember.Name = teamBundle.name;
//                      teamMember.Email = teamBundle.email;
//                      teamMember.TeamId = team.Id;
//                      teamMember.Active = true;
//                      db.TeamMembers.Add(teamMember);
//                  }

//              }
//              //populate surcharge
//              if (bundle.charges != null)  //if no surcharges skip this
//              {
//                  foreach (dynamic surchargeBundle in bundle.charges)
//                  {
//                      var surcharge = new Surcharge
//                      {
//                          Amount = surchargeBundle.amount,
//                          EventureListId = surchargeBundle.listId,
//                          ChargeType = surchargeBundle.chargeType,
//                          Description = surchargeBundle.desc,
//                          ParticipantId = surchargeBundle.partId,
//                          EventureOrderId = order.Id,
//                          DateCreated = DateTime.Now,
//                          CouponId = surchargeBundle.couponId
//                      };
//                      totalFees = totalFees + Convert.ToDecimal(surchargeBundle.amount);
//                      db.Surcharges.Add(surcharge);
//                  }
//              }

//              Owner owner = db.Owners.Where(o => o.Id == order.OwnerId).SingleOrDefault();
//              if (owner == null)
//              {
//                  throw new Exception("Owner Setup is Not Configured Correctly");
//              }

//              //calulate
//              order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
//              order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerPerChargeInCents);
//              order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

//              if (order.LocalApplicationFee < 0)
//                  order.LocalApplicationFee = 0;

//              // create customer
//              var customerOptions = new StripeCustomerCreateOptions
//              {
//                  Email = partEmail,
//                  Description = custDesc,
//                  TokenId = order.Token,
//              };

//              var stripeCustomerService = new StripeCustomerService(owner.AccessToken);
//              var customer = stripeCustomerService.Create(customerOptions);

//              var stripeChargeService = new StripeChargeService(owner.AccessToken); //The token returned from the above method
//              var stripeChargeOption = new StripeChargeCreateOptions()
//              {
//                  AmountInCents = Convert.ToInt32(order.Amount * 100),
//                  Currency = "usd",
//                  CustomerId = customer.Id,
//                  Description = owner.Name,
//                  ApplicationFeeInCents = order.LocalApplicationFee
//              };
//              var stripeCharge = stripeChargeService.Create(stripeChargeOption);

//              if (string.IsNullOrEmpty(stripeCharge.FailureCode))
//              {
//                  order.AuthorizationCode = stripeCharge.Id;
//                  //stripeCharge.
//                  order.CardNumber = stripeCharge.StripeCard.Last4;
//                  order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
//                  order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
//                  order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
//                  //order.CardId = stripeCharge.StripeCard.;
//                  order.CardName = stripeCharge.StripeCard.Name;
//                  order.CardOrigin = stripeCharge.StripeCard.Country;
//                  order.CardType = stripeCharge.StripeCard.Type;
//                  order.Voided = false;
//                  order.Status = "Complete";
//                  order.PaymentType = "credit";
//                  //db.Orders.Add(order);
//                  db.SaveChanges();

//                  //call mail
//                  //HttpResponseMessage result = new MailController().SendConfirmMail(order.Id);
//                  foreach (var member in db.TeamMembers.Where(m => m.TeamId == teamId))
//                  {
//                      //if member.partId is null
//                      if (member.ParticipantId == null)
//                      {
//                          HttpResponseMessage result = new MailController().SendSoccerTryoutInviteMail(member.Id);
//                          //teamGuid = member.Team.TeamGuid.ToString().ToUpper(); //this sucks too!!
//                      }
//                  }

//                  //return Request.CreateResponse(HttpStatusCode.OK, stripeCharge);

//                  var resp = Request.CreateResponse(HttpStatusCode.OK);
//                  //order.Id.ToString()  we are using teamGuid for demo because we already have 
//                  //                     a getbyteamguid
//                  resp.Content = new StringContent(paymentId.ToString(), Encoding.UTF8, "text/plain");    
//                  return resp;

//              }
//              else
//              {
//                  order.Status = stripeCharge.FailureMessage;
//                  db.SaveChanges();
//                  return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
//              }

//          }
//          catch (Exception ex)
//          {
//              //send quick email
//              //HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

//              //regular log
//              var logE = new EventureLog
//              {
//                  Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException + " -- bundle: " + saveBundle,
//                  Caller = "Payment_PostTeam",
//                  Status = "ERROR",
//                  LogDate = System.DateTime.Now.ToLocalTime()
//              };
//              db.EventureLogs.Add(logE);
//              db.SaveChanges();

//              var returnMessage = "There was error with your transaction, please try again.";

//              if (ex.Source == "Stripe.net")
//                  returnMessage = ex.Message;

//              if (Request != null)
//                  return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, returnMessage);
//              //return Request.CreateResponse(HttpStatusCode.InternalServerError);
//              else
//              {
//                  return new HttpResponseMessage(HttpStatusCode.InternalServerError);
//                  //return new HttpResponseMessage(HttpStatusCode.InternalServerError,);
//              }
//          }
//      }