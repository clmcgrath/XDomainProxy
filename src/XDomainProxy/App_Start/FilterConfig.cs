using System.Web;
using System.Web.Mvc;

namespace XDomainProxy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    }
    }
}
