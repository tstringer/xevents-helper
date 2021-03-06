﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace xevents_helper
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "EventDescriptionSearch",
                url: "descsearch/{releaseName}/{eventName}",
                defaults: new { controller = "Home", action = "GetEventDescription" }
            );

            routes.MapRoute(
                name: "GetReleaseEvents",
                url: "relevents/{releaseName}",
                defaults: new { controller = "Home", action = "GetAllEventsForRelease" }
            );

            routes.MapRoute(
                name: "SearchEvents",
                url: "searchevents/{releaseName}/{searchTerm}/{searchDescriptions}",
                defaults: new { controller = "Home", action = "SearchEvents" }
            );

            routes.MapRoute(
                name: "GetSessionDefinition",
                url: "getcreatesession",
                defaults: new { controller = "Home", action = "GetCreateSessionDdl" }
            );

            routes.MapRoute(
                name: "GetAllActions",
                url: "actions/{releaseName}",
                defaults: new { controller = "Home", action = "GetAllActionsForRelease" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
