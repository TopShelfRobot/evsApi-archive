using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace evs.API.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        //[Authorize]
        //[Route("")]
        //public IHttpActionResult Get()
        //{
        //    return Ok(Order.CreateOrders());
        //}


        //readonly IStudentService _studentService;       

        //public StudentApiController(IStudentService studentService)
        //{
        //    _studentService = studentService;           
        //}

        //[HttpPost("create")]
        [Route("Post")]
        public HttpResponseMessage CreateOrder(JObject saveBundle)
        {
            //TransactionStatus transactionStatus;
            var results = 54; //new StudentValidation().Validate(studentViewModel);

            //if (!results.IsValid)
            //{
            //    studentViewModel.Errors = GenerateErrorMessage.Built(results.Errors);
            //    studentViewModel.ErrorType = ErrorTypeEnum.Error.ToString().ToLower();
            //    studentViewModel.Status = false;
            //    var badResponse = Request.CreateResponse(HttpStatusCode.BadRequest, studentViewModel);
            //    return badResponse;
            //}

            //var stundentBo = BuiltStudentBo(studentViewModel);
            //stundentBo.PaymentMethods = string.Join(",", studentViewModel.SelectedPaymentMethods);
            //stundentBo.Gender = studentViewModel.SelectedGender;

            //transactionStatus = _studentService.CreateStudent(stundentBo);



            var resp = Request.CreateResponse(HttpStatusCode.OK);
            //resp.Content = new StringContent();
            resp.Content = new StringContent("17", Encoding.UTF8, "text/plain");
            return resp;
            
            //if (transactionStatus.Status == false)
            //{
            //    var badResponse = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(studentViewModel));
            //    return badResponse;
            //}
            //else
            //{
            //    transactionStatus.ErrorType = ErrorTypeEnum.Success.ToString();
            //    transactionStatus.ReturnMessage.Add("Record successfully inserted to database");

            //    var badResponse = Request.CreateResponse(HttpStatusCode.Created, transactionStatus);

            //    return badResponse;
            //}
        }

        [Route("Post1")]
        public HttpRequestMessage CreateOrderOld(JObject saveBundle)
        {
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

    }







    //#region Helpers

    //public class Order
    //{
    //    public int OrderID { get; set; }
    //    public string CustomerName { get; set; }
    //    public string ShipperCity { get; set; }
    //    public Boolean IsShipped { get; set; }

    //    public static List<Order> CreateOrders()
    //    {
    //        List<Order> OrderList = new List<Order> 
    //        {
    //            new Order {OrderID = 10248, CustomerName = "Taiseer Joudeh", ShipperCity = "Amman", IsShipped = true },
    //            new Order {OrderID = 10249, CustomerName = "Ahmad Hasan", ShipperCity = "Dubai", IsShipped = false},
    //            new Order {OrderID = 10250,CustomerName = "Tamer Yaser", ShipperCity = "Jeddah", IsShipped = false },
    //            new Order {OrderID = 10251,CustomerName = "Lina Majed", ShipperCity = "Abu Dhabi", IsShipped = false},
    //            new Order {OrderID = 10252,CustomerName = "Yasmeen Rami", ShipperCity = "Kuwait", IsShipped = true}
    //        };

    //        return OrderList;
    //    }
    //}
    //#endregion
}
