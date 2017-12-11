using System.Web.Http;

/// <summary>
/// Summary description for WebApiConfig
/// </summary>
public class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "4GApi",
            routeTemplate: "4gapi/"
        );
    }
}