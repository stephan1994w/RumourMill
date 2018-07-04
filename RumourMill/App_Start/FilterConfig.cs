using System.Web;
using System.Web.Mvc;

namespace RumourMill
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //HandleErrorAttribute is the class that will handle all exceptions thrown by the action method.
            filters.Add(new HandleErrorAttribute());
            //AuthorizeAttribute specifies access to a controller or action method that is restricted to users who meet the authorization requirement.
            filters.Add(new AuthorizeAttribute());
        }
    }
}
