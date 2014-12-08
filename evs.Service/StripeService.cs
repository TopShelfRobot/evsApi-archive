using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;

namespace evs.Service
{
    public class StripeService
    {
        public StripeCharge CreateCharge(string customerEmail,string customerDescription, string customerToken, string accessToken, string chargeDescription, decimal chargeAmount, Int32 applicationFee )
        {
            // create charge
            var customerOptions = new StripeCustomerCreateOptions
            {
                Email = customerEmail, //Email,
                Description = customerDescription,
                TokenId = customerToken,
            };

            var stripeCustomerService = new StripeCustomerService(accessToken);   //owner.AccessToken
            var customer = stripeCustomerService.Create(customerOptions);

            var stripeChargeService = new StripeChargeService(accessToken); //The token returned from the above method
            var stripeChargeOption = new StripeChargeCreateOptions()
            {
                Amount = Convert.ToInt32(chargeAmount * 100),
                Currency = "usd",
                CustomerId = customer.Id,
                Description = chargeDescription,
                ApplicationFee = applicationFee
            };
            return stripeChargeService.Create(stripeChargeOption);
        }


    }

    //public bool CreateOrder(EventureOrder orderToCreate)
    //{
    //    // Validation logic
    //    //if (!ValidateOrder(orderToCreate))
    //    //    return false;

    //    // Database logic
    //    try
    //    {
    //        // _repository.CreateProduct(orderToCreate);
    //        db.Orders.Add(orderToCreate);
    //        db.SaveChanges();
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //    return true;
    //}
}
