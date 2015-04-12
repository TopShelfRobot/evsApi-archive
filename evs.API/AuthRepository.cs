using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using evs.DAL;
using evs.Model;
using System.Web.Security;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using System.Globalization;
//using System.Security.Claims;
using evs.API.Infrastructure;


namespace evs.API
{
    public class AuthRepository : IDisposable
    {
        // private AuthContext _ctx;// = new evsContext();
        private evsContext _ctx;//= new evsContext();

        //private UserManager<IdentityUser> _userManager;
        //private ApplicationUserManager _userManager;
        private ApplicationUserManager _AppUserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;

        public AuthRepository()
        {
            _ctx = new evsContext();
            // _ctx = new AuthContext();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));
            //_AppUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //_appRoleManager = Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();

            _AppUserManager = new ApplicationUserManager(new UserStore<IdentityUser>(_ctx) );

             //new UserValidator<TUser>(UserManager) { AllowOnlyAlphanumericUserNames = false }

            //_AppUserManager.UserValidator = new UserValidator(new UserStore<IdentityUser>(_ctx))
            _AppUserManager.UserValidator = new UserValidator<IdentityUser>(_AppUserManager)   //{ AllowOnlyAlphanumericUserNames = false }
            {
                AllowOnlyAlphanumericUserNames = false
                //RequireUniqueEmail = true
            };
      
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            //_AppUserManager.UserValidator = new UserValidator<user>(_AppUserManager) { AllowOnlyAlphanumericUserNames = false };

            var result = await _AppUserManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _AppUserManager.FindAsync(userName, password);

            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);
            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _AppUserManager.FindAsync(loginInfo);
            return user;
        }

        public async Task<IdentityUser> FindAsync(string email)  //mjb
        {
            IdentityUser user = await _AppUserManager.FindByNameAsync(email);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await _AppUserManager.CreateAsync(user);

            return result;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string tkey)     //mjb 
        {
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("ResetPassword");
            _AppUserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<IdentityUser>(provider.Create("ResetPassword"))
              {
                  TokenLifespan = TimeSpan.FromHours(3)
              };

            return _AppUserManager.GeneratePasswordResetToken(tkey);
        }

        //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
        public async Task<bool> SendEmailAsync(string id, string subject, string body)     //mjb 
        {
            await _AppUserManager.SendEmailAsync(id, subject, body);
            //return Ok();
            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string id, string code, string password)     //mjb 
        {

            //try
            //{
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("ResetPassword");
            _AppUserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<IdentityUser>(provider.Create("ResetPassword"))
            {
                TokenLifespan = TimeSpan.FromHours(100)
            };

            var result = await _AppUserManager.ResetPasswordAsync(id, code, password);
            return result;

            //}
            //catch (Exception ex)
            //{
            //    var x = ex.InnerException;
            //    List<string> errors = new List<string>() { ex.Message };
            //    return IdentityResult.Failed(errors.ToArray());
            //}
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _AppUserManager.AddLoginAsync(userId, login);

            return result;
        }

        public List<string> GetAllRoles()
        {
            var rolesList = new List<string>();

            var x = _ctx.Roles
                         .Select(r => new RoleDTO
                         {
                             //RoleId = r.Id,
                             Name = r.Name
                         });

            foreach (var y in x)
            {
                rolesList.Add(y.Name.ToString());
            }

            return rolesList;
        }

        public List<string> GetRolesByUserId(string userId)
        {
            var user = _AppUserManager.FindByName(userId);
            var roles = _AppUserManager.GetRoles(user.Id).ToList();
            return roles;
        }

        public async Task<IdentityResult> AddUsersToRole(List<string> roles, string userName)
        {
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));
            var user = _AppUserManager.FindByName(userName);
            var allRoles = _AppUserManager.GetRoles(user.Id).ToArray();

            var removeResult = _AppUserManager.RemoveFromRoles(user.Id, allRoles);

            var addResult = _AppUserManager.AddToRoles(user.Id, roles.ToArray());
            return addResult;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _AppUserManager.Dispose();

        }
    }
}