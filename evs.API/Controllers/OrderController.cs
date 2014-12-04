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
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
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
