using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.Mail;
using SendGrid;

using evs.DAL;

namespace evs.Service
{
    public class MailService
    {

        readonly evsContext db = new evsContext();

        public MailService()  //ModelStateDictionary modelState, IOrderRepository repository
        {
            // _modelState = modelState;
            //_repository = repository;
            
        }

        public bool SendResetPassword(string email, string subject, string body)
        {
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("boone.mike@gmail.com");

            // Add multiple addresses to the To field.
            List<String> recipients = new List<String>();

            recipients.Add(email);
            //{
            //    //@"Jeff Smith <jeff@example.com>",
            //    //@"Anna Lidman <anna@example.com>",
            //    @"Mike Boone <boone@firstegg.com>"
            //};

            myMessage.AddTo(recipients);
            myMessage.Subject = subject;
            myMessage.Html = body;
            //Add the HTML and Text bodies
            //myMessage.Html = "<p>Hello World!</p>";
            //myMessage.Text = "Hello World plain text!";
         

            // Create network credentials to access your SendGrid account.
            var username = "boone10";
            var pswd = "fr33_b33r";

            var credentials = new NetworkCredential(username, pswd);
            //To send an email message, use the Deliver method on the Web transport class, which calls the SendGrid Web API. The following example shows how to send a message.

            // Create the email object first, then add the properties.
            //SendGridMessage myMessage = new SendGridMessage();
            //myMessage.AddTo("anna@example.com");
            //myMessage.From = new MailAddress("john@example.com", "John Smith");
            //myMessage.Subject = "Testing the SendGrid Library";
            //myMessage.Text = "Hello World!";

            // Create credentials, specifying your user name and password.
            //var credentials = new NetworkCredential("username", "password");

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            transportWeb.Deliver(myMessage);



            return true;
        }

 public bool SendConfirmEmail(string email, string subject, string body)
        {
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("boone.mike@gmail.com");

            // Add multiple addresses to the To field.
            List<String> recipients = new List<String>();

            recipients.Add(email);
            //{
            //    //@"Jeff Smith <jeff@example.com>",
            //    //@"Anna Lidman <anna@example.com>",
            //    @"Mike Boone <boone@firstegg.com>"
            //};

            myMessage.AddTo(recipients);
            myMessage.Subject = subject;
            myMessage.Html = body;
            //Add the HTML and Text bodies
            //myMessage.Html = "<p>Hello World!</p>";
            //myMessage.Text = "Hello World plain text!";
         

            // Create network credentials to access your SendGrid account.
            var username = "boone10";
            var pswd = "fr33_b33r";

            var credentials = new NetworkCredential(username, pswd);
            //To send an email message, use the Deliver method on the Web transport class, which calls the SendGrid Web API. The following example shows how to send a message.

            // Create the email object first, then add the properties.
            //SendGridMessage myMessage = new SendGridMessage();
            //myMessage.AddTo("anna@example.com");
            //myMessage.From = new MailAddress("john@example.com", "John Smith");
            //myMessage.Subject = "Testing the SendGrid Library";
            //myMessage.Text = "Hello World!";

            // Create credentials, specifying your user name and password.
            //var credentials = new NetworkCredential("username", "password");

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            transportWeb.Deliver(myMessage);



            return true;
        }


        //private string BuildConfirmMail(int id)
        //{
        //    //needs to be a send amazon email
        //    //Type : confirm, reset password
        //    // to, cc, bc, sender, reply, subject, text

