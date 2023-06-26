using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Controllers
{
    public class User_Vehicle_DetailsController : ApiController
    {
        [HttpGet]
        [Route("api/User_Vehicle_Details/GetVehicles")]
        public IHttpActionResult GetVehicles(string searchCriteria, int page = 1, int pageSize = 10)
        {

            using (var db = new VehicleTrackingAppEntities2())
            {
                // Retrieve the list of vehicles from your database using Entity Framework
                var query = db.User_Vehicle_Details.AsQueryable();

                // Apply search criteria (if provided)
                if (!string.IsNullOrEmpty(searchCriteria))
                {
                    query = query.Where(v => v.VehicleNumber.Contains(searchCriteria)
                                            || v.VehicleType.Contains(searchCriteria)
                                            || v.ChassisNumber.Contains(searchCriteria)
                                            || v.EngineNumber.Contains(searchCriteria)
                                            || v.ManufacturingYear.ToString().Contains(searchCriteria)
                                            || v.LoadCarryingCapacity.ToString().Contains(searchCriteria)
                                            || v.MakeOfVehicle.Contains(searchCriteria)
                                            || v.ModelNumber.Contains(searchCriteria)
                                            || v.BodyType.Contains(searchCriteria)
                                            || v.OrganizationName.Contains(searchCriteria)
                                            || v.DeviceID.ToString().Contains(searchCriteria)
                                            || v.UserID.ToString().Contains(searchCriteria));

                }

                // Perform pagination
                var totalItems = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Validate the page number
                if (page < 1 || page > totalPages)
                {
                    return BadRequest("Invalid page number.");
                }

                // Retrieve the vehicles for the requested page
                var vehicles = query.OrderBy(v => v.VehicleNumber)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

                // Prepare the response
                var result = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Vehicles = vehicles
                };
                return Ok(result);
            }
            
        }

    }
}
