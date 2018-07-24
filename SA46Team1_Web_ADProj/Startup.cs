using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SA46Team1_Web_ADProj.Startup))]
namespace SA46Team1_Web_ADProj
{
   
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}