using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.Mail;
using SendGrid;

namespace evs.Service
{
    public class MailService
    {
        //D:\source\evs30Api\evs.Service\OrderService.cs
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

            //Add the HTML and Text bodies
            //myMessage.Html = "<p>Hello World!</p>";
            //myMessage.Text = "Hello World plain text!";

            myMessage.Html = body;


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






    }





}
