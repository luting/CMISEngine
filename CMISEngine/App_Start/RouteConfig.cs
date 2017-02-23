using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMISEngine
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
         //   routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //name: "Meteo",
            //url: "{jour}/{mois}/{annee}",
            //defaults: new { controller = "Folders", action = "Afficher" },
            //constraints: new { jour = @"\d+", mois = @"\d+", annee = @"\d+" });
            //routes.MapRoute(
            //    name: "Folders",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Folders", action = "FolderPath", id = UrlParameter.Optional }
            //);
        }
    }
}
