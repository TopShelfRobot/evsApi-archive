using System;
using System.Collections.Generic;
using System.Linq;
//using System.Transactions;
//using System.Web;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Net.Http;
using evs.Model;
using evs.DAL;
using evs30Api.Filters;
using IsolationLevel = System.Data.IsolationLevel;

namespace evs30Api.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //private evsContext db = new evsContext();

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string owner)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.owner = owner;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                // return Html.Partial("_SetPasswordPartial")

                //returnUrl = "/" + returnUrl + "/" + model.UserName;
                return Redirect(returnUrl);
                //return RedirectToLocal(returnUrl);

                //return PartialView("_SetPasswordPartial");  //, OAuthWebSecurity.RegisteredClientData
            }

            // If we got this far, something failed, redisplay form
            ViewBag.ReturnUrl = returnUrl;
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string UserName)
        {
            //check user existance
            var user = Membership.GetUser(UserName);
            if (user == null)
            {
                TempData["Message"] = "User Does Not exist.";
            }
            else
            {
                //generate password token
                var token = WebSecurity.GeneratePasswordResetToken(UserName);
                //create url with above token
                var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
                //get user emailid
                //UsersContext db = new UsersContext();
                var emailid = (from i in db.UserProfiles
                               where i.UserName == UserName
                               select i.UserName).FirstOrDefault();
                //send mail
                string subject = "Password Reset Token";
                string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; //edit it
                try
                {
                    //SendEMail(emailid, subject, body);      //mjb????
                    TempData["Message"] = "Mail Sent.";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error occured while sending email." + ex.Message;
                }
                //only for testing
                TempData["Message"] = resetLink;
            }

            return View();
            //return @Html.Partial("_SetPasswordPartial")
        }



        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string returnUrl)
        {
            WebSecurity.Logout();
            return RedirectToLocal(returnUrl);  //mjb look at this
            //return RedirectToAction("Index", "Home");  //mjb need something else here
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    //return RedirectToAction("Index", "Home");
                    //return RedirectToAction("Index", "Reg");
                    return Redirect(returnUrl);
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                //using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                //{
                //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                //    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                //    {
                //        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                //        scope.Complete();
                //        message = ManageMessageId.RemoveLoginSuccess;
                //    }
                //}
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (evsContext db = new evsContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());   //check something else here mjb
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        [AllowAnonymous]
        public ActionResult PasswordReset(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult PasswordSet(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset(LoginModel model, string returnUrl)
        {
            //we come here but wouls be a lot cooler if 

            if (WebSecurity.UserExists(model.UserName))
            {
                var ptoken=string.Empty;
                try
                {
                    ptoken = WebSecurity.GeneratePasswordResetToken(model.UserName, 190);
                }
                catch 
                {
                    ViewData["msg"] = "You created your account with a social media login.  You don't have a password.";
                    return View(model);
                }
                //var token = WebSecurity.GeneratePasswordResetToken(UserName);

                //var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = model.UserName, rt = ptoken }, "http") + "'>Reset Password</a>";

                //change to be as secure as you choose
                string tmpPass = Membership.GeneratePassword(10, 4);
                WebSecurity.ResetPassword(ptoken, tmpPass);

                HttpResponseMessage result = new MailController().SendPasswordReset(model.UserName, tmpPass);
                //add your own email logic here
                ViewData["msg"] = "A new password has been sent.";
                return View(model);
                //return @Html.Partial("_SetPasswordPartial")


                //generate password token

                //create url with above token

                //var emailid = (from i in db.UserProfiles
                //               where i.UserName == UserName
                //               select i.UserName).FirstOrDefault();
                ////send mail
                //string subject = "Password Reset Token";
                //string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; //edit it
                //try
                //{
                //    //SendEMail(emailid, subject, body);
                //    TempData["Message"] = "Mail Sent.";
                //}
                //catch (Exception ex)
                //{
                //    TempData["Message"] = "Error occured while sending email." + ex.Message;
                //}
                ////only for testing
                //TempData["Message"] = resetLink;




            }
            else
            {
                ModelState.AddModelError("", "The user name wasn't found.");
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Post()
        {

            //int x = 1;
            //registration.EventureListId = regBundle.eventureListId;
            //registration.ParticipantId = regBundle.partId; //IsNullOrEmpty(planRec.approved_by) ? "" : planRec.approved_by.toString();
            //registration.Name = regBundle.displayList;
            //HttpResponseMessage result = Response.;

            var result = new HttpResponseMessage();
            return result;
        }

        //[AllowAnonymous]
        //public ActionResult PasswordReset1(string un, string rt)
        //{
        //    //UsersContext db = new UsersContext();
        //    //TODO: Check the un and rt matching and then perform following
        //    //get userid of received username
        //    var userid = (from i in db.UserProfiles
        //                  where i.UserName == un
        //                  select i.UserId).FirstOrDefault();
        //    //check userid and token matches
        //    bool any = (from j in db.webpages_Memberships
        //                where (j.UserId == userid)
        //                && (j.PasswordVerificationToken == rt)
        //                //&& (j.PasswordVerificationTokenExpirationDate < DateTime.Now)
        //                select j).Any();

        //    if (any == true)
        //    {
        //        //generate random password
        //        string newpassword = GenerateRandomPassword(6);
        //        //reset password
        //        bool response = WebSecurity.ResetPassword(rt, newpassword);
        //        if (response == true)
        //        {
        //            //get user emailid to send password
        //            var emailid = (from i in db.UserProfiles
        //                           where i.UserName == un
        //                           select i.EmailId).FirstOrDefault();
        //            //send email
        //            string subject = "New Password";
        //            string body = "<b>Please find the New Password</b><br/>" + newpassword; //edit it
        //            try
        //            {
        //                //SendEMail(emailid, subject, body);
        //                TempData["Message"] = "Mail Sent.";
        //            }
        //            catch (Exception ex)
        //            {
        //                TempData["Message"] = "Error occured while sending email." + ex.Message;
        //            }

        //            //display message
        //            TempData["Message"] = "Success! Check email we sent. Your New Password Is " + newpassword;
        //        }
        //        else
        //        {
        //            TempData["Message"] = "Hey, avoid random request on this page.";
        //        }
        //    }
        //    else
        //    {
        //        TempData["Message"] = "Username and token not maching.";
        //    }

        //    return View();
        //}

        //private string GenerateRandomPassword(int length)
        //{
        //    string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
        //    char[] chars = new char[length];
        //    Random rd = new Random();
        //    for (int i = 0; i < length; i++)
        //    {
        //        chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        //    }
        //    return new string(chars);
        //}


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            // if (Url.IsLocalUrl(returnUrl))
            //{
            return Redirect(returnUrl);
            // }
            // else
            // {
            //     return RedirectToAction("Index", "Home");
            // }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
