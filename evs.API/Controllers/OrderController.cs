using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using evs.Model;
using evs.Service;
using evs.DAL;
using System.Configuration;
using Stripe;

namespace evs.API.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
    {
        //[Authorize]
        //[Route("")]
        //public IHttpActionResult Get()
        //{
        //    return Ok(Order.CreateOrders());
        //}


        //readonly IOrderService _orderService;

        //public OrderController(IOrderService orderService)
        //{
        //    _orderService = orderService;
        //}

        //public OrderController()
        //{

        //}

        readonly evsContext db = new evsContext();

        //[HttpPost("create")]
        [Route("Post")]
        [HttpPost]
        public HttpResponseMessage CreateOrder(JObject orderBundle)
        {
            //TransactionStatus transactionStatus;
            // var results = new StudentValidation().Validate(studentViewModel);

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

            int place = 0;

            try
            {

                //quick val  check required?

                //deserial obj
                var order = BuildOrder(orderBundle);

                //validate  //capacity and such

                //calculate fees

                place = 1;


                Owner owner = db.Owners.Where(o => o.Id == 1).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }


                //var custDesc = string.Empty;
                //var partEmail = string.Empty;
                //var partName = string.Empty;
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



                place = 2;

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                if (order.LocalApplicationFee < 0)
                    order.LocalApplicationFee = 0;
                //}

                //create stripe service,customer, charge
                //charge card
                //var stripeService = new Stripe.

                //if good
                //record charege
                //create order
                //StripeService stripeService = new StripeService();

                //tring customerEmail,string customerDescription, string customerToken, string accessToken, string chargeDescription, decimal chargeAmount, Int32 applicationFee
                //var stripeCharge = stripeService.CreateCharge(part.Email, part.FirstName + " " + part.LastName, order.Token, owner.AccessToken, owner.Name, order.Amount, order.LocalApplicationFee);
                //         public StripeCharge CreateCharge(string customerEmail,string customerDescription, string customerToken, string accessToken, string chargeDescription, decimal chargeAmount, Int32 applicationFee )
                //{

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


                //if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                if (place == 9)
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
                    //mjb fix order.PaymentType = "credit";
                    //db.Orders.Add(order);
                    //db.SaveChanges();

                    place = 7;

                    //not good return reason
                    OrderService _orderService = new OrderService();
                    var x = _orderService.CreateOrder(order);

                    place = 8;

                    HttpResponseMessage result;

                    if (ConfigurationManager.AppSettings["CustomName"] == "bourbonchase")
                        result = new MailController().SendBourbonLotteryConfirm(order.Id);
                    else
                        result = new MailController().SendConfirmMail(order.Id);
                    //HttpResponseMessage result = new MailController().SendTestEmail();

                    var resp = Request.CreateResponse(HttpStatusCode.OK);
                    //resp.Content = new StringContent();
                    resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                    return resp;
                }
                else
                {
                    //order.Status = stripeCharge.FailureMessage;
                    //db.SaveChanges();
                    //return 
                    //var badResponse = Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);  //stripeCharge.FailureCode
                    //return badResponse;

                    var logE = new EventureLog();
                    logE.Message = "Stripe Exception: " + stripeCharge.FailureMessage + " -- place: " + place + " -- bundle: " + orderBundle;
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
                var logE = new EventureLog();
                logE.Message = "Order exception: " + ex.Message + " -- place: " + place + " -- bundle: " + orderBundle;
                logE.Caller = "Order_ERROR";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
                logE.DateCreated = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                //var x = "there was an issue";
                //var badResponse = Request.CreateResponse(HttpStatusCode.BadRequest, x.ToString());
                //return badResponse;

                var badResp = Request.CreateResponse(HttpStatusCode.BadRequest);
                //resp.Content = new StringContent();
                badResp.Content = new StringContent("There was a problem with your order.  Please try again", Encoding.UTF8, "text/plain");
                return badResp;

            }


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

        [Route("PostZero")]
        [HttpPost]
        public HttpResponseMessage CreateZeroAmountOrder(JObject orderBundle)
        {
            int place = 0;
            try
            {
                //deserial obj
                var order = BuildOrder(orderBundle);
                //validate  //capacity and such
                //calculate fees
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

                place = 2;

                ////orderService.CompleteOrder(stripeCharge)
                //order.AuthorizationCode = stripeCharge.Id;
                ////stripeCharge.
                //order.CardNumber = stripeCharge.StripeCard.Last4;
                //order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                //order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                //order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                ////order.CardId = stripeCharge.StripeCard.;
                //order.CardName = stripeCharge.StripeCard.Name;
                //order.CardOrigin = stripeCharge.StripeCard.Country;
                //mjb fixorder.CardType = stripeCharge.StripeCard.Type;
                order.Voided = false;
                order.Status = "Complete";
                //mjb fix order.PaymentType = "credit";
                //db.Orders.Add(order);
                //db.SaveChanges();

                place = 7;

                //not good return reason
                OrderService _orderService = new OrderService();
                var x = _orderService.CreateOrder(order);

                place = 8;

                HttpResponseMessage result;

                if (ConfigurationManager.AppSettings["CustomName"] == "bourbonchase")
                    result = new MailController().SendBourbonLotteryConfirm(order.Id);
                else
                    result = new MailController().SendConfirmMail(order.Id);
                //HttpResponseMessage result = new MailController().SendTestEmail();

                var resp = Request.CreateResponse(HttpStatusCode.OK);
                //resp.Content = new StringContent();
                resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                return resp;
            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "Order exception: " + ex.Message + " -- place: " + place + " -- bundle: " + orderBundle;
                logE.Caller = "OrderZERo_ERROR";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
                logE.DateCreated = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                var badResp = Request.CreateResponse(HttpStatusCode.BadRequest);
                //resp.Content = new StringContent();
                badResp.Content = new StringContent("There was a problem with your order.  Please try again", Encoding.UTF8, "text/plain");
                return badResp;
            }
        }


        private EventureOrder BuildOrder(JObject orderBundle)
        {
            //orderAmount: cart.getTotalPrice(),
            //   orderHouseId: cart.houseId,
            //   ownerId: cart.ownerId,
            //   teamId: cart.teamId,
            //   teamMemberId: cart.teamMemberId,
            //   regs: cart.registrations

            var order = new EventureOrder
            {
                DateCreated = DateTime.Now,
                HouseId = (Int32)orderBundle["orderHouseId"],
                Amount = (Decimal)orderBundle["orderAmount"],
                Token = (string)orderBundle["stripeToken"],   //is this safe??
                OwnerId = (Int32)orderBundle["ownerId"],
                Status = "Init",
                Voided = false
            };

            dynamic bundle = orderBundle;
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
                        ParticipantId = (Int32)orderBundle["orderHouseId"],  //surchargeBundle.partId,
                        //EventureOrderId = order.Id,
                        EventureOrder = order,
                        DateCreated = DateTime.Now,
                        CouponId = surchargeBundle.couponId
                    };
                    //totalFees = 33; //totalFees + Convert.ToDecimal(surchargeBundle.amount);
                    order.Surcharges.Add(surcharge);
                }
            }
            //db.Orders.Add(order);

            foreach (var regBundle in bundle.regs)
            {


                Registration registration = new Registration
                    {
                        EventureListId = regBundle.eventureListId,
                        ParticipantId = regBundle.partId,
                        ListAmount = regBundle.fee,
                        Quantity = regBundle.quantity,
                        //EventureOrderId = order.Id,   
                        GroupId = regBundle.groupId,
                        EventureOrder = order,
                        DateCreated = DateTime.Now,
                        TotalAmount = Convert.ToDecimal(regBundle.fee),
                        Type = "reg",
                        Redeemed = true
                    };
                // order.

                var eventureListTypeId = regBundle.eventureListTypeId;
                order.Registrations.Add(registration);

                foreach (var answerBundle in regBundle.answers)
                {
                    var answer = new Answer
                    {
                        AnswerText = answerBundle.answer,
                        QuestionId = answerBundle.questionId,
                        Registration = registration
                    };
                    //registration.Answers.Add(answer);
                    registration.Answers.Add(answer);
                }
            }
            return order;
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
