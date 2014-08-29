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
using evs30Api.Filters;

namespace evs30Api.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class PaymentController : ApiController
    {
        private evsContext db = new evsContext();

        [HttpPost]
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Post(JObject saveBundle)
        {
            //Uow.Sessions.Add(session);
            //Uow.Commit();
            //int rowsAdded = Convert.ToInt32((string) saveBundle["d"][0]["numberOfRowsAdded"]);
            //JsonConvert.DeserializeObject<Cart>(saveBundle);

            var log = new EventureLog();
            log.Message = "starting payment";
            log.Caller = "StripePayment";
            log.Status = "Info";
            log.LogDate = System.DateTime.Now.ToLocalTime();
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
                        Token = (string)saveBundle["orderToken"],   //is this safe??
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

                dynamic bundle = saveBundle;
                foreach (var regBundle in bundle.regs)
                {
                    //check that this exists?
                    var answer = new StockAnswerSet
                    {
                        StockQuestionSetId = regBundle.stockAnswerSet.stockQuestionSetId,
                        ShirtSize = regBundle.stockAnswerSet.shirtSize,
                        FinishTime = regBundle.stockAnswerSet.finishTime,
                        BibName = regBundle.stockAnswerSet.bibName,
                        HowHear = regBundle.stockAnswerSet.howHear,
                        //OwnRv = regBundle.stockAnswerSet.ownRv,
                        //NextRv = regBundle.stockAnswerSet.nextRv,
                        //HowHearRv = regBundle.stockAnswerSet.howHearRv,
                        School = regBundle.stockAnswerSet.school,
                        HowHearDropDown = regBundle.stockAnswerSet.howHearDropDown,
                        EstimatedSwimTime400 = regBundle.stockAnswerSet.estimatedSwimTime400,
                        EstimatedSwimTime = regBundle.stockAnswerSet.estimatedSwimTime,
                        Notes = regBundle.stockAnswerSet.notes,
                        AnnualIncome = regBundle.stockAnswerSet.annualIncome,
                        RelayTeamQuestion = regBundle.stockAnswerSet.relayTeamQuestion,
                        Usat = regBundle.stockAnswerSet.usat,
                        ShirtUpgrade = regBundle.stockAnswerSet.shirtUpgrade,
                        Wheelchair = regBundle.stockAnswerSet.wheelchair,
                        PuretapUnisex = regBundle.stockAnswerSet.puretapUnisex,
                        NortonUnisex = regBundle.stockAnswerSet.nortonUnisex,
                        BourbonGenderSpecific = regBundle.stockAnswerSet.bourbonGenderSpecific,
                        HearRunathon = regBundle.stockAnswerSet.hearRunathon,
                        HearPure = regBundle.stockAnswerSet.hearPure,
                        HearNorton = regBundle.stockAnswerSet.hearNorton,
                        HearBourbon = regBundle.stockAnswerSet.hearBourbon,
                        ParticipatePure = regBundle.stockAnswerSet.participatePure,
                        ParticipateNorton = regBundle.stockAnswerSet.participateNorton,
                        ParticipateBourbon = regBundle.stockAnswerSet.participateBourbon,
                        Mile15 = regBundle.stockAnswerSet.mile15,
                        SportsEmails = regBundle.stockAnswerSet.sportsEmails,
                        //BourbonWaiver = regBundle.stockAnswerSet.bourbonWaiver
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
                            Type = "reg"
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
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                if (order.LocalApplicationFee < 0)
                    order.LocalApplicationFee = 0;

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
                                                        AmountInCents = Convert.ToInt32(order.Amount * 100),
                                                        Currency = "usd",
                                                        CustomerId = customer.Id,
                                                        Description = owner.Name,
                                                        ApplicationFeeInCents = order.LocalApplicationFee
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
                    order.CardType = stripeCharge.StripeCard.Type;
                    order.Voided = false;
                    order.Status = "Complete";
                    order.PaymentType = "credit";
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
                HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                    {
                        Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException,
                        Caller = "Payment_Post",
                        Status = "ERROR",
                        LogDate = System.DateTime.Now.ToLocalTime()
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
        [System.Web.Mvc.ValidateAntiForgeryToken]
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
                    Token = (string)saveBundle["orderToken"],   //is this safe??
                    OwnerId = (Int32)saveBundle["ownerId"],
                    Status = "Init",
                    PaymentType = (string)saveBundle["paymentType"],
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
                        StockQuestionSetId = regBundle.stockAnswerSet.stockQuestionSetId,
                        ShirtSize = regBundle.stockAnswerSet.shirtSize,
                        FinishTime = regBundle.stockAnswerSet.finishTime,
                        BibName = regBundle.stockAnswerSet.bibName,
                        HowHear = regBundle.stockAnswerSet.howHear,
                        //OwnRv = regBundle.stockAnswerSet.ownRv,
                        //NextRv = regBundle.stockAnswerSet.nextRv,
                        //HowHearRv = regBundle.stockAnswerSet.howHearRv,
                        School = regBundle.stockAnswerSet.school,
                        HowHearDropDown = regBundle.stockAnswerSet.howHearDropDown,
                        EstimatedSwimTime400 = regBundle.stockAnswerSet.estimatedSwimTime400,
                        EstimatedSwimTime = regBundle.stockAnswerSet.estimatedSwimTime,
                        Notes = regBundle.stockAnswerSet.notes,
                        AnnualIncome = regBundle.stockAnswerSet.annualIncome,
                        RelayTeamQuestion = regBundle.stockAnswerSet.relayTeamQuestion,
                        Usat = regBundle.stockAnswerSet.usat,
                        ShirtUpgrade = regBundle.stockAnswerSet.shirtUpgrade,
                        Wheelchair = regBundle.stockAnswerSet.wheelchair,
                        PuretapUnisex = regBundle.stockAnswerSet.puretapUnisex,
                        NortonUnisex = regBundle.stockAnswerSet.nortonUnisex,
                        BourbonGenderSpecific = regBundle.stockAnswerSet.bourbonGenderSpecific,
                        HearRunathon = regBundle.stockAnswerSet.hearRunathon,
                        HearPure = regBundle.stockAnswerSet.hearPure,
                        HearNorton = regBundle.stockAnswerSet.hearNorton,
                        HearBourbon = regBundle.stockAnswerSet.hearBourbon,
                        ParticipatePure = regBundle.stockAnswerSet.participatePure,
                        ParticipateNorton = regBundle.stockAnswerSet.participateNorton,
                        ParticipateBourbon = regBundle.stockAnswerSet.participateBourbon,
                        Mile15 = regBundle.stockAnswerSet.mile15,
                        SportsEmails = regBundle.stockAnswerSet.sportsEmails,
                        //BourbonWaiver = regBundle.stockAnswerSet.bourbonWaiver
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
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerPerChargeInCents);
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
                HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException,
                    Caller = "Manual_Payment_Post",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime()
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
        [AcceptVerbs("OPTIONS")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage PostTeam(JObject saveBundle)
        {
            try
            {
                //TODO:  add a team member payment
                //int numOfRegs = 0;
                decimal totalFees = 0;
                Int32 teamId = 0;
                string teamMemberGuid = string.Empty;    // = 0;

                //public static EventureOrder populateOrderFromBundle(JObject saveBundle)
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
                db.Orders.Add(order);

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
                    //custDesc = "participant" + "_ord" + order.Id + "_id:" + order.HouseId;
                    //partEmail = "email" + "_ord" + order.Id + "_id:" + order.HouseId;
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
                            EventureOrderId = order.Id, //mjb1 this one seems ok
                            DateCreated = DateTime.Now,
                            TotalAmount = Convert.ToDecimal(regBundle.fee),
                            Type = "reg"
                        };
                    db.Registrations.Add(registration);
                    //db.SaveChanges();

                    var team = new Team
                        {
                            Name = regBundle.teamName, // (string)saveBundle["teamName"];
                            RegistrationId = registration.Id,
                            CoachId = order.HouseId,
                            OwnerId = order.OwnerId
                        };
                    db.Teams.Add(team);
                    db.SaveChanges();
                    teamId = team.Id;      //this sucks

                    //add coach to teamMember
                    var teamCoach = new TeamMember
                        {
                            Name = partName,
                            Email = partEmail,
                            TeamId = team.Id,
                            ParticipantId = order.HouseId,
                            Active = true
                        };
                    db.TeamMembers.Add(teamCoach);
                    db.SaveChanges();
                    teamMemberGuid = teamCoach.TeamMemberGuid.ToString().ToUpper();     //this is returned to app in response

                    var payment = new TeamMemberPayment();
                    //{
                    payment.TeamId = team.Id;
                    payment.Amount = order.Amount;
                    payment.TeamMemberId = teamCoach.Id;
                    //};
                    db.TeamMemberPayments.Add(payment);

                    foreach (dynamic teamBundle in regBundle.teamMembers)
                    //this is not nessessary now because only 1 reg in team
                    {
                        var teamMember = new TeamMember();
                        teamMember.Name = teamBundle.name;
                        teamMember.Email = teamBundle.email;
                        teamMember.TeamId = team.Id;
                        teamMember.Active = true;
                        db.TeamMembers.Add(teamMember);
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
                            EventureOrderId = order.Id,
                            DateCreated = DateTime.Now,
                            CouponId = surchargeBundle.couponId
                        };
                        totalFees = totalFees + Convert.ToDecimal(surchargeBundle.amount);
                        db.Surcharges.Add(surcharge);
                    }
                }

                Owner owner = db.Owners.Where(o => o.Id == order.OwnerId).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerPerChargeInCents);
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
                    AmountInCents = Convert.ToInt32(order.Amount * 100),
                    Currency = "usd",
                    CustomerId = customer.Id,
                    Description = owner.Name,
                    ApplicationFeeInCents = order.LocalApplicationFee
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
                    order.CardType = stripeCharge.StripeCard.Type;
                    order.Voided = false;
                    order.Status = "Complete";
                    order.PaymentType = "credit";
                    //db.Orders.Add(order);
                    db.SaveChanges();

                    //call mail
                    //HttpResponseMessage result = new MailController().SendConfirmMail(order.Id);
                    foreach (var member in db.TeamMembers.Where(m => m.TeamId == teamId))
                    {
                        //if member.partId is null
                        if (member.ParticipantId == null)
                        {
                            HttpResponseMessage result = new MailController().SendTeamPlayerInviteMail(member.Id);
                            //teamGuid = member.Team.TeamGuid.ToString().ToUpper(); //this sucks too!!
                        }
                    }

                    //return Request.CreateResponse(HttpStatusCode.OK, stripeCharge);

                    var resp = Request.CreateResponse(HttpStatusCode.OK);
                    //order.Id.ToString()  we are using teamGuid for demo because we already have 
                    //                     a getbyteamguid
                    resp.Content = new StringContent(teamMemberGuid.ToString(), Encoding.UTF8, "text/plain");    
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
                //HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException + " -- bundle: " + saveBundle,
                    Caller = "Payment_PostTeam",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime()
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
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage PostTeamPayment(JObject saveBundle)
        {
            try
            {
                decimal totalFees = 0;
                Int32 teamMemberId = (Int32)saveBundle["teamMemberId"];
                Int32 teamId = (Int32)saveBundle["teamId"];

                //public static EventureOrder populateOrderFromBundle(JObject saveBundle)
                var order = new EventureOrder
                {
                    DateCreated = DateTime.Now,
                    HouseId = 1,      //(Int32)saveBundle["participantId"],
                    Amount = (Decimal)saveBundle["orderAmount"],
                    Token = (string)saveBundle["orderToken"],
                    OwnerId = (Int32)saveBundle["ownerId"],  //need to send
                    Status = "Init",
                    Voided = false
                };
                db.Orders.Add(order);

                dynamic bundle = saveBundle;
                //foreach (dynamic regBundle in bundle.regs)  //this is not nessessary now because only 1 reg in team
                //{
                //    var registration = new Registration
                //    {
                //        EventureListId = regBundle.eventureListId,
                //        ParticipantId = regBundle.partId,
                //        ListAmount = regBundle.fee,
                //        Quantity = regBundle.quantity,
                //        EventureOrderId = order.Id,    //mjb1 this one seems ok
                //        DateCreated = DateTime.Now,
                //        TotalAmount = Convert.ToDecimal(regBundle.fee),
                //        Type = "reg"
                //    };
                //    db.Registrations.Add(registration);
                //}

                //var team = new Team
                //    {

                //    }

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


                Int32 participantId = 0;
                //if (bundle.participant != null)
                //{
                //    dynamic part = bundle.charges;

                //    var getPart = db.Participants.Where(p => p.Email == part.email).SingleOrDefault();
                //    if (getPart != null)
                //    {
                //        //enter part and get part id

                //    }
                //    else
                //    {
                //        participantId = getPart.Id;
                //    }

                //}

                var payment = new TeamMemberPayment();
                //{
                payment.TeamId = teamId;
                payment.Amount = order.Amount;
                payment.TeamMemberId = teamMemberId;
                //};
                db.TeamMemberPayments.Add(payment);

                Owner owner = db.Owners.Where(o => o.Id == 1).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                if (order.LocalApplicationFee < 0)
                    order.LocalApplicationFee = 0;

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
                    //custDesc = "participant" + "_ord" + order.Id + "_id:" + order.HouseId;
                    //partEmail = "email" + "_ord" + order.Id + "_id:" + order.HouseId;
                    throw new Exception("No Partipant found to match houseId ");
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
                    AmountInCents = Convert.ToInt32(order.Amount * 100),
                    Currency = "usd",
                    CustomerId = customer.Id,
                    Description = owner.Name,
                    ApplicationFeeInCents = order.LocalApplicationFee
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
                    order.CardType = stripeCharge.StripeCard.Type;
                    order.Voided = false;
                    order.Status = "Complete";
                    order.PaymentType = "credit";
                    //db.Orders.Add(order);
                    db.SaveChanges();


                    //flip flag

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
                HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException,
                    Caller = "Payment_Post",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime()
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
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage PostTest(JObject saveBundle)
        {
            //Uow.Sessions.Add(session);
            //Uow.Commit();
            //int rowsAdded = Convert.ToInt32((string) saveBundle["d"][0]["numberOfRowsAdded"]);
            //JsonConvert.DeserializeObject<Cart>(saveBundle);

            try
            {
                var log = new EventureLog();
                log.Message = "starting payment -- bundle: " + saveBundle;
                log.Caller = "StripePayment";
                log.Status = "Info";
                log.LogDate = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(log);
                db.SaveChanges();

                //return Request.CreateResponse(HttpStatusCode.OK);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                //resp.Headers.Add("Access-Control-Allow-Origin", "*");
                //resp.Headers.Add("Access-Control-Allow-Methods", "POST");

                return resp;


            }
            catch (Exception ex)
            {
                //send quick email
                //HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_PostTest: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException + "-- bundle: " + saveBundle,
                    Caller = "Payment_PostTest",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime()
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
    }
}