using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
//using System.Net.Mail;
//using SendGrid;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using evs.Model;  //just here for enums
using evs.DAL;  //get rid of this


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

        public Boolean SendConfirmEmail(Int32 orderId, Boolean isResend)
        {
            var _mailBuilder = new MailBuilder();
            var ownerId = 1;    //fix this
            
            List<string> addresses = new List<string>();
            addresses.Add("boone.mike@gmail.com");    //getEmailFromOrderId

            List<string> bcc= new List<string>(); 
            bcc = _mailBuilder.getEventureBcc();

            var subject = _mailBuilder.GetSubject(MailType.OrderConfirm, ownerId);
            var sender = _mailBuilder.GetSender(ownerId);

            return SendEmail(_mailBuilder.BuildConfirmEmailBody(orderId), subject, sender, addresses, bcc);
        }

        public Boolean SendResetPassword(string email, string resetCode, Int32 ownerId)
        {
            var _mailBuilder = new MailBuilder();
            
            List<string> addresses = new List<string>();
            addresses.Add(email);

            List<string> bcc= new List<string>();
            //bcc = _mailBuilder.getEventureBcc();

            var subject = _mailBuilder.GetSubject(MailType.ResetPassword, ownerId);
            var sender = _mailBuilder.GetSender(ownerId);

            return SendEmail(_mailBuilder.BuildResetPasswordBody(ownerId, resetCode), subject, sender, addresses, bcc);
        }

        private Boolean SendEmail(string messageBody, string messageSubject, string sender, List<string> mailTo, List<string> bcc)
        {
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(ConfigurationManager.AppSettings["AWSAccessKey"], ConfigurationManager.AppSettings["AWSKey"], Amazon.RegionEndpoint.USEast1);
            Destination destination = new Destination();
            destination.ToAddresses = mailTo;
            if (bcc.Count > 0)
            {
                destination.BccAddresses = mailTo;
            }
            Body body = new Body() { Html = new Content(messageBody) };
            Content subject = new Content(messageSubject);
            Message message = new Message(subject, body);
            SendEmailRequest sendEmailRequest = new SendEmailRequest(sender, destination, message);
            client.SendEmail(sendEmailRequest);

            return true;
        }
    }

  



}