        //    try
        //    {
        //        var regs = from o in db.Orders
        //                   join r in db.Registrations
        //                       on o.Id equals r.EventureOrderId
        //                   join h in db.Participants
        //                       on o.HouseId equals h.Id
        //                   join p in db.Participants
        //                      on r.ParticipantId equals p.Id
        //                   join e in db.Eventures
        //                        on r.EventureList.EventureId equals e.Id
        //                   where o.Id == id
        //                   select new
        //                   {
        //                       o.Id,
        //                       o.DateCreated,
        //                       r.EventureList.DisplayName,
        //                       p.FirstName,
        //                       p.LastName,
        //                       r.Quantity,
        //                       r.ListAmount,
        //                       partEmail = p.Email,
        //                       e.DisplayHeading,
        //                       houseFirst = h.FirstName,
        //                       houseLast = h.LastName,
        //                       houseEmail = h.Email,
        //                       regQuantity = r.Quantity,
        //                       e.OwnerId
        //                   };

        //        var fees = from s in db.Surcharges
        //                   where s.EventureOrderId == id
        //                   select new { s.Amount, s.Description };

        //        Int32 ownerId = 0;
        //        string houseName = string.Empty;
        //        //string carriageReturn = "<BR>";
        //        string orderNum = string.Empty;
        //        string houseEmail = string.Empty;
        //        string lineItems = "<TABLE cellpadding=\"8\" cellspacingBono=\"8\"><tr><td>Events</td><td>Listings</td><td>Participants</td><td>Quantity</td><td>Price</td></tr>";
        //        int numReg = 0;
        //        decimal orderAmount = 0;

        //        foreach (var reg in regs)
        //        {
        //            houseName = reg.houseFirst + " " + reg.houseLast;
        //            orderNum = Convert.ToString(reg.Id);

        //            lineItems = lineItems + "<TR><TD>" + reg.DisplayHeading + "</TD><TD>" + reg.DisplayName + "</TD><TD>" +
        //                        reg.FirstName + " " + reg.LastName + "</TD><TD Align=\"right\">" + reg.Quantity + "</TD><TD Align=\"right\">" + reg.ListAmount + "</TD></TR>";
        //            numReg = numReg + reg.regQuantity;
        //            orderAmount = orderAmount + (reg.ListAmount * reg.Quantity);

        //            //this is a little ugly but works
        //            ownerId = reg.OwnerId;
        //            houseEmail = reg.houseEmail;
        //        }

        //        lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";

        //        foreach (var fee in fees)
        //        {
        //            lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + fee.Description + "</TD><TD></TD><TD Align=\"right\">" + fee.Amount + "</TD></TR>";
        //            orderAmount = orderAmount + fee.Amount;
        //        }

        //        lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";
        //        lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + "Total" + "</TD><TD></TD><TD Align=\"right\">" + orderAmount + "</TD></TR>";

        //        lineItems = lineItems + "</TABLE>";

        //        //string numOfRegs = Convert.ToString(numReg);



        //        emailText = emailText + "Order Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
        //        emailText = emailText + "Dear " + houseName + ",<BR><BR>Thank you for purchasing your registration. This email serves as your receipt. Your confirmation number is " + orderNum + ". <BR><BR><BR>You have been charged for the following:";
        //        emailText = emailText + "<BR>" + lineItems;

        //    }
        //    catch (Exception ex)
        //    {
        //        var test = ex.Message;
        //    }

        //}




        public bool SendSingleEmail(string email, string subject, string body)
        {

            var myMessage = new SendGridMessage();

            myMessage.From = new MailAddress("boone.mike@gmail.com");
            List<String> recipients = new List<String>();

            recipients.Add(email);
            //{
            //    //@"Jeff Smith <jeff@example.com>",
            //    //@"Anna Lidman <anna@example.com>",
            //    @"Mike Boone <boone@firstegg.com>"
            //};

            myMessage.AddTo(recipients);
            myMessage.Subject = subject;
            myMessage.Html = body;

            // Create network credentials to access your SendGrid account.
            var username = "boone10";
            var pswd = "fr33_b33r";

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential(username, pswd);
            var transportWeb = new Web(credentials);

            try
            {
                transportWeb.Deliver(myMessage);
            }
            catch(Exception ex)
            {
                var test = ex.Message;
            }
            
            return true;
        }



    }





}
