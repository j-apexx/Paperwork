using Microsoft.AspNet.FriendlyUrls;
using System.Web.Routing;

namespace Paperwork
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            //settings.AutoRedirectMode = RedirectMode.Permanent;
            //3/6/2023 - changing the above line allowed me to use a WebMethod from within the SPecialFunctionsENtry page to create autocomplete on the Org Billed field
            settings.AutoRedirectMode = RedirectMode.Off;
            routes.EnableFriendlyUrls(settings);
        }
    }
}
