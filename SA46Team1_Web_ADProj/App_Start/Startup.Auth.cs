using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SA46Team1_Web_ADProj.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using Microsoft.Owin.Security.OAuth;
using SA46Team1_Web_ADProj.provider;

namespace SA46Team1_Web_ADProj
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new SSISdbEntities());
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<RoleManager<Role>>((options, context) =>
                new RoleManager<Role>(
                    new RoleStore<Role>(context.Get<SSISdbEntities>())));           

           app.UseCookieAuthentication(new CookieAuthenticationOptions
           {
               AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
               LoginPath = new PathString("/Main/login"),
               ExpireTimeSpan = TimeSpan.FromMinutes(30)
           });

        }
        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider()
            };
            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}