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

            var sender = _mailBuilder.GetSender(ownerId);   //amazon send email

            List<string> addresses = new List<string>();
            addresses = _mailBuilder.getSendToAddressFromOrderId(orderId);    //house email

            List<string> bcc = new List<string>();
            if (!isResend)
                bcc = _mailBuilder.getEventureBcc(ownerId);    //boone, paul, confirm email

            var subject = _mailBuilder.GetSubject(MailType.OrderConfirm, ownerId);


            return SendEmail(_mailBuilder.BuildConfirmEmailBody(orderId), subject, sender, addresses, bcc);
        }

        public Boolean SendResetPassword(string email, string resetCode, Int32 ownerId)
        {
            var _mailBuilder = new MailBuilder();

            List<string> addresses = new List<string>();
            addresses.Add(email);

            List<string> bcc = new List<string>();
            //bcc = _mailBuilder.getEventureBcc();

            var subject = _mailBuilder.GetSubject(MailType.ResetPassword, ownerId);
            var sender = _mailBuilder.GetSender(ownerId);

            return SendEmail(_mailBuilder.BuildResetPasswordBody(ownerId, resetCode), subject, sender, addresses, bcc);
        }

        public string SendGroupEmailGroup(List<string> emails, string subject, string body, Int32 ownerId)
        {
            var _mailBuilder = new MailBuilder();
            DateTime timeControllerForSendingEmails = DateTime.Now;
            string log = string.Empty;

            foreach (string email in emails)
            {
                bool processed = false;
                while (!processed)
                {
                    if ((DateTime.Now - timeControllerForSendingEmails).TotalSeconds >= .22)
                    {
                        timeControllerForSendingEmails = DateTime.Now;

                        //    //this method gets a list of 60 emails and remove them from the main list
                        //    List<EmailEnt> queuedEmails = GetEmailsQueue(emails, 60));

                        //    SendList(queuedEmails);
                        try
                        {
                            List<string> mailTos = new List<string>();
                            mailTos.Add(email);
                            SendEmail(body, subject, _mailBuilder.GetSender(ownerId), mailTos, new List<string>());
                            //SendEmail(new List<string>().Add(email.ToString()), body, subject, new List<string>(), new List<string>());
                            log = log + "_success: " + DateTime.Now.ToString("o") + " @ " + email  +" || ";
                            
                        }
                        catch (Exception ex)
                        {
                            log = log + "_error: " + DateTime.Now.ToString("o") + " err-> " + ex.Message + " || ";
                        }
                        finally{
                            processed = true;
                        }
                        //timeControllerForSendingEmails = DateTime.Now;
                    }

                }
            }
            return log;
        }

        private Boolean SendEmail(string messageBody, string messageSubject, string sender, List<string> mailTo, List<string> bcc)
        {
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(ConfigurationManager.AppSettings["AWSAccessKey"], ConfigurationManager.AppSettings["AWSKey"], Amazon.RegionEndpoint.USEast1);
            Destination destination = new Destination();
            destination.ToAddresses = mailTo;
            if (bcc.Count > 0)
            {
                destination.BccAddresses = bcc;
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
//    public void SendList(List<EmailEnt> queuedEmails)
//{
//    IList<Task> tasks = new List<Task>();
//    List<string> logLines = new List<string>();

//    foreach (EmailEnt emailEnt in queuedEmails)
//    {
//        string subject = "﻿Hello {name}";
//        string body = "im the body;
//        
//        tasks.Add(Task.Factory.StartNew(() =>
//        {
//            SendEmail(emailEnt, subject, body);
//        }));
//    }


//    Task.WaitAll(tasks.ToArray());
//}