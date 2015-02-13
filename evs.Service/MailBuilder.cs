using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
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

        public string GetSender(Int32 ownerId)
        {
            string sender = string.Empty;
            //repo:  getOwnersById
            var ownerInfo = db.Owners
              .Where(o => o.Id == ownerId)
              .Select(o => new
              {
                  o.SendMailEmailAddress
              }).FirstOrDefault();

            sender = ownerInfo.SendMailEmailAddress;

            if (string.IsNullOrEmpty(sender))
                sender = ConfigurationManager.AppSettings["DevEmail"];

            return sender;
        }

        public string GetSubject(MailType emailType, Int32 ownerId)
        {
            //repo:  getOwnersById
            var ownerInfo = db.Owners
             .Where(o => o.Id == ownerId)
             .Select(o => new
             {
                 o.SendConfirmEmailSubject,
                 o.Name,
                 //o.SendConfirmTeamEmailSubject,
                 o.SendConfirmTeamInviteEmailSubject
             }).FirstOrDefault();

            var subject = string.Empty;
            switch (emailType)
            {
                case MailType.OrderConfirm:
                    subject = ownerInfo.SendConfirmEmailSubject;
                    break;
                case MailType.TeamPlayerInvite:
                    subject = ownerInfo.SendConfirmTeamInviteEmailSubject;
                    break;
                case MailType.ResetPassword:
                    subject = ownerInfo.Name + " Password Assistance";
                    break;
                //default:
                //    Console.WriteLine("Default case");
                //    break;
            }

            if (subject.Length == 0)
                subject = "Thank You";

            if (ConfigurationManager.AppSettings["MailMode"] == "TEST")
                subject = "TEST: " + subject;

            return subject;
        }

        public List<string> getEventureBcc(Int32 ownerId)
        {
            List<string> bcc = new List<string>();

            if (ConfigurationManager.AppSettings["MailMode"] != "TEST")
            {
                //repo:  getOwnersById
                var ownerInfo = db.Owners
                                  .Where(o => o.Id == ownerId)
                                  .Select(o => new
                                      {
                                          o.Email
                                      }).FirstOrDefault();
                bcc.Add(ownerInfo.Email);
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminEmail"]))
                    bcc.Add(ConfigurationManager.AppSettings["AdminEmail"]);
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DevEmail"]))
                    bcc.Add(ConfigurationManager.AppSettings["DevEmail"]);
            }

            return bcc;
        }

        public List<string> getSendToAddressFromOrderId(Int32 orderId)
        {
            List<string> sendTo = new List<string>();
            if (ConfigurationManager.AppSettings["MailMode"] == "TEST")
                sendTo.Add(ConfigurationManager.AppSettings["DevEmail"]);
            else
            {
                //repo: getOrdersById
                var orderInfo = db.Orders
                                   .Where(o => o.Id == orderId)
                                   .Select(o => new
                                      {
                                          o.House.Email
                                      }).FirstOrDefault();
                sendTo.Add(orderInfo.Email);
            }
            return sendTo;
        }

        public string BuildConfirmEmailBody(Int32 orderId)
        {
            string body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("/Content/EmailTemplates/order-confirmation.min.html"));

            //repo getOrdersById
            var orderInfo = db.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new
               {
                   o.Id,
                   o.DateCreated,
                   o.House.FirstName,
                   o.House.LastName,
                   o.Owner.MainColor,
                   o.Owner.Name,
                   o.Owner.GroupName,
                   o.Owner.ListingName,
                   o.Owner.Url,
                   o.Owner.LogoImageName,
                   o.Owner.SupportPhone,
                   o.Owner.SupportEmail
               }).FirstOrDefault();

            Dictionary<string, string> replaceTokens = new Dictionary<string, string>();
            replaceTokens.Add("COMPANYNAME", orderInfo.Name);
            replaceTokens.Add("ORDERDATE", orderInfo.DateCreated.ToString("MMMM dd, yyyy"));
            replaceTokens.Add("ORDERNUMBER", orderId.ToString());
            replaceTokens.Add("GROUPNAME", orderInfo.GroupName);
            replaceTokens.Add("LISTINGNAME", orderInfo.ListingName);
            replaceTokens.Add("PARTNAME", orderInfo.FirstName + " " + orderInfo.LastName);
            replaceTokens.Add("SUPPORTEMAIL", orderInfo.SupportEmail);
            replaceTokens.Add("SUPPORTPHONE", " " + orderInfo.SupportPhone);  //added " " because the minification removes it
            replaceTokens.Add("MAINCOLOR", orderInfo.MainColor);
            if (ConfigurationManager.AppSettings["MailMode"] == "TEST")
                replaceTokens.Add("IMAGEURL", ConfigurationManager.AppSettings["TestLogo"]);
            else
                replaceTokens.Add("IMAGEURL", orderInfo.Url + "/Content/images/" + orderInfo.LogoImageName);
            replaceTokens.Add("TABLEBODY", BuildOrderSummaryTable(orderId));

            replaceTokens.Select(a => body = body.Replace(string.Concat("{{", a.Key, "}}"), a.Value)).ToList();

            return body;
        }

        public string BuildResetPasswordBody(Int32 ownerId, string resetCode)
        {
            string body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("/Content/EmailTemplates/reset-password.min.html"));

            //move this call to respository
            var ownerInfo = db.Owners
              .Where(o => o.Id == ownerId)
              .Select(o => new
              {
                  o.Name,
                  o.Url,
                  o.SupportEmail,
                  o.SupportPhone,
                  o.MainColor,
                  o.LogoImageName
              }).FirstOrDefault();

            Dictionary<string, string> replaceTokens = new Dictionary<string, string>();
            replaceTokens.Add("COMPANYNAME", ownerInfo.Name);
            replaceTokens.Add("URL", ownerInfo.Url);
            if (ConfigurationManager.AppSettings["MailMode"] == "TEST")
                replaceTokens.Add("IMAGEURL", ConfigurationManager.AppSettings["TestLogo"]);
            else
                replaceTokens.Add("IMAGEURL", ownerInfo.Url + "/Content/images/" + ownerInfo.LogoImageName);
            replaceTokens.Add("SUPPORTEMAIL", ownerInfo.SupportEmail);
            replaceTokens.Add("SUPPORTPHONE", ownerInfo.SupportPhone);
            replaceTokens.Add("RESETPASSWORDURL", ownerInfo.Url + "#/resetpassword?userId=" + resetCode);
            replaceTokens.Add("MAINCOLOR", ownerInfo.MainColor);

            replaceTokens.Select(a => body = body.Replace(string.Concat("{{", a.Key, "}}"), a.Value)).ToList();

            return body;
        }

        private string BuildOrderSummaryTable(int orderId)
        {
            try
            {
                //repo: getRegsoByOrderId
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
                               //o.Id,
                               //o.DateCreated,
                               r.EventureList.DisplayName,
                               p.FirstName,
                               p.LastName,
                               r.Quantity,
                               regGroup = r.EventureGroup.Name,   //this can be null but doesn't seem to cause an issue
                               r.ListAmount,
                               e.DisplayHeading,
                               regQuantity = r.Quantity,
                           };

                //repo:  getSurchargesByOrderId
                var fees = from s in db.Surcharges
                           where s.EventureOrderId == orderId
                           select new { s.Amount, s.Description };

                string summaryTable = "<tbody>";
                int numReg = 0;
                decimal orderAmount = 0;

                foreach (var reg in regs)
                {
                    summaryTable = summaryTable + "<TR><TD>" + reg.DisplayName + "</TD><TD>" + reg.FirstName + " " + reg.LastName + "</TD><TD>" +
                                reg.regGroup + "</TD><TD Align=\"center\">" + reg.Quantity + "</TD><TD Align=\"center\">" + reg.ListAmount + "</TD></TR>";
                    numReg = numReg + reg.regQuantity;
                    orderAmount = orderAmount + (reg.ListAmount * reg.Quantity);
                }
                summaryTable = summaryTable + "<TR><TD></TD><TD></TD><TD>&nbsp;</TD><TD></TD><TD></TD></TR>";  //empty row
                
                foreach (var fee in fees)
                {
                    summaryTable = summaryTable + "<TR><TD></TD><TD></TD><TD Align=\"center\">" + fee.Description + "</TD><TD></TD><TD Align=\"center\">" + fee.Amount + "</TD></TR>";
                    orderAmount = orderAmount + fee.Amount;
                }
                summaryTable = summaryTable + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";
                summaryTable = summaryTable + "<TR><TD></TD><TD></TD><TD Align=\"center\">" + "Total" + "</TD><TD></TD><TD Align=\"center\">" + orderAmount + "</TD></TR>";
                summaryTable = summaryTable + "</tbody>";

                return summaryTable;
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                return test;
            }

        }

    }
}
