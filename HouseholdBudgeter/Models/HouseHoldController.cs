using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HouseholdBudgeter.Models
{
    [RoutePrefix("api/Household")]
    public class HouseHoldController : ApiController
    {
        private ApplicationDbContext DbContext { get; set; }

        public HouseHoldController()
        {
            DbContext = new ApplicationDbContext();
        }

        [Authorize]
        [Route("Create")]
        public IHttpActionResult Create(CreateHouseholdViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var houseHold = new HouseHold();
            houseHold.Name = formData.Name;
            houseHold.Desrcipton = formData.Description;
            houseHold.Created = DateTime.Now;
            houseHold.OwnerId = HttpContext.Current.User.Identity.GetUserId();

            DbContext.HouserHolds.Add(houseHold);
            DbContext.SaveChanges();
           
            return Ok("Create sucessfully!");
        }

        //[Authorize]
        [Route("GetAll")]
        public IHttpActionResult GetAll()
        {
           
            return Ok();
        }

    }
}
