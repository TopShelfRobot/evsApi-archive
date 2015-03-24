using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using evs.API.Providers;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using System.Configuration;
using evs.API.Infrastructure;
//using Microsoft.Owin.Security.DataHandler;

[assembly: OwinStartup(typeof(evs.API.Startup))]
namespace evs.API
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            //WebApiConfig.Register(config);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {

            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
           
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider(),
               // AccessTokenFormat = new TicketDataFormat(app.CreateDataProtector(typeof(OAuthAuthorizationServerMiddleware).Namespace,"Access_Token", "v1")),
                //AccessTokenFormat = new TicketDataFormat(app.
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            //OAuthBearerOptions.AccessTokenFormat = OAuthServerOptions.AccessTokenFormat;

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configure Google External Login
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "xxxxxx",
                ClientSecret = "xxxxxx",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId =  ConfigurationManager.AppSettings["fbAppId"],   //"447816931997733",  
                AppSecret = ConfigurationManager.AppSettings["fbAppSecret"], //"ed72d01390147d4be82cd86e6d7106e4",
                Provider = new FacebookAuthProvider()
                //{
                //    OnAuthenticated = (context) =>
                //    {
                //        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, XmlSchemaString, "Facebook"));
                //        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, XmlSchemaString, "Facebook"));
                //        return Task.FromResult(0);
                //    }
                //}
            };
            facebookAuthOptions.Scope.Add("email");
            app.UseFacebookAuthentication(facebookAuthOptions);
        }
    }
}