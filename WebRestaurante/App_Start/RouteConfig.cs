using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebRestaurante.Models;

namespace WebRestaurante
{
    public class RouteConfig
    {

        public static void RegisterRoutes(RouteCollection routes)
        {
           WebRestauranteContext db = new WebRestauranteContext();
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
            new { botdetect = @"(.*) BotDetectCaptcha\.ashx" });
            if (db.Restaurantes.Count() == 0)
            {
                routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "RestaurantesConf", action = "Create", id = UrlParameter.Optional }
            );
            }
            else
            {
                routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            }
        }
    }
}
