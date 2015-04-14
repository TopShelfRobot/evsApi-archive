using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Reflection;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using evs.DAL;
using evs.Model;
using System.Web;
using evs.Service;
//using Amazon.S3;
//using System.Net.Mail;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;


namespace evs.API.Controllers
{
    [RoutePrefix("api/mail")]
    public class MailController : ApiController
    {
        readonly evsContext db = new evsContext();

        [Route("SendConfirmMail")]
        [HttpPost]
        public HttpResponseMessage SendConfirmMail(int id)
        {
            try
            {
                var _mailService = new MailService();
                var success = _mailService.SendConfirmEmail(id, false);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //var logE = new EventureLog();
                //logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                //logE.Caller = "Mail Api_SendConfirmMail";
                //logE.Status = "ERROR";
                //logE.LogDate = System.DateTime.Now.ToLocalTime();
                //db.EventureLogs.Add(logE);
                //db.SaveChanges();

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
        }

        [Route("SendMassMessage")]
        [HttpPost]
        public HttpResponseMessage SendMassMessage(JObject jsonBundle)
        {
            try
            {
                List<string> emails = new List<string>();
                dynamic emailBundle = jsonBundle;
                if (emailBundle.email != null)  //if no surcharges skip this
                {
                    foreach (dynamic dynEmail in emailBundle.email)
                    {
                        emails.Add(dynEmail.Value);
                    }
                }

                string body = (string)jsonBundle["body"];
                string subject = (string)jsonBundle["subject"];
                Int32 ownerId = (Int32)jsonBundle["ownerId"]; 
 
                var _mailService = new MailService();
                var returnMessage = _mailService.SendGroupEmailGroup(emails, subject, body, ownerId);

                var resp = Request.CreateResponse(HttpStatusCode.OK);
                //resp.Content = new StringContent();
                resp.Content = new StringContent(returnMessage, Encoding.UTF8, "text/plain");
                return resp;


                //if (Request != null)
                //    return Request.CreateResponse(HttpStatusCode.OK);
                //else
                //    return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //var logE = new EventureLog();
                //logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                //logE.Caller = "Mail Api_SendConfirmMail";
                //logE.Status = "ERROR";
                //logE.LogDate = System.DateTime.Now.ToLocalTime();
                //db.EventureLogs.Add(logE);
                //db.SaveChanges();

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
        }
        

        public HttpResponseMessage SendResetPassword(string email, string resetCode)
        {
            var ownerId = 1;   //this will become an argument when going multi-tenant

            var _mailService = new MailService();
            //var _mailBuilder = new MailBuilder();

            var success = _mailService.SendResetPassword(email, resetCode, ownerId);

            if (Request != null)
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("ResendReceipt")]
        [HttpPost]
        public HttpResponseMessage ResendReceipt(JObject jsonBundle)
        {
            try
            {
                int orderId = (Int32)jsonBundle["orderId"];
                var _mailService = new MailService();
                var success = _mailService.SendConfirmEmail(orderId, true);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //var logE = new EventureLog();
                //logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                //logE.Caller = "Mail Api_SendConfirmMail";
                //logE.Status = "ERROR";
                //logE.LogDate = System.DateTime.Now.ToLocalTime();
                //db.EventureLogs.Add(logE);
                //db.SaveChanges();

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }

        }


        [Route("SendTeamPlayerInviteMail/{id}")]
        public HttpResponseMessage SendTeamPlayerInviteMail(int id)
        {
            //needs to be a send amazon email
            //Type : confirm, reset password
            // to, cc, bc, sender, reply, subject, text
            try
            {
                var teamMember = db.TeamMembers.Where(m => m.Id == id).Select(m => new
                {
                    m.TeamMemberGuid,
                    m.Email,
                    m.Name,
                    m.Team.Coach.FirstName,
                    m.Team.Coach.LastName,
                    m.Team.Registration.EventureList.DisplayName,
                    m.Team.TeamGuid,
                    teamName = m.Team.Name,
                    m.Team.Owner.Url,
                    m.Team.Owner.SendImageHtml,
                    m.Team.Owner.SendMailEmailAddress,
                    m.Team.Owner.SendConfirmTeamInviteEmailSubject
                }).SingleOrDefault();

                if (teamMember == null)
                {
                    throw new Exception("Could not find TeamMember from id: " + id.ToString());
                }

                var addresses = new List<string>();
                var mode = ConfigurationManager.AppSettings["MailMode"];
                var emailText = string.Empty;
                var subject = string.Empty;
                var sender = string.Empty;
                var ccs = new List<string>();  //i use cc here because it actaully bccs and that is what i want
                var bcc = new List<string>();
                //this is actually bbc
                //an amazon issue?

                if (mode == "TEST")
                {
                    addresses.Add("boone@firstegg.com");
                    subject = "TEST: Eventure Sports Confirmation";
                    sender = "boone@eventuresports.com";
                    emailText = "<img src=\"http://www.eventuresports.com/Portals/0/Skins/EventureSports_Skin/img/logo.png\"><br><br>";
                }
                else
                {
                    addresses.Add(teamMember.Email);
                    subject = teamMember.SendConfirmTeamInviteEmailSubject;
                    sender = teamMember.SendMailEmailAddress;
                    ccs.Add("boone@firstegg.com");
                    //ccs.Add("podaniel@firstegg.com");
                    ccs.Add(sender);
                    emailText = teamMember.SendImageHtml;
                }

                string url = teamMember.Url + "#/team/" + teamMember.TeamGuid.ToString().ToUpper() + "/member/" +
                             teamMember.TeamMemberGuid.ToString().ToUpper() + "/payment";
                emailText = emailText + "Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                emailText = emailText + "Dear " + teamMember.Name + ",<BR><BR>You have been invited by " + teamMember.FirstName + ' ' + teamMember.LastName + " to join team " + teamMember.teamName + " in " + teamMember.DisplayName + ".";   //TODO: put the back from bc change
                emailText = emailText + " We are excited to have you join the fun!";
                emailText = emailText + "<BR> Please click on the following link: " + url + " to initiate your registration.";

                //var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
                //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);

                var mailTo = new List<string>();
                var ccMail= new List<string>();
                mailTo.Add(teamMember.Email);
                ccMail.Add("boone@firstegg.com");

                //ccs and bcc seem to be reversed
                MailService _mailService = new MailService();
                //var y = _mailService.SendSingleEmail(teamMember.Email, subject, emailText);
                var x = _mailService.SendEmail(emailText, subject, sender, mailTo, ccMail);

                return new HttpResponseMessage(HttpStatusCode.OK);

                //if (mail.ErrorException == null)
                //{
                //    if (Request != null)
                //        return Request.CreateResponse(HttpStatusCode.OK);
                //    else
                //        return new HttpResponseMessage(HttpStatusCode.OK);
                //}
                //else
                //{
                //    var log = new EventureLog();
                //    log.Message = "orderId: " + id + "_email failed" + mail.ErrorException;
                //    log.Caller = "Mail Api_SendConfirmMail";
                //    log.Status = "Error";
                //    log.LogDate = System.DateTime.Now.ToLocalTime();
                //    db.EventureLogs.Add(log);
                //    db.SaveChanges();
                //    if (Request != null)
                //        return Request.CreateResponse(HttpStatusCode.OK); //change this ??  //mjb
                //    else
                //    {
                //        return new HttpResponseMessage(HttpStatusCode.OK);
                //    }
                //}

            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                logE.Caller = "Mail Api_SendConfirmMail";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
                logE.DateCreated = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}



//public HttpResponseMessage SendBourbonLotteryConfirm(Int32 id)
//       {

//           var regs = from o in db.Orders
//                      join r in db.Registrations
//                          on o.Id equals r.EventureOrderId
//                      join h in db.Participants
//                          on o.HouseId equals h.Id
//                      join p in db.Participants
//                         on r.ParticipantId equals p.Id
//                      join e in db.Eventures
//                           on r.EventureList.EventureId equals e.Id
//                      where o.Id == id
//                      select new
//                      {
//                          o.Id,
//                          o.DateCreated,
//                          r.EventureList.DisplayName,
//                          r.EventureGroup.Name,
//                          p.FirstName,
//                          p.LastName,
//                          r.Quantity,
//                          r.ListAmount,
//                          partEmail = p.Email,
//                          e.DisplayHeading,
//                          houseFirst = h.FirstName,
//                          houseLast = h.LastName,
//                          houseEmail = h.Email,
//                          regQuantity = r.Quantity,
//                          e.OwnerId
//                      };

//           //var fees = from s in db.Surcharges
//           //           where s.EventureOrderId == id
//           //           select new { s.Amount, s.Description };

//           Int32 ownerId = 0;
//           string houseName = string.Empty;
//           //string carriageReturn = "<BR>";
//           string orderNum = string.Empty;
//           string houseEmail = string.Empty;
//           string lineItems = "<TABLE cellpadding=\"8\" cellspacingBono=\"8\"><tr><td>Events</td><td>Division</td><td>Participants</td><td>Quantity</td><td>Price</td></tr>";
//           int numReg = 0;
//           decimal orderAmount = 0;

//           foreach (var reg in regs)
//           {
//               houseName = reg.houseFirst + " " + reg.houseLast;
//               orderNum = Convert.ToString(reg.Id);

//               lineItems = lineItems + "<TR><TD>" + reg.DisplayHeading + "</TD><TD>" + reg.Name + "</TD><TD>" +
//                           reg.FirstName + " " + reg.LastName + "</TD><TD Align=\"right\">" + reg.Quantity + "</TD><TD Align=\"right\">" + reg.ListAmount + "</TD></TR>";
//               numReg = numReg + reg.regQuantity;
//               orderAmount = orderAmount + (reg.ListAmount * reg.Quantity);

//               //this is a little ugly but works
//               ownerId = reg.OwnerId;
//               houseEmail = reg.houseEmail;
//           }

//           lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";

//           //foreach (var fee in fees)
//           //{
//           //    lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + fee.Description + "</TD><TD></TD><TD Align=\"right\">" + fee.Amount + "</TD></TR>";
//           //    orderAmount = orderAmount + fee.Amount;
//           //}

//           lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";
//           lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + "Total" + "</TD><TD></TD><TD Align=\"right\">" + orderAmount + "</TD></TR>";
//           lineItems = lineItems + "</TABLE>";

//           //string numOfRegs = Convert.ToString(numReg);

//           var addresses = new List<string>();
//           var mode = ConfigurationManager.AppSettings["MailMode"];
//           var emailText = string.Empty;
//           var subjectText = string.Empty;
//           var sender = string.Empty;
//           var ccs = new List<string>();  //i use cc here because it actaully bccs and that is what i want
//           var bcc = new List<string>();
//           //this is actually bbc
//           //an amazon issue?

//           if (mode == "TEST")
//           {
//               addresses.Add("boone@eventuresports.com");
//               subjectText = "TEST: Eventure Sports Confirmation";
//               sender = "boone@firstegg.com";
//               emailText = "<img src=\"http://www.eventuresports.com/Portals/0/Skins/EventureSports_Skin/img/logo.png\"><br><br>";
//           }
//           else
//           {
//               addresses.Add(houseEmail);
//               var owner = db.Owners.Where(o => o.Id == ownerId).SingleOrDefault();
//               subjectText = owner.SendConfirmEmailSubject;
//               sender = owner.SendMailEmailAddress;
//               bcc.Add("boone@firstegg.com");
//               bcc.Add("podaniel@firstegg.com");
//               bcc.Add(sender);
//               emailText = "<img src=\"https://bourbonchase.eventuresports.com/content/images/logo.png\"><br><br>";
//               //emailText = owner.SendImageHtml;
//           }
//           emailText = emailText + "Order Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
//           emailText = emailText + "Confirmation Number: " + orderNum + "<BR>";
//           //emailText = emailText + "Dear " + houseName + ",<BR><BR>Thank you for purchasing your registration. This email serves as your receipt. Your confirmation number is " + orderNum + ". <BR><BR><BR>You have been charged for the following:";



//           //var addresses = new List<string>();
//           ////var mode = ConfigurationManager.AppSettings["MailMode"];
//           //var emailText = string.Empty;
//           //var subjectText = string.Empty;
//           //var sender = string.Empty;
//           //var ccs = new List<string>();  //i use cc here because it actaully bccs and that is what i want
//           //var bcc = new List<string>();

//           //addresses.Add(email);
//           subjectText = "The Bourbon Chase - 2015 Lottery";
//           sender = "mike@bourbonchase.com";
//           emailText = emailText + "<br><br> Thank you for submitting an application for this fall's Bourbon Chase.  "
//           + "You will be notified of your team's status by February 2, 2015.  <p>We hope to see you on the course in October!</p>";

//           emailText = emailText + "<BR>" + lineItems + "<br><br>Mike Kuntz<br>The Bourbon Chase";

//           //var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
//           var ses = new AmazonSimpleEmailServiceClient("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV", Amazon.RegionEndpoint.USEast1);

//           Destination destination = new Destination();
//           destination.ToAddresses = addresses;
//           //destination.CcAddresses = ccs;
//           destination.BccAddresses = bcc;

//           Body body = new Body() { Html = new Content(emailText) };
//           Content subject = new Content(subjectText);
//           Message message = new Message(subject, body);

//           //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
//           SendEmailRequest sendEmailRequest = new SendEmailRequest(sender, destination, message);
//           ses.SendEmail(sendEmailRequest);
//           //ccs and bcc seem to be reversed

//           //if (mail.ErrorException == null)
//           //if ses.
//           //{
//           if (Request != null)
//               return Request.CreateResponse(HttpStatusCode.OK);
//           else
//               return new HttpResponseMessage(HttpStatusCode.OK);

//       }



//public HttpResponseMessage SendResetPasswordBourbon(string email, string callbackUrl)
//       {
//           var addresses = new List<string>();
//           //var mode = ConfigurationManager.AppSettings["MailMode"];
//           var emailText = string.Empty;
//           var subjectText = string.Empty;
//           var sender = string.Empty;
//           var ccs = new List<string>();  //i use cc here because it actaully bccs and that is what i want
//           var bcc = new List<string>();

//           addresses.Add(email);
//           subjectText = "Bourbon Chase Reset Password";
//           sender = "mike@bourbonchase.com";
//           emailText = "<img src=\"https://bourbonchase.eventuresports.com/content/images/logo.png\"><br><br>" + callbackUrl;

//           //var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
//           var ses = new AmazonSimpleEmailServiceClient("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV", Amazon.RegionEndpoint.USEast1);

//           Destination destination = new Destination();
//           destination.ToAddresses = addresses;
//           //destination.CcAddresses = ccs;
//           //destination.BccAddresses = bcc;

//           Body body = new Body() { Html = new Content(emailText) };
//           Content subject = new Content(subjectText);
//           Message message = new Message(subject, body);

//           //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
//           SendEmailRequest sendEmailRequest = new SendEmailRequest("mike@bourbonchase.com", destination, message);
//           ses.SendEmail(sendEmailRequest);
//           //ccs and bcc seem to be reversed

//           //if (mail.ErrorException == null)
//           //if ses.
//           //{
//           if (Request != null)
//               return Request.CreateResponse(HttpStatusCode.OK);
//           else
//               return new HttpResponseMessage(HttpStatusCode.OK);

//       }