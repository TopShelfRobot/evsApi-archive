using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
using evs.Model;
using evs.DAL;
using Stripe;
//using System.Web.ModelBinding.ModelStateDictionary;

namespace evs.Service
{
    public class OrderService : IOrderService
    {
        //private ModelStateDictionary _modelState;
        //private IOrderRepository _repository;
        readonly evsContext db = new evsContext();

        public OrderService()  //ModelStateDictionary modelState, IOrderRepository repository
        {
            // _modelState = modelState;
            //_repository = repository;
        }GetTeamRegistrationsByCoachId

        protected bool ValidateOrder(EventureOrder orderToValidate)
        {
            //if (productToValidate.Name.Trim().Length == 0)
            //    _modelState.AddModelError("Name", "Name is required.");
            //if (productToValidate.Description.Trim().Length == 0)
            //    _modelState.AddModelError("Description", "Description is required.");
            //if (productToValidate.UnitsInStock < 0)
            //    _modelState.AddModelError("UnitsInStock", "Units in stock cannot be less than zero.");
            //return _modelState.IsValid;
            return true;
        }

        //public IEnumerable<Order> ListOrders()
        //{
        //    return _repository.ListProducts();
        //}

        public bool CreateOrder(EventureOrder orderToCreate)
        {
            // Validation logic
            //if (!ValidateOrder(orderToCreate))
            //    return false;

            // Database logic
            try
            {
                // _repository.CreateProduct(orderToCreate);
                db.Orders.Add(orderToCreate);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    public class TransactionManager : ITransactionManager
    {
        //private ModelStateDictionary _modelState;
        //private IOrderRepository _repository;
        readonly evsContext db = new evsContext();

        public TransactionManager()  //ModelStateDictionary modelState, IOrderRepository repository
        {
            // _modelState = modelState;
            //_repository = repository;
        }

        //protected bool ValidateOrder(EventureOrder orderToValidate)
        //{
        //    //if (productToValidate.Name.Trim().Length == 0)
        //    //    _modelState.AddModelError("Name", "Name is required.");
        //    //if (productToValidate.Description.Trim().Length == 0)
        //    //    _modelState.AddModelError("Description", "Description is required.");
        //    //if (productToValidate.UnitsInStock < 0)
        //    //    _modelState.AddModelError("UnitsInStock", "Units in stock cannot be less than zero.");
        //    //return _modelState.IsValid;
        //    return true;
        //}



        public string ProcessRefund(Refund refund)
        {
            // Validation logic
            //if (!ValidateOrder(orderToCreate))
            //    return false;

            // Database logic
            try
            {
                //stripe.process refund

                var order = db.Orders.Where(o => o.Id == refund.EventureOrderId).FirstOrDefault();


                Int32 amountInCents = Convert.ToInt32(refund.Amount * 100);   //Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100)

                var chargeService = new StripeChargeService();
                //StripeCharge stripeCharge = chargeService.Refund(*chargeId*, *amount*, *refundApplicationFee*);
                StripeCharge stripeCharge = chargeService.Refund(order.AuthorizationCode, amountInCents, false);
                
                refund.AmountRefunded = stripeCharge.AmountRefunded;
                //refund.BalanceTransaction = stripeCharge.BalanceTransaction;
                refund.BalanceTransactionId = stripeCharge.BalanceTransactionId;
                refund.CustomerId = stripeCharge.CustomerId;
                refund.FailureMessage = stripeCharge.FailureMessage;

                refund.Paid = stripeCharge.Paid;
                refund.ReceiptEmail = stripeCharge.ReceiptEmail;
                refund.Refunded = stripeCharge.Refunded;

                refund.DateCreated = DateTime.Now;
                db.Refunds.Add(refund);
                db.SaveChanges();

                return "Success:  A refund of $ " + stripeCharge.AmountRefunded + " has been applied";

                //create transaction ( orderid, refundId)

            }
            catch (Exception ex)
            {
                string message = string.Empty;
                if (ex.Source == "Stripe.net")
                    message = ex.Message;
                else{
                    message = "There was an error processing this refund.";
                        //TODO:  log real error
                }

                return message;
            }
        }
    }


    public interface IOrderService
    {
        bool CreateOrder(EventureOrder orderToCreate);
        //IEnumerable<Order> ListOrders();
    }


    public interface ITransactionManager
    {
        //bool CreateOrder(EventureOrder orderToCreate);
        //IEnumerable<Order> ListOrders();
    }
}
