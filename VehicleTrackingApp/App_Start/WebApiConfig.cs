using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Http;
using System.Web.Routing;

namespace VehicleTrackingApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //POSTUSER
            config.Routes.MapHttpRoute(
                name: "Users",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
           );

            //GET
            config.Routes.MapHttpRoute(
                name: "GetUserDetails",
                routeTemplate: "api/Users/{userId}",
                defaults: new { controller = "Users", action = "GetUserDetails", userId = RouteParameter.Optional }
                );
            //PUT
            config.Routes.MapHttpRoute(
                name: "UpdateUser",
                routeTemplate: "api/Users/{userId}",
                defaults: new { controller = "Users", action = "UpdateUser" },
                constraints: new { httpMethod = new HttpMethodConstraint("PUT") }
            );

            config.Routes.MapHttpRoute(
            name: "GetVehicles",
            routeTemplate: "api/User_Vehicle_Details/{searchCriteria}/{page}/{pageSize}",
            defaults: new { controller = "User_Vehicle_Details", action = "GetVehicles" }
        );


        }
    }
}
