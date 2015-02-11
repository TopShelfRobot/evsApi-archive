using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using evs.DAL;
using evs.Model;


namespace evs.Service
{
    public class MailBuilder
    {
        readonly evsContext db = new evsContext();   //move to respoitory

        public MailBuilder()  //ModelStateDictionary modelState, IOrderRepository repository
        {
            // _modelState = modelState;
            //_repository = repository;
        }




        public string GetSender()
        {
            return "boone.mike@gmail.com";
        }

        public string GetSubject(MailType emailType)
        {

            return "important stuff";
        }

        public string BuildResetPasswordBody(Int32 ownerId, string resetCode)
        {
            string body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("/Content/EmailTemplates/reset-password.html"));

            //move this call to respository
            var ownerInfo = db.Owners
              .Where(o => o.Id == ownerId)
              .Select(o => new
              {
                  o.Name,
                  o.Url,
                  o.SupportEmail,
                  o.SupportPhone,
                  o.LogoImageName
              }).FirstOrDefault();

            Dictionary<string, string> replaceTokens = new Dictionary<string, string>();
            replaceTokens.Add("COMPANYNAME", ownerInfo.Name);
            replaceTokens.Add("URL", ownerInfo.Url);
            replaceTokens.Add("IMAGEURL", ownerInfo.Url + "/Content/images/" + ownerInfo.LogoImageName);
            replaceTokens.Add("SUPPORTEMAIL", ownerInfo.SupportEmail);
            replaceTokens.Add("SUPPORTPHONE", ownerInfo.SupportPhone);
            replaceTokens.Add("RESETPASSWORDURL", ownerInfo.Url + "#/resetpassword?userId=" + resetCode);
        
            replaceTokens.Select(a => body = body.Replace(string.Concat("{{", a.Key, "}}"), a.Value)).ToList();

            return body;
        }

        private string BuildOrderSummaryTable(int orderId)
        {
           try
            {
                var regs = from o in db.Orders
                           join r in db.Registrations
                               on o.Id equals r.EventureOrderId
                           join h in db.Participants
                               on o.HouseId equals h.Id
                           join p in db.Participants
                              on r.ParticipantId equals p.Id
                           join e in db.Eventures
                                on r.EventureList.EventureId equals e.Id
                           where o.Id == orderId
                           select new
                           {
                               o.Id,
                               o.DateCreated,
                               r.EventureList.DisplayName,
                               p.FirstName,
                               p.LastName,
                               r.Quantity,
                               r.ListAmount,
                               partEmail = p.Email,
                               e.DisplayHeading,
                               houseFirst = h.FirstName,
                               houseLast = h.LastName,
                               houseEmail = h.Email,
                               regQuantity = r.Quantity,
                               e.OwnerId
                           };

                var fees = from s in db.Surcharges
                           where s.EventureOrderId == orderId
                           select new { s.Amount, s.Description };

                string emailText = string.Empty;
                Int32 ownerId = 0;
                string houseName = string.Empty;
                //string carriageReturn = "<BR>";
                string orderNum = string.Empty;
                string houseEmail = string.Empty;
                string lineItems = "<TABLE cellpadding=\"8\" cellspacingBono=\"8\"><tr><td>Events</td><td>Listings</td><td>Participants</td><td>Quantity</td><td>Price</td></tr>";
                int numReg = 0;
                decimal orderAmount = 0;

                foreach (var reg in regs)
                {
                    houseName = reg.houseFirst + " " + reg.houseLast;
                    orderNum = Convert.ToString(reg.Id);

                    lineItems = lineItems + "<TR><TD>" + reg.DisplayHeading + "</TD><TD>" + reg.DisplayName + "</TD><TD>" +
                                reg.FirstName + " " + reg.LastName + "</TD><TD Align=\"right\">" + reg.Quantity + "</TD><TD Align=\"right\">" + reg.ListAmount + "</TD></TR>";
                    numReg = numReg + reg.regQuantity;
                    orderAmount = orderAmount + (reg.ListAmount * reg.Quantity);
                }

                lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";

                foreach (var fee in fees)
                {
                    lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + fee.Description + "</TD><TD></TD><TD Align=\"right\">" + fee.Amount + "</TD></TR>";
                    orderAmount = orderAmount + fee.Amount;
                }

                lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";
                lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + "Total" + "</TD><TD></TD><TD Align=\"right\">" + orderAmount + "</TD></TR>";

                lineItems = lineItems + "</TABLE>";


                emailText = emailText + "Order Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                emailText = emailText + "Dear " + houseName + ",<BR><BR>Thank you for purchasing your registration. This email serves as your receipt. Your confirmation number is " + orderNum + ". <BR><BR><BR>You have been charged for the following:";
                emailText = emailText + "<BR>" + lineItems;

                return emailText;

            }
            catch (Exception ex)
            {
                var test = ex.Message;
                return test;
            }

        }

    }
}
