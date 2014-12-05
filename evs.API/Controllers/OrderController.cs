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

        //[HttpPost("create")]
        [Route("Post")]
        public HttpResponseMessage CreateOrder(JObject orderBundle)
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




             //orderAmount: cart.getTotalPrice(),
             //   orderHouseId: cart.houseId,
             //   ownerId: cart.ownerId,
             //   teamId: cart.teamId,
             //   teamMemberId: cart.teamMemberId,
             //   regs: cart.registrations


            //quick val  check required?

            //deserial obj

            //validate  //capacity and such

            //calculate fees

            //create stripe service,customer, charge

            //charge card

            //if good
                //record charege
                //create order

            //not good return reason




            //var order = BuildOrder(orderBundle);














            var resp = Request.CreateResponse(HttpStatusCode.OK);
            //resp.Content = new StringContent();
            resp.Content = new StringContent("69", Encoding.UTF8, "text/plain");
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


        //private EventureOrder BuildOrder(JObject saveBundle)
        //{
        //    var order = new EventureOrder
        //    {
        //        DateCreated = DateTime.Now,
        //        HouseId = (Int32)saveBundle["orderHouseId"],
        //        Amount = (Decimal)saveBundle["orderAmount"],
        //        Token = (string)saveBundle["stripeToken"],   //is this safe??
        //        OwnerId = (Int32)saveBundle["ownerId"],
        //        Status = "Init",
        //        Voided = false
        //    };
        //    //db.Orders.Add(order);
        //    dynamic bundle = saveBundle;
        //    foreach (var regBundle in bundle.regs)
        //    {

        //        var registration = new Registration
        //            {
        //                EventureListId = regBundle.eventureListId,
        //                ParticipantId = regBundle.partId,
        //                ListAmount = regBundle.fee,
        //                Quantity = regBundle.quantity,
        //                //EventureOrderId = order.Id,    
        //                EventureOrder = order,
        //                DateCreated = DateTime.Now,
        //                TotalAmount = Convert.ToDecimal(regBundle.fee),
        //                Type = "reg",
        //            };
        //        //var eventureListTypeId = regBundle.eventureListTypeId;
        //        //db.Registrations.Add(registration);
               

        //        foreach (var answerBundle in regBundle.answers)
        //        {
        //            var answer = new Answer
        //            {
        //                AnswerText = answerBundle.answer,
        //                QuestionId = answerBundle.questionId,
        //                Registration = registration
        //            };
        //            //registration.Answers.Add(answer);
        //            registration.Answers.Add(answer);
        //        }
        //        order.Registrations.Add(registration);
        //    }
            
        //    //return (StudentBo)new StudentBo().InjectFrom(studentViewModel);
        //    return order;
        //}
        
        

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
