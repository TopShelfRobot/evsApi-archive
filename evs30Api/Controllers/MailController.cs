using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Net.Mail;
using Amazon.SimpleEmail.Model;
using System.Net;
using evs.DAL;
using evs.Model;
using System.Web;

//using evs.Model;

namespace evs30Api.Controllers
{
    public class MailController : ApiController
    {
        evsContext db = new evsContext();

        public HttpResponseMessage SendPasswordReset(string email, string message)
        {
            //needs to be a send amazon email
            //Type : confirm, reset password
            // to, cc, bc, sender, reply, subject, text
            try
            {
                var customName = ConfigurationManager.AppSettings["CustomName"];
                var resetEmail = string.Empty;
                var emailText = string.Empty;

                switch (customName)
                {
                    case "headfirst":
                        resetEmail = "info@headfirstperformance.com";
                        emailText = "<img src=\"https://reg.headfirstperformance.com/Content/images/logo.png\"><br><br>";
                        emailText = emailText + "We received your request to reset your password. Here's what you need to do: <BR><BR>1. Click this link to return to the <a href=\"https://reg.headfirstperformance.com/reg/1\">Headfirst Performance login</a> screen.  <BR>";
                        emailText = emailText + "2. Enter your email address and the new temporary password:  " + message + "  (The temporary password will only work for 3 hours…if you need a new one just click the Forgot Password button again.).  <BR>";
                        emailText = emailText + "3. Once successfully logged in, click on the Settings button at the top right corner of the screen- choose Change Password.  <BR>";
                        emailText = emailText + "4. Choose a new password for your account and you're done!  <BR><BR>";
                        emailText = emailText + " Thank You, <BR>";
                        emailText = emailText + "Headfirst Performance";
                        break;
                    case "titan":
                        resetEmail = "info@mightytitanadventures.com";
                        emailText = "<img src=\"https://reg.mightytitanadventures.com/Content/images/logo.png\"><br><br>";
                        emailText = emailText + "We received your request to reset your password. Here's what you need to do: <BR><BR>1. Click this link to return to the <a href=\"https://reg.mightytitanadventures.com/reg/1\">Mighty Titan login</a> screen.  <BR>";
                        emailText = emailText + "2. Enter your email address and the new temporary password:  " + message + "  (The temporary password will only work for 3 hours…if you need a new one just click the Forgot Password button again.).  <BR>";
                        emailText = emailText + "3. Once successfully logged in, click on the Settings button at the top right corner of the screen- choose Change Password.  <BR>";
                        emailText = emailText + "4. Choose a new password for your account and you're done!  <BR><BR>";
                        emailText = emailText + " Thank You, <BR>";
                        emailText = emailText + "Mighty Titan Adventures";

                        break;

                    case "louisvillesports":
                        resetEmail = "Info@louisvillesports.org";
                        emailText = "<img src=\"https://louisvillesports.eventuresports.com/Content/images/logo.png\"><br><br>";
                        emailText = emailText + "We received your request to reset your password. Here's what you need to do: <BR><BR>1. Click this link to return to the <a href=\"https://louisvillesports.eventuresports.com/reg/1\">Louisville Sports Commission login</a> screen.  <BR>";
                        emailText = emailText + "2. Enter your email address and the new temporary password:  " + message + "  (The temporary password will only work for 3 hours…if you need a new one just click the Forgot Password button again.).  <BR>";
                        emailText = emailText + "3. Once successfully logged in, click on the Settings button at the top right corner of the screen- choose Change Password.  <BR>";
                        emailText = emailText + "4. Choose a new password for your account and you're done!  <BR><BR>";
                        emailText = emailText + " Thank You, <BR>";
                        emailText = emailText + "Louisville Sports Commission";

                        break;

                    case "mockingbird":
                        resetEmail = "kwallace@mockingbirdsoccer.net";
                        emailText = "<img src=\"https://mockingbird.eventuresports.com/Content/images/logo.png\"><br><br>";
                        emailText = emailText + "We received your request to reset your password. Here's what you need to do: <BR><BR>1. Click this link to return to the <a href=\"https://mockingbird.eventuresports.com/reg/1\"Mockingbird login</a> screen.  <BR>";
                        emailText = emailText + "2. Enter your email address and the new temporary password:  " + message + "  (The temporary password will only work for 3 hours…if you need a new one just click the Forgot Password button again.).  <BR>";
                        emailText = emailText + "3. Once successfully logged in, click on the Settings button at the top right corner of the screen- choose Change Password.  <BR>";
                        emailText = emailText + "4. Choose a new password for your account and you're done!  <BR><BR>";
                        emailText = emailText + " Thank You, <BR>";
                        emailText = emailText + "Mockingbird";

                        break;

                    //case "indyrv":
                    //    break;
                    default:
                        resetEmail = "boone@firstegg.com";
                        emailText = "<img src=\"http://www.eventuresports.com/Portals/0/Skins/EventureSports_Skin/img/logo.png\"><br><br>";
                        emailText = emailText + "We received your request to reset your password. Here's what you need to do: <BR><BR>1. Click this link to return to the <a href=\"https://reg.eventuresports.com/reg/1\">Eventures Sports login</a> screen.  <BR>";
                        emailText = emailText + "2. Enter your email address and the new temporary password:  " + message + "  (The temporary password will only work for 3 hours…if you need a new one just click the Forgot Password button again.).  <BR>";
                        emailText = emailText + "3. Once successfully logged in, click on the Settings button at the top right corner of the screen- choose Change Password.  <BR>";
                        emailText = emailText + "4. Choose a new password for your account and you're done!  <BR><BR>";
                        emailText = emailText + " Thank You, <BR>";
                        emailText = emailText + "Eventure Sports";
                        break;
                }


                var addresses = new List<string>();
                addresses.Add(email);
                var ccs = new List<string>();
                //ccs.Add("boone@firstegg.com");
                var bcc = new List<string>();
                //bcc.Add("boone.mike@gmail.com");

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
                //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "info@headfirstperformance.com", "info@headfirstperformance.com", "Password Reset", emailText);
                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, resetEmail, resetEmail, "Password Reset", emailText);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                var logE = new EventureLog
                    {
                        Message = "Error Handler: " + ex.Message,
                        Caller = "Mail Api_SendPasswordReset",
                        Status = "ERROR",
                        LogDate = System.DateTime.Now.ToLocalTime(),
                        DateCreated = System.DateTime.Now.ToLocalTime()
                    };
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

        public HttpResponseMessage SendHeadfirstWelcomeEmail(string email)
        {
            try
            {
                //string emailText = "Your new password is: " + message;
                string emailText = "<img src=\"https://reg.headfirstperformance.com/Content/images/logo.png\"><br><br>";
                emailText = emailText + "Headfirst Performance is excited to announce an even better website and registration process as well as a new online store.   <BR><BR>";
                emailText = emailText + "For security purposes, click the Create New Account button at the bottom of the login to set a new password- the system will remember the rest.  Or even better, use our new Facebook login for instant sign in! <BR><BR>";
                emailText = emailText + "Check out our new & improved website, register for some upcoming races and get your Headfirst gear to look your best!  Click this link  <a href=\"http://headfirstperformance.com/\">to check it out.</a><BR><BR>";
                emailText = emailText + "Let's have a great 2014!  <BR><BR>";
                emailText = emailText + " Thank You, <BR>";
                emailText = emailText + "Headfirst Performance";

                var addresses = new List<string>();
                addresses.Add(email);
                var ccs = new List<string>();
                ccs.Add("boone@eventuresports.com");
                var bcc = new List<string>();
                //bcc.Add("boone.mike@gmail.com");

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
                //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "info@headfirstperformance.com", "info@headfirstperformance.com", "Password Reset", emailText);
                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "todd@headfirstperformance.com", "todd@headfirstperformance.com", "New and Improved Website!", emailText);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message,
                    Caller = "Mail Api_SendWelcomeEmail",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
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

        public HttpResponseMessage SendMockingbirdWelcomeEmail()
        //public HttpResponseMessage SendMockingbirdWelcomeEmail();
        {
            try
            {
                string email = "boone@firstegg.com";

                //string emailText = "Your new password is: " + message;
                string emailText = "<img src=\"https://mockingbird.eventuresports.com/Content/images/logo.png\"><br><br>";
                emailText = emailText + "Mockingbird Valley Sports Complex is excited to offer our members a new and improved registration process:    <BR><BR>";
                emailText = emailText + "NEW Facebook login makes the process even easier and no new passwords to remember! <BR><BR>";
                emailText = emailText + "Still want to use your original Mockingbird user account?  No worries, from the login screen, simply click the bottom Create Account button, enter in your associated email address and a new password and we will synch your login with your account profile.  <BR><BR>";
                //Click this link  <a href=\"http://headfirstperformance.com/\">to check it out.</a><BR><BR>";
                emailText = emailText + "We still have a few open spots for Spring Soccer- Register Today!  <BR><BR>";
                emailText = emailText + " Thank You, <BR>";
                emailText = emailText + "Mockingbird Valley Sports Complex";

                var addresses = new List<string>();
                addresses.Add(email);
                var ccs = new List<string>();
                ccs.Add("boone@eventuresports.com");
                var bcc = new List<string>();
                //bcc.Add("boone.mike@gmail.com");

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
                //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "info@headfirstperformance.com", "info@headfirstperformance.com", "Password Reset", emailText);
                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "kwallace@mockingbirdsoccer.net", "kwallace@mockingbirdsoccer.net", "New and Improved Registration!", emailText);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message,
                    Caller = "Mail Api_SendWelcomeEmail",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
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

        public HttpResponseMessage SendEmailStock(string email, string code)
        {
            try
            {

                var emailText = string.Empty;
                //var email = string.Empty;
                var subject = string.Empty;

                switch (code)
                {
                    case "QuickMail":

                        emailText = "<img src=\"https://reg.headfirstperformance.com/Content/images/logo.png\"><br><br>";

                        emailText = emailText + "On Saturday, January 18th at 7:45am, there will be a training run that will meet at the Horine Center and will run the first 15 miles of the course.  If you have any questions, please call (502) 220-3440.  Hope to see you there!   <BR><BR>";

                        //email = "boone.mike@gmail.com";
                        subject = "Louisville Lovin the Hills Training Run!";

                        break;
                    case "ResetPassword":
                        Console.WriteLine("Case 2");
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }



                var addresses = new List<string>();
                addresses.Add(email);
                var ccs = new List<string>();
                ccs.Add("boone@eventuresports.com");
                var bcc = new List<string>();
                //bcc.Add("boone.mike@gmail.com");

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
                //AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "info@headfirstperformance.com", "info@headfirstperformance.com", "Password Reset", emailText);
                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "info@headfirstperformance.com", "info@headfirstperformance.com", subject, emailText);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message,
                    Caller = "Mail Api_SendWelcomeEmail",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
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


        public HttpResponseMessage SendInfoMessage(string email, string message)
        {
            //needs to be a send amazon email
            //Type : confirm, reset password
            // to, cc, bc, sender, reply, subject, text
            try
            {
                string emailText = "Error: " + message;

                var addresses = new List<string>();
                addresses.Add(email);
                var ccs = new List<string>();
                ccs.Add("boone@eventuresports.com");
                var bcc = new List<string>();
                //bcc.Add("boone.mike@gmail.com");

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");
                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, "boone@eventuresports.com", "boone@eventuresports.com", "Error", emailText);

                if (Request != null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message,
                    Caller = "Mail Api_SendInfoMessage",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime(),
                    DateCreated = System.DateTime.Now.ToLocalTime()
                };
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

        public HttpResponseMessage SendConfirmMail(int id)
        {
            //needs to be a send amazon email
            //Type : confirm, reset password
            // to, cc, bc, sender, reply, subject, text

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
                           where o.Id == id
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
                           where s.EventureOrderId == id
                           select new { s.Amount, s.Description };

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

                    //this is a little ugly but works
                    ownerId = reg.OwnerId;
                    houseEmail = reg.houseEmail;
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

                //string numOfRegs = Convert.ToString(numReg);

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

                    addresses.Add(houseEmail);

                    var owner = db.Owners.Where(o => o.Id == ownerId).SingleOrDefault();
                    subject = owner.SendConfirmEmailSubject;
                    sender = owner.SendMailEmailAddress;
                    ccs.Add("boone@eventuresports.com");
                    ccs.Add("podaniel@firstegg.com");
                    ccs.Add(sender);
                    //emailText = "<img src=\"https://reg.headfirstperformance.com/Content/images/logo.png\"><br><br>";
                    emailText = owner.SendImageHtml;
                }
                emailText = emailText + "Order Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                emailText = emailText + "Dear " + houseName + ",<BR><BR>Thank you for purchasing your registration. This email serves as your receipt. Your confirmation number is " + orderNum + ". <BR><BR><BR>You have been charged for the following:";
                emailText = emailText + "<BR>" + lineItems;

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");

                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
                //ccs and bcc seem to be reversed

                if (mail.ErrorException == null)
                {
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    var log = new EventureLog();
                    log.Message = "orderId: " + id + "_email failed" + mail.ErrorException;
                    log.Caller = "Mail Api_SendConfirmMail";
                    log.Status = "Error";
                    log.LogDate = System.DateTime.Now.ToLocalTime();
                    log.DateCreated = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(log);
                    db.SaveChanges();
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK); //change this ??  //mjb
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }

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

        [HttpPost]
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage SendTeamPaymentConfirmMail(int id)
        {
            //needs to be a send amazon email
            //Type : confirm, reset password
            // to, cc, bc, sender, reply, subject, text

            try
            {

                var payment = db.TeamMemberPayments.Where(p => p.Id == id).Select(p => new
                {
                    p.Id,
                    p.Amount,
                    p.TeamMember.Name,
                    p.TeamMember.Email,
                    teamName = p.TeamMember.Team.Name
                }).SingleOrDefault();


                Int32 ownerId = 1;
                string houseName = string.Empty;
                //string carriageReturn = "<BR>";
                string orderNum = string.Empty;
                string houseEmail = string.Empty;
                string lineItems = "<TABLE cellpadding=\"8\" cellspacingBono=\"8\"><tr><td>Name</td><td>Email</td><td>Team</td><td>Quantity</td><td>Price</td></tr>";
                int numReg = 0;
                decimal orderAmount = 0;

                //foreach (var reg in regs)
                //{
                houseName = payment.Name;
                orderNum = Convert.ToString(payment.Id);
                lineItems = lineItems + "<TR><TD>" + payment.Name + "</TD><TD>" + payment.Email + "</TD><TD>" +
                            payment.teamName + "</TD><TD Align=\"right\">" + "1" + "</TD><TD Align=\"right\">" + payment.Amount + "</TD></TR>";
                //numReg = numReg + reg.regQuantity;
                orderAmount = payment.Amount;
                //this is a little ugly but works
                //ownerId = reg.OwnerId;
                houseEmail = payment.Email;
                //}

                lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";

                //foreach (var fee in fees)
                //{
                //    lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + fee.Description + "</TD><TD></TD><TD Align=\"right\">" + fee.Amount + "</TD></TR>";
                //    orderAmount = orderAmount + fee.Amount;
                //}

                lineItems = lineItems + "<TR><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD></TR>";
                lineItems = lineItems + "<TR><TD></TD><TD></TD><TD Align=\"right\">" + "Total" + "</TD><TD></TD><TD Align=\"right\">" + orderAmount + "</TD></TR>";

                lineItems = lineItems + "</TABLE>";

                //string numOfRegs = Convert.ToString(numReg);

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

                    addresses.Add(houseEmail);

                    var owner = db.Owners.SingleOrDefault(o => o.Id == ownerId);
                    subject = owner.SendConfirmTeamEmailSubject;
                    sender = owner.SendMailEmailAddress;
                    ccs.Add("boone@eventuresports.com");
                    ccs.Add("podaniel@firstegg.com");
                    ccs.Add(sender);
                    emailText = owner.SendImageHtml;
                }
                emailText = emailText + "Order Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                emailText = emailText + "Dear " + houseName + ",<BR><BR>Thank you for purchasing your registration. This email serves as your receipt. Your confirmation number is " + orderNum + ". <BR><BR><BR>You have been charged for the following:";
                emailText = emailText + "<BR>" + lineItems;

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");

                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
                //ccs and bcc seem to be reversed

                if (mail.ErrorException == null)
                {
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    var log = new EventureLog();
                    log.Message = "orderId: " + id + "_email failed" + mail.ErrorException;
                    log.Caller = "Mail Api_SendConfirmMail";
                    log.Status = "Error";
                    log.LogDate = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(log);
                    db.SaveChanges();
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK); //change this ??  //mjb
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                logE.Caller = "Mail Api_SendConfirmMail";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
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
                    ccs.Add("boone@eventuresports.com");
                    //ccs.Add("podaniel@firstegg.com");
                    ccs.Add(sender);
                    emailText = teamMember.SendImageHtml;
                }

                string url = teamMember.Url + "/reg.html#/team/" + teamMember.TeamGuid.ToString().ToUpper() + "/member/" +
                             teamMember.TeamMemberGuid.ToString().ToUpper() + "/payment";
                emailText = emailText + "Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                emailText = emailText + "Dear " + teamMember.Name + ",<BR><BR>You have been invited by " + teamMember.FirstName + ' ' + teamMember.LastName + " to join team " + teamMember.teamName + " in the " + teamMember.DisplayName;
                emailText = emailText + ". <BR> Please click on the following link: " + url;

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");

                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
                //ccs and bcc seem to be reversed

                if (mail.ErrorException == null)
                {
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    var log = new EventureLog();
                    log.Message = "orderId: " + id + "_email failed" + mail.ErrorException;
                    log.Caller = "Mail Api_SendConfirmMail";
                    log.Status = "Error";
                    log.LogDate = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(log);
                    db.SaveChanges();
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK); //change this ??  //mjb
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                logE.Caller = "Mail Api_SendConfirmMail";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
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

        public HttpResponseMessage SendSoccerTryoutInviteMail(int id)
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
                    ccs.Add("boone@eventuresports.com");
                    //ccs.Add("podaniel@firstegg.com");
                    //ccs.Add(sender);
                    emailText = teamMember.SendImageHtml;
                }

                string url = teamMember.Url + "/reg.html#/team/" + teamMember.TeamGuid.ToString().ToUpper() + "/member/" +
                             teamMember.TeamMemberGuid.ToString().ToUpper() + "/payment";
                emailText = emailText + "<table width=\"100%\"><tr><td><tablewidth=\"600\" align=\"center\" style=\"font-family:Helvetica;\">";

                emailText = emailText + "Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                //emailText = emailText + "Dear " + teamMember.Name;
                emailText = emailText + " <tr><td><h3>Dear " + teamMember.Name + ",</h3></td></tr>";
                emailText = emailText +
                            " <tr><td style=\"line-height:140%;\"><p>We are pleased to invite you to attend the Louisville City FC Invitational Tryouts scheduled for November 22 and 23 at Collegiate Fields.  In order to secure your place at the upcoming tryouts you will need to submit payment and provide your travel itinerary prior to October 31st.  If you have not registered and paid in full by October 31st your invitation at the tryout is not guaranteed.</p><p>Cost<br>$160 for local participants NOT requiring a hotel accommodations<br>$310 for commuter participants requiring a hotel accommodations for two nights (Nov. 21 & 22)</p><p>Hotel<br>Holiday Inn Louisville Airport<br>447 Farmington Avenue<br>Louisville, KY  40209<br><a href=\"http://www.ihg.com/holidayinn/hotels/us/en/louisville/sdfcd/hoteldetail\">Hotel Details</a><br><strong>NOTE:</strong>&nbsp;&nbsp;&nbsp;Please do not call the hotel.  Louisville City FC is handling your hotel arrangements directly<br>The Holiday Inn provides a free shuttle to and from the airport<br>Transportation from the hotel to Collegiate Field will be provided<br></p><p>Additional Information<ul><li>Two (2) tryout t-shirts will be provided</li><li>Collegiate fields are a grass surface.  Please bring appropriate footwear</li><li>If participant is awarded a playing contract, tryout fee will be reimbursed</li><li>If tryout is cancelled due to unforeseen inclement weather, $75 will be reimbursed for local participants, and $150 will be reimbursed for commuters</li></ul></p><p>Congratulations on your selection and we look forward to seeing you at tryouts on November 22 and 23.</p><p>Should you have any questions please contact Amanda Duffy at <a href=\"mailto:aduffy@louisvillecityfc.com\">aduffy@louisvillecityfc.com</a>.</p></td></tr></table></td></tr></table>";
                emailText = emailText + "<BR> Please click on the following link: " + url;

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");

                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
                //ccs and bcc seem to be reversed

                if (mail.ErrorException == null)
                {
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    var log = new EventureLog();
                    log.Message = "orderId: " + id + "_email failed" + mail.ErrorException;
                    log.Caller = "Mail Api_SendConfirmMail";
                    log.Status = "Error";
                    log.LogDate = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(log);
                    db.SaveChanges();
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK); //change this ??  //mjb
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "orderId: " + id + "_Error Handler: " + ex.Message;
                logE.Caller = "Mail Api_SendConfirmMail";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
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

        public HttpResponseMessage ReendConfirmEmail(int orderId, string emailAddress)
        {
            //needs to be a send amazon email
            //Type : confirm, reset password
            // to, cc, bc, sender, reply, subject, text

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
                               p.Email,
                               e.DisplayHeading,
                               houseFirst = h.FirstName,
                               houseLast = h.LastName,
                               regQuantity = r.Quantity,
                               e.OwnerId
                           };

                var fees = from s in db.Surcharges
                           where s.EventureOrderId == orderId
                           select new { s.Amount, s.Description };

                Int32 ownerId = 0;
                string houseName = string.Empty;
                //string carriageReturn = "<BR>";
                string orderNum = string.Empty;
                string partEmail = string.Empty;
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

                    //this is a little ugly but works
                    ownerId = reg.OwnerId;
                    partEmail = reg.Email;
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

                //string numOfRegs = Convert.ToString(numReg);

                var addresses = new List<string>();
                addresses.Add(partEmail);

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
                    subject = "TEST: Headfirst Performance Registration Confirmation";
                    sender = "boone@eventuresports.com";
                    emailText = "<img src=\"https://reg.headfirstperformance.com/Content/images/logo.png\"><br><br>";
                }
                else
                {
                    var owner = db.Owners.Where(o => o.Id == ownerId).SingleOrDefault();
                    subject = owner.SendConfirmEmailSubject;
                    sender = owner.SendMailEmailAddress;
                    addresses.Add(owner.SendMailEmailAddress);
                    emailText = owner.SendImageHtml;
                }
                emailText = emailText + "Order Date: " + DateTime.Now.ToString("M/d/yyyy") + "<BR>";
                emailText = emailText + "Dear " + houseName + ",<BR><BR>Thank you for purchasing your registration. This email serves as your receipt. Your confirmation number is " + orderNum + ". <BR><BR><BR>You have been charged for the following:";
                emailText = emailText + "<BR>" + lineItems;

                var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");

                AmazonSentEmailResult mail = ses.SendEmail(addresses, ccs, bcc, sender, sender, subject, emailText);
                //ccs and bcc seem to be reversed

                if (mail.ErrorException == null)
                {
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    var log = new EventureLog();
                    log.Message = "orderId: " + orderId + "_email failed" + mail.ErrorException;
                    log.Caller = "Mail Api";
                    log.Status = "Error";
                    log.LogDate = System.DateTime.Now.ToLocalTime();
                    db.EventureLogs.Add(log);
                    db.SaveChanges();
                    if (Request != null)
                        return Request.CreateResponse(HttpStatusCode.OK); //change this ??  //mjb
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                var logE = new EventureLog();
                logE.Message = "orderId: " + orderId + "_Error Handler: " + ex.Message;
                logE.Caller = "Mail Api";
                logE.Status = "ERROR";
                logE.LogDate = System.DateTime.Now.ToLocalTime();
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

        public HttpResponseMessage MailTest(JObject saveBundle)
        {
            List<string> addresses = new List<string>();
            addresses.Add("boone.mike@gmail.com");
            addresses.Add("kmacdonald@firstegg.com");
            addresses.Add("podaniel@firstegg.com");

            var ses = new AmazonSESWrapper("AKIAIACOACRTWREUKHWA", "eXlslxG5YX2+SKAvBbSuMqeJouwGEDci3cfa7TaV");

            //SendEmail(string awsAccessKey, string awsSecretKey, List<string> to, List<string> cc, List<string> bcc, string senderEmailAddress, string replyToEmailAddress, string subject, string body)
            AmazonSentEmailResult mail = ses.SendEmail(addresses, addresses, addresses, "kmacdonald@firstegg.com", "kmacdonald@firstegg.com", "multi-mail blaster", "this is a test of the ebs");

            //AmazonSESWrapper.SendEmail("", "", addresses, addresses, addresses, "kmacdonald@firstegg.com",
            //                                   "kmacdonald@firstegg.com", "subj", "body");

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Holds the sended email info.
        /// </summary>
        public class AmazonSentEmailResult
        {
            public Exception ErrorException { get; set; }
            public string MessageId { get; set; }
            public bool HasError { get; set; }

            public AmazonSentEmailResult()
            {
                this.HasError = false;
                this.ErrorException = null;
                this.MessageId = string.Empty;
            }
        }

        /// <summary>
        /// Send Quota Response 
        /// </summary>
        public class AmazonSendQuotaResponse
        {
            public double Max24HourSend { get; set; }
            public double MaxSendRate { get; set; }
            public double SentLast24Hours { get; set; }

            public AmazonSendQuotaResponse()
            {
            }
        }

        public class AmazonSESWrapper
        {

            public AmazonSESWrapper(string accessKey, string secretKey)
            {
                this.AWSAccessKey = accessKey;
                this.AWSSecretKey = secretKey;
            }

            /// <summary>
            /// Amazon Access key
            /// </summary>
            public string AWSAccessKey { get; set; }

            /// <summary>
            /// Amazon Secret key
            /// </summary>
            public string AWSSecretKey { get; set; }

            /// <summary>
            /// Send email to list of email collections.
            /// </summary>
            /// <param name="to">List of strings TO address collection</param>
            /// <param name="cc">List of strings CCC address collection</param>
            /// <param name="bcc">List of strings BCC address collection</param>
            /// <param name="senderEmailAddress">Sender email. Must be verified before sending.</param>
            /// <param name="replyToEmailAddress">Reply to email.</param>
            /// <param name="subject">Mail Subject</param>
            /// <param name="body">Mail Body</param>
            /// <returns></returns>
            public AmazonSentEmailResult SendEmail(List<string> to, List<string> cc, List<string> bcc, string senderEmailAddress, string replyToEmailAddress, string subject, string body)
            {
                return SendEmail(this.AWSAccessKey, this.AWSSecretKey, to, cc, bcc, senderEmailAddress, replyToEmailAddress, subject, body);
            }

            /// <summary>
            /// Simple Send email 
            /// </summary>
            /// <param name="to">List of strings TO address collection</param>
            /// <param name="senderEmailAddress">Sender email. Must be verified before sending.</param>
            /// <param name="replyToEmailAddress">Reply to email.</param>
            /// <param name="subject">Mail Subject</param>
            /// <param name="body">Mail Body</param>
            /// <returns></returns>
            public AmazonSentEmailResult SendEmail(string toEmail, string senderEmailAddress, string replyToEmailAddress, string subject, string body)
            {
                List<string> toAddressList = new List<string>();
                toAddressList.Add(toEmail);
                return SendEmail(this.AWSAccessKey, this.AWSSecretKey, toAddressList, new List<string>(), new List<string>(), senderEmailAddress, replyToEmailAddress, subject, body);
            }

            /// <summary>
            /// Send Email Via Amazon SES
            /// </summary>
            /// <param name="awsAccessKey"></param>
            /// <param name="awsSecretKey"></param>
            /// <param name="to">List of strings TO address collection</param>
            /// <param name="cc">List of strings CCC address collection</param>
            /// <param name="bcc">List of strings BCC address collection</param>
            /// <param name="senderEmailAddress">Sender email. Must be verified before sending.</param>
            /// <param name="replyToEmailAddress">Reply to email.</param>
            /// <param name="subject">Mail Subject</param>
            /// <param name="body">Mail Body</param>
            /// <returns></returns>
            public AmazonSentEmailResult SendEmail(string awsAccessKey, string awsSecretKey, List<string> to, List<string> cc, List<string> bcc, string senderEmailAddress, string replyToEmailAddress, string subject, string body)
            {
                AmazonSentEmailResult result = new AmazonSentEmailResult();

                try
                {
                    List<string> listColTo = new List<string>();
                    listColTo.AddRange(to);

                    List<string> listColCc = new List<string>();
                    listColCc.AddRange(cc);

                    List<string> listColBcc = new List<string>();
                    listColBcc.AddRange(bcc);

                    Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient(awsAccessKey, awsSecretKey);
                    Amazon.SimpleEmail.Model.SendEmailRequest mailObj = new Amazon.SimpleEmail.Model.SendEmailRequest();
                    Amazon.SimpleEmail.Model.Destination destinationObj = new Amazon.SimpleEmail.Model.Destination();

                    //Add addreses
                    destinationObj.ToAddresses.AddRange(listColTo);
                    destinationObj.BccAddresses.AddRange(listColCc);
                    destinationObj.CcAddresses.AddRange(listColBcc);

                    //Add address info
                    mailObj.Destination = destinationObj;
                    mailObj.Source = senderEmailAddress;
                    mailObj.ReturnPath = replyToEmailAddress;

                    ////Create Message
                    Amazon.SimpleEmail.Model.Content emailSubjectObj = new Amazon.SimpleEmail.Model.Content(subject);
                    Amazon.SimpleEmail.Model.Content emailBodyContentObj = new Amazon.SimpleEmail.Model.Content(body);

                    //Create email body object
                    Amazon.SimpleEmail.Model.Body emailBodyObj = new Amazon.SimpleEmail.Model.Body();
                    emailBodyObj.Html = emailBodyContentObj;
                    emailBodyObj.Text = emailBodyContentObj;

                    //Create message
                    Amazon.SimpleEmail.Model.Message emailMessageObj = new Amazon.SimpleEmail.Model.Message(emailSubjectObj, emailBodyObj);
                    mailObj.Message = emailMessageObj;

                    //Send Message
                    Amazon.SimpleEmail.Model.SendEmailResponse response = client.SendEmail(mailObj);
                    result.MessageId = response.SendEmailResult.MessageId;
                }
                catch (Exception ex)
                {
                    //If any error occurs, HasError flag will be set to true.
                    result.HasError = true;
                    result.ErrorException = ex;
                }

                return result;
            }

            /// <summary>
            /// Create Amazon SDK Client
            /// </summary>
            /// <returns></returns>
            public Amazon.SimpleEmail.AmazonSimpleEmailServiceClient CreateAmazonSDKClient()
            {
                return CreateAmazonSDKClient(AWSAccessKey, AWSSecretKey);
            }

            /// <summary>
            /// Create Amazon SDK Client
            /// </summary>
            /// <param name="awsAccessKey">Amazon Access Key</param>
            /// <param name="awsSecretKey">Amazon Secret Key</param>
            /// <returns></returns>
            public Amazon.SimpleEmail.AmazonSimpleEmailServiceClient CreateAmazonSDKClient(string awsAccessKey, string awsSecretKey)
            {
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = null;

                //mjb i modified this;  it wan't creating client because vals were populated ??
                //if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(AWSSecretKey))
                //{
                client = new Amazon.SimpleEmail.AmazonSimpleEmailServiceClient(awsAccessKey, awsSecretKey);
                //}

                return client;
            }


            /// <summary>
            /// Send a verification email to specified email. Amazon SES needs to a verified email in order to use it as a sender email.
            /// When this function calls, a verification email will be sent to specified email. You need to click the verification link on the upcoming email.
            /// </summary>
            /// <param name="email"></param>
            /// <returns></returns>
            public bool VerifyEmailAddress(string email)
            {
                bool result = false;

                Amazon.SimpleEmail.Model.VerifyEmailAddressRequest request = new Amazon.SimpleEmail.Model.VerifyEmailAddressRequest();
                Amazon.SimpleEmail.Model.VerifyEmailAddressResponse response = new Amazon.SimpleEmail.Model.VerifyEmailAddressResponse();
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient();

                if (client != null)
                {

                    request.EmailAddress = email.Trim();
                    response = client.VerifyEmailAddress(request);

                    if (!string.IsNullOrEmpty(response.ResponseMetadata.RequestId))
                    {
                        result = true;
                    }
                }

                return result;
            }

            /// <summary>
            /// Delete sender email from verified email list.
            /// </summary>
            /// <param name="email"></param>
            /// <returns></returns>
            public bool DeleteEmailAddress(string email)
            {
                bool result = false;

                Amazon.SimpleEmail.Model.DeleteVerifiedEmailAddressRequest request = new Amazon.SimpleEmail.Model.DeleteVerifiedEmailAddressRequest();
                Amazon.SimpleEmail.Model.DeleteVerifiedEmailAddressResponse response = new Amazon.SimpleEmail.Model.DeleteVerifiedEmailAddressResponse();
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient();

                if (client != null)
                {
                    request.EmailAddress = email.Trim();
                    response = client.DeleteVerifiedEmailAddress(request);

                    if (!string.IsNullOrEmpty(response.ResponseMetadata.RequestId))
                    {
                        result = true;
                    }
                }

                return result;
            }

            /// <summary>
            /// Get Send Qouta information from Amazon
            /// </summary>
            /// <returns></returns>
            public AmazonSendQuotaResponse GetSendQuotaInformation()
            {
                AmazonSendQuotaResponse quotaResponse = new AmazonSendQuotaResponse();
                Amazon.SimpleEmail.Model.GetSendQuotaRequest request = new Amazon.SimpleEmail.Model.GetSendQuotaRequest();
                Amazon.SimpleEmail.Model.GetSendQuotaResponse response = new Amazon.SimpleEmail.Model.GetSendQuotaResponse();
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient();


                if (client != null)
                {
                    response = client.GetSendQuota(request);

                    if (!string.IsNullOrEmpty(response.ResponseMetadata.RequestId))
                    {
                        if (response.GetSendQuotaResult != null)
                        {
                            quotaResponse.Max24HourSend = response.GetSendQuotaResult.Max24HourSend;

                            quotaResponse.MaxSendRate = response.GetSendQuotaResult.MaxSendRate;

                            quotaResponse.SentLast24Hours = response.GetSendQuotaResult.SentLast24Hours;
                        }
                    }
                }

                return quotaResponse;
            }


            /// <summary>
            /// Get Send Statistics information from Amazon
            /// </summary>
            /// <returns></returns>
            public List<Amazon.SimpleEmail.Model.SendDataPoint> GetSendStatisticInformation()
            {
                List<Amazon.SimpleEmail.Model.SendDataPoint> resultSendDataPointList = new List<Amazon.SimpleEmail.Model.SendDataPoint>();
                AmazonSendQuotaResponse quotaResponse = new AmazonSendQuotaResponse();
                Amazon.SimpleEmail.Model.GetSendStatisticsRequest request = new Amazon.SimpleEmail.Model.GetSendStatisticsRequest();
                Amazon.SimpleEmail.Model.GetSendStatisticsResponse response = new Amazon.SimpleEmail.Model.GetSendStatisticsResponse();
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient();

                if (client != null)
                {
                    response = client.GetSendStatistics(request);

                    if (!string.IsNullOrEmpty(response.ResponseMetadata.RequestId))
                    {
                        if (response.GetSendStatisticsResult != null)
                        {
                            if (response.GetSendStatisticsResult.SendDataPoints != null)
                            {
                                resultSendDataPointList = response.GetSendStatisticsResult.SendDataPoints;
                            }
                        }
                    }
                }

                return resultSendDataPointList;
            }


            /// <summary>
            /// Lists the verified sender emails 
            /// </summary>
            /// <returns></returns>
            public List<string> ListVerifiedEmailAddresses()
            {
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient();
                Amazon.SimpleEmail.Model.ListVerifiedEmailAddressesRequest request = new Amazon.SimpleEmail.Model.ListVerifiedEmailAddressesRequest();
                Amazon.SimpleEmail.Model.ListVerifiedEmailAddressesResponse response = new Amazon.SimpleEmail.Model.ListVerifiedEmailAddressesResponse();

                List<string> verifiedEmailList = new List<string>();
                response = client.ListVerifiedEmailAddresses(request);
                if (client != null)
                {
                    if (response.ListVerifiedEmailAddressesResult != null)
                    {
                        if (response.ListVerifiedEmailAddressesResult.VerifiedEmailAddresses != null)
                        {
                            verifiedEmailList.AddRange(response.ListVerifiedEmailAddressesResult.VerifiedEmailAddresses);
                        }
                    }
                }

                return verifiedEmailList;
            }


            /// <summary>
            /// Send raw email
            /// </summary>
            /// <param name="toEmail"></param>
            /// <param name="senderEmailAddress"></param>
            /// <param name="replyToEmailAddress"></param>
            /// <param name="subject"></param>
            /// <param name="body"></param>
            /// <param name="text"></param>
            /// <returns></returns>
            public AmazonSentEmailResult SendRawEmail(string toEmail, string senderEmailAddress, string replyToEmailAddress, string subject, string body, string text)
            {
                List<string> toAddressList = new List<string>();
                toAddressList.Add(toEmail);
                return SendRawEmail(this.AWSAccessKey, this.AWSSecretKey, toAddressList, senderEmailAddress, replyToEmailAddress, subject, body, text);
            }

            /// <summary>
            /// Send raw email
            /// </summary>
            /// <param name="awsAccessKey"></param>
            /// <param name="awsSecretKey"></param>
            /// <param name="toEmail"></param>
            /// <param name="senderEmailAddress"></param>
            /// <param name="replyToEmailAddress"></param>
            /// <param name="subject"></param>
            /// <param name="body"></param>
            /// <param name="text"></param>
            /// <returns></returns>
            public AmazonSentEmailResult SendRawEmail(string awsAccessKey, string awsSecretKey, string toEmail, string senderEmailAddress, string replyToEmailAddress, string subject, string body, string text)
            {
                List<string> toAddressList = new List<string>();
                toAddressList.Add(toEmail);
                return SendRawEmail(awsAccessKey, awsSecretKey, toAddressList, senderEmailAddress, replyToEmailAddress, subject, body, text);
            }

            /// <summary>
            /// Send Raw Email. All the fields are populated via parameters. MailMessage object will be converted to MemeoryStream and use SendRawEmail function in Amazon C# SDK.
            /// </summary>
            /// <param name="awsAccessKey"></param>
            /// <param name="awsSecretKey"></param>
            /// <param name="to"></param>
            /// <param name="cc"></param>
            /// <param name="bcc"></param>
            /// <param name="senderEmailAddress"></param>
            /// <param name="replyToEmailAddress"></param>
            /// <param name="subject"></param>
            /// <param name="body"></param>
            /// <param name="text"></param>
            /// <returns></returns>
            public AmazonSentEmailResult SendRawEmail(string awsAccessKey, string awsSecretKey, List<string> to, string senderEmailAddress, string replyToEmailAddress, string subject, string body, string text)
            {
                //bool UseDKIMSignature = false;

                AmazonSentEmailResult result = new AmazonSentEmailResult();
                Amazon.SimpleEmail.AmazonSimpleEmailServiceClient client = CreateAmazonSDKClient();

                AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(senderEmailAddress);
                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;


                List<string> listColTo = new List<string>();
                listColTo.AddRange(to);

                foreach (String toAddress in listColTo)
                {
                    mailMessage.To.Add(new MailAddress(toAddress));
                }

                if (replyToEmailAddress != null)
                {
                    mailMessage.ReplyTo = new MailAddress(replyToEmailAddress);
                }

                if (text != null)
                {
                    mailMessage.AlternateViews.Add(plainView);
                }

                if (body != null)
                {
                    mailMessage.AlternateViews.Add(htmlView);
                }

                //Will be implemented
                //if (UseDKIMSignature)
                //{
                //    SignEmail(mailMessage);
                //}

                RawMessage rawMessage = new RawMessage();

                using (MemoryStream memoryStream = ConvertMailMessageToMemoryStream(mailMessage))
                {
                    rawMessage.WithData(memoryStream);
                }

                SendRawEmailRequest request = new SendRawEmailRequest();
                request.RawMessage = rawMessage;
                request.Destinations = listColTo;
                request.Source = senderEmailAddress;

                try
                {
                    SendRawEmailResponse response = client.SendRawEmail(request);
                    result.MessageId = response.SendRawEmailResult.MessageId;
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorException = ex;
                }

                return result;
            }




            /// <summary>
            /// Helper function for converting .Net MailMessage object to stream. Used when sending RawEmail.
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public MemoryStream ConvertMailMessageToMemoryStream(MailMessage message)
            {
                Assembly assembly = typeof(SmtpClient).Assembly;

                Type mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                MemoryStream fileStream = new MemoryStream();

                ConstructorInfo mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                object mailWriter = mailWriterContructor.Invoke(new object[] { fileStream });

                MethodInfo sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true }, null);

                MethodInfo closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

                return fileStream;
            }


            //public class BreezeSimpleCorsHandler : DelegatingHandler
            //{
            //    const string Origin = "Origin";
            //    const string AccessControlRequestMethod = "Access-Control-Request-Method";
            //    const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
            //    const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
            //    const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
            //    const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
            //    const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";

            //    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            //    {
            //        var isCorsRequest = request.Headers.Contains(Origin);
            //        var isPreflightRequest = request.Method == HttpMethod.Options;
            //        if (isCorsRequest)
            //        {
            //            if (isPreflightRequest)
            //            {
            //                var response = new HttpResponseMessage(HttpStatusCode.OK);
            //                response.Headers.Add(AccessControlAllowOrigin,
            //                  request.Headers.GetValues(Origin).First());

            //                var accessControlRequestMethod =
            //                  request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();

            //                if (accessControlRequestMethod != null)
            //                {
            //                    response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
            //                }

            //                var requestedHeaders = string.Join(", ",
            //                   request.Headers.GetValues(AccessControlRequestHeaders));

            //                if (!string.IsNullOrEmpty(requestedHeaders))
            //                {
            //                    response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
            //                }

            //                response.Headers.Add(AccessControlAllowCredentials, "true");

            //                var tcs = new TaskCompletionSource<HttpResponseMessage>();
            //                tcs.SetResult(response);
            //                return tcs.Task;
            //            }
            //            return base.SendAsync(request, cancellationToken).ContinueWith(t =>
            //            {
            //                var resp = t.Result;
            //                resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
            //                resp.Headers.Add(AccessControlAllowCredentials, "true");
            //                return resp;
            //            });
            //        }
            //        return base.SendAsync(request, cancellationToken);
            //    }
            //}

            /*
               private static void SignEmail(MailMessage message)
               {
                   string base64privatekey = @"-----BEGIN RSA PRIVATE KEY-----
       MIICXgIBAAKBgQDdA6uU906CZrUCpf55CEGY+GX++tw+H9iLenDjEyB8lf0NAIE9
       /M0gQ5pT0U6sbfhpAmcATOy2UP12w1+09QUR3MZ4qKzYmAHd0FfthwYr0GijAUqc
       Zvvp5masZXbA8NayQmYvLl2dwfi0UcP0omqlvrhQ+014aGJKQX6lKjgubQIDAQAB
       AoGBAIB4VLGHu9Qi/Y7njG8wNGlF4ov/qCxYeJhC+QGVqamhyfFX3Mh6fYYGpduY
       7CFg3yezJMpQ7LvhgiQZ1zUpw+KUI9pmq6wqpYtAswA07A6aux82iBvpwy8pPMOP
       5BDba6mlErpngC/EAubYyV8HFjPT2RWVXbksA1kNexy5gtvBAkEA/Oi5Qk9m+R+6
       irPlVaBEyAt8AVyvlJOXuMW4CzgNzlCr4X/uGDC6+qE7COwirXdBOlNYuOMy04iD
       A7kioJ0BPQJBAN+3KFRtwSS6dpzDrxzThCmAkvWji2+YaP8ljkOS8aJ7qzyCBYg1
       4dfgD0BHvmKPT8dd8A0dzQMhO2e0FxYqVPECQQCooL9NYEXnW2l0q+gAhKD3xPiE
       q/kCFrq131cMW+6QnqdL7pGhHXS+QZxsIY4pnPcn3YStmgc8lavNYrac4rJ9AkEA
       oUm6cMxEMIeiVjkaeczg/s7spN4I/CbEpBbeb0d0oDFK7i/Lbz1xmqK2PCC9WO97
       k//cvogas0P1QTnsXxWb8QJAS+vmIYWH/3bjK/Vl9fHOMP9PJUfAAJyH3U7umF/c
       gL6jJHJTEY6zpOfr5dWXRXPWilxOGxhMcJVk3uqBUDCdhw==
       -----END RSA PRIVATE KEY-----";

                   if (!string.IsNullOrEmpty(base64privatekey))
                   {
                       HashAlgorithm hash = new SHA256Managed();
                       // HACK!! simulate the quoted-printable encoding SmtpClient will use
                       string hashBody = message.Body.Replace(Environment.NewLine, "=0D=0A") + Environment.NewLine;
                       byte[] bodyBytes = Encoding.ASCII.GetBytes(hashBody);
                       string hashout = Convert.ToBase64String(hash.ComputeHash(bodyBytes));
                       TimeSpan t = DateTime.Now.ToUniversalTime() - DateTime.SpecifyKind(DateTime.Parse("00:00:00 January 1, 1970"), DateTimeKind.Utc);


                       var signatureHeader =
                       "v=1; " +
                       "a=rsa-sha256; " +
                       "c=relaxed/relaxed; " +
                       "d=domain.com; " +
                       "s=selector; " +
                       "t=" + Convert.ToInt64(t.TotalSeconds) + "; " +
                       "bh=" + hashout + "; " +
                       "h=From:To:Subject:" + // Content-Type:Content-Transfer-Encoding; " +
                       "b=";

                       // Create the canonical Headers
                       var canonicalizedHeaders =
                       "from:" + message.From.ToString() + Environment.NewLine +
                       "to:" + message.To[0].ToString() + Environment.NewLine +
                       "subject:" + message.Subject + Environment.NewLine +
                       "content-type:text/plain; charset=us-ascii" + Environment.NewLine +
                       "content-transfer-encoding:quoted-printable" + Environment.NewLine +
                       "dkim-signature:" + signatureHeader;

                       TextReader reader = new StringReader(base64privatekey);
                       Org.BouncyCastle.OpenSsl.PemReader r = new Org.BouncyCastle.OpenSsl.PemReader(reader);
                       AsymmetricCipherKeyPair o = r.ReadObject() as AsymmetricCipherKeyPair;
                       byte[] plaintext = Encoding.ASCII.GetBytes(canonicalizedHeaders);
                       ISigner sig = SignerUtilities.GetSigner("SHA256WithRSAEncryption");
                       sig.Init(true, o.Private);
                       sig.BlockUpdate(plaintext, 0, plaintext.Length);
                       byte[] signature = sig.GenerateSignature();
                       signatureHeader += Convert.ToBase64String(signature);

                       message.Headers.Add("DKIM-Signature", signatureHeader);
                   }
               }
                * */
        }
    }

}

