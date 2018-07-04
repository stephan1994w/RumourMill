using Microsoft.Owin.Security.Cookies;
using Owin;
namespace RumourMill
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
            });
        }
    }
}