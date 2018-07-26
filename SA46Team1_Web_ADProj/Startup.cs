using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SA46Team1_Web_ADProj.App_Start;
using SA46Team1_Web_ADProj.provider;
using System;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(SA46Team1_Web_ADProj.Startup))]
namespace SA46Team1_Web_ADProj
{
   
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
            ConfigureAuth(app);
        }
    }
}