using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Controllers
{
    public class UsersController : ApiController
    {
        public HttpResponseMessage Get()
        {
            using (VehicleTrackingAppEntities1 dbContext = new VehicleTrackingAppEntities1())
            {
                var Employees = dbContext.Users.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, Employees);
            }
        }
        public HttpResponseMessage Get(int id)
        {
            using (VehicleTrackingAppEntities1 dbContext = new VehicleTrackingAppEntities1())
            {
                var entity = dbContext.Users.FirstOrDefault(e => e.UserID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Users with ID " + id.ToString() + "not found");
                }
            }
        }
        public HttpResponseMessage Post([FromBody] User user)
        {
            try
            {
                using (VehicleTrackingAppEntities1 dbContext = new VehicleTrackingAppEntities1())
                {
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, user);
                    message.Headers.Location = new Uri(Request.RequestUri +
                        user.UserID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        //===================Consumeing WEBAPI methods=======================//
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Users/GetUsers")]
        public IHttpActionResult GetUsers()
        {
            //for resolving dependencies required by controller
            using (var dbContext1 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities1>())
            using (var dbContext2 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities2>())
            {
                var users = dbContext1.Users.ToList();
                var userIDs = dbContext1.Users.Select(u => u.UserID).ToList();
                var vehicles = dbContext2.User_Vehicle_Details.Where(v => userIDs.Contains(v.UserID)).ToList();
                var data = new { Users = dbContext1.Users.ToList(), Vehicles = vehicles };

                var jsonString = JsonConvert.SerializeObject(data); // Deserialize the JSON string into a JSON object // var jsonObject = JsonConvert.DeserializeObject(jsonString); 
                return Ok(jsonString);
            }
        }
        public class CombinedModel
        {
            public User user { get; set; }
            public User_Vehicle_Details vehicle { get; set; }
        }



        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Users/CreateUser")]
        public IHttpActionResult CreateUser(CombinedModel model)
        {
            User userData = model.user;
            User_Vehicle_Details vehicleData = model.vehicle;

            using (var dbContext1 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities1>())
            using (var dbContext2 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities2>())
            {
                int maxUserID = dbContext1.Users.Max(u => u.UserID) ?? 0;
                int newUserID = maxUserID + 1;
                userData.UserID = newUserID;
                vehicleData.UserID = newUserID;
                dbContext1.Users.Add(userData);
                dbContext1.SaveChanges();
                dbContext2.User_Vehicle_Details.Add(vehicleData);
                dbContext2.SaveChanges();
            }

                return Ok();
        }
        

        //api/Users/GetUserDetails
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Users/{userId}")]
        public IHttpActionResult GetUserDetails(int userId)
        {
            using (var dbContext1 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities1>())
            using (var dbContext2 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities2>())
            {
                User user = dbContext1.Users.FirstOrDefault(u => u.UserID == userId);
                User_Vehicle_Details vehicle = dbContext2.User_Vehicle_Details.FirstOrDefault(v => v.UserID == userId);

                if (user == null || vehicle ==null)
                {
                    return NotFound(); // User not found
                }
                var data = new { Users = user, Vehicles = vehicle };
                var jsonString = JsonConvert.SerializeObject(data); // Deserialize the JSON string into a JSON object // var jsonObject = JsonConvert.DeserializeObject(jsonString); 
                return Ok(jsonString);
            }
                // Return the user details
                
            
        }
        //public IHttpActionResult GetUserDetails(int userId)
        //{
        //    // Retrieve user details from the database using the userId
        //    using (var dbContext = new VehicleTrackingAppEntities1())
        //    {
        //        User user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);

        //        if (user == null)
        //        {
        //            return NotFound(); // User not found
        //        }
        //        return Ok(user);
        //    }
        //    // Return the user details


        //}


        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("api/Users/{userId}")]
        public IHttpActionResult UpdateUser(int userId, [FromBody]CombinedModel model)
        {
            User user = model.user;
            User_Vehicle_Details vehicleData = model.vehicle;
            using (var dbContext1 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities1>())
            using (var dbContext2 = DependencyResolver.Current.GetService<VehicleTrackingAppEntities2>())
            {
                var existingUser = dbContext1.Users.Find(userId);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Update the user details
                existingUser.Name = user.Name;
                existingUser.MobileNumber = user.MobileNumber;
                existingUser.Organization = user.Organization;
                existingUser.Address = user.Address;
                existingUser.EmailAddress = user.EmailAddress;
                existingUser.Location = user.Location;
                existingUser.Photopath = user.Photopath;

                dbContext1.SaveChanges();

                User_Vehicle_Details vehicle = model.vehicle;
                var existingVehicle = dbContext2.User_Vehicle_Details.Find(vehicle.VehicleNumber);
                // Update the vehicle details
                if (existingVehicle != null && vehicle != null)
                {
                    
                    // Continue setting other properties

                    // existingVehicle.VehicleNumber = vehicle.VehicleNumber;
                    existingVehicle.VehicleType = vehicle.VehicleType;
                    existingVehicle.ChassisNumber = vehicle.ChassisNumber;
                    existingVehicle.EngineNumber = vehicle.EngineNumber;
                    existingVehicle.ManufacturingYear = vehicle.ManufacturingYear;
                    existingVehicle.LoadCarryingCapacity = vehicle.LoadCarryingCapacity;
                    existingVehicle.MakeOfVehicle = vehicle.MakeOfVehicle;
                    existingVehicle.ModelNumber = vehicle.ModelNumber;
                    existingVehicle.BodyType = vehicle.BodyType;
                    existingVehicle.OrganizationName = vehicle.OrganizationName;
                    existingVehicle.DeviceID = vehicle.DeviceID;
                }
                dbContext2.SaveChanges();


                return Ok();
            }
        }


    }
}
