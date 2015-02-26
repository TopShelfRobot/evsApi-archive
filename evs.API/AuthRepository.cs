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


namespace evs.API
{
    public class AuthRepository : IDisposable
    {
        // private AuthContext _ctx;// = new evsContext();
        private evsContext _ctx;//= new evsContext();

        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new evsContext();
            // _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

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
            IdentityUser user = await _userManager.FindAsync(loginInfo);
            return user;
        }

        public async Task<IdentityUser> FindAsync(string email)  //mjb
        {
            IdentityUser user = await _userManager.FindByNameAsync(email);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string tkey)     //mjb 
        {
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("ResetPassword");
            _userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<IdentityUser>(provider.Create("ResetPassword"))
              {
                  TokenLifespan = TimeSpan.FromHours(3)
              };

            return _userManager.GeneratePasswordResetToken(tkey);
        }

        //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
        public async Task<bool> SendEmailAsync(string id, string subject, string body)     //mjb 
        {
            await _userManager.SendEmailAsync(id, subject, body);
            //return Ok();
            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string id, string code, string password)     //mjb 
        {

            //try
            //{
                var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("ResetPassword");
                _userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<IdentityUser>(provider.Create("ResetPassword"))
                {
                    TokenLifespan = TimeSpan.FromHours(100)
                };

                var result =  await _userManager.ResetPasswordAsync(id, code, password);
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
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public IEnumerable<RoleDTO> GetAllRolesDTO()   //ByOwnerId
        {
            return _ctx.Roles
                        .Select(r => new RoleDTO
                        {
                            RoleId = r.Id,
                            Name = r.Name
                        });
        }

        //public IEnumerable<RoleDTO> GetAllRolesDTO()   //ByOwnerId
        //{
        //    //_roleManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        //    var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));

        //    //var x = rm.Roles.
        //    return rm.Roles;
        //}

        public IEnumerable<RoleDTO> GetRolesByUserId(string userId)
        //public IEnumerable<IdentityRole> GetRolesByUserId(string userId)
        {
            var user = _userManager.FindByName(userId);
            var roles = _userManager.GetRoles(user.Id);    //.OrderBy(r

           



            var dtos = new List<RoleDTO>();

            var dto = new RoleDTO();
            dto.RoleId = "1";
            dto.Name = "user";

            dtos.Add(dto);


            var dto1 = new RoleDTO();
            dto1.RoleId = "3";
            dto1.Name = "admin";

            dtos.Add(dto1);

            return dtos;


            //foreach(var role in roles)
            //{
            //    var dto = new RoleDTO();
            //    dto.RoleId = 
            //}

            //return _ctx.Roles
            //            .Select(r => new RoleDTO
            //            {
            //                RoleId = r.Id,
            //                Name = r.Name
            //            });

            //var xy = _userManager.GetRoles(userId.Id);
            ////var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));

            //return _userManager.GetRoles(user.Id).ToList();
            //var x = rm.user
            //return xy;
        }

        public async Task<IdentityResult> AddUsersToRole(List<RoleDTO> roles, string userName)     //mjb 
        {

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));
            //rm.
            //Roles.AddUserToRole("boone.mike@gmail.com", "admin");

            var user = _userManager.FindByName(userName);

            //roleManager.

            //var currentUser = _userManager.FindByName(user.UserName);

            //foreach (var role in roles)
            //{
            //    //var roleresult = _userManager.AddToRole(user.Id, role);
            //    var x = _userManager.RemoveFromRoles
            //}
            var newRoles = new string[roles.Count];
            var count = 0;

            foreach (var role in roles)
            {
                newRoles[count] = role.Name;
                count = count + 1;
            }


            //var x = _userManager.RemoveFromRoles   //maybe remove from all roles then add what we want
            //var roleresult = _userManager.add
            var result = await _userManager.AddToRolesAsync(user.Id, newRoles);
            return result;
                

            //string[] userGroup = new string[2];
            //userGroup[0] = "JillShrader";
            //userGroup[1] = "ShaiBassli";
            //Roles.AddUsersToRole(userGroup, "members");
            //return "test";
        }

        //public List<string> GetRolesByUserId(string userId)
        //{
        //    var user = _userManager.FindByName(userId);
        //    return _userManager.GetRoles(user.Id).ToList();
        //}


        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}