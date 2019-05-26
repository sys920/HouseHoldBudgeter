using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;


namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        private ApplicationDbContext DbContext { get; set; }
        private Validation Validation { get; set; }        

        public CategoryController()
        {
            DbContext = new ApplicationDbContext();
            Validation = new Validation();
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(CategoryBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isHouseHoldExit = Validation.IsHouseHoldExist(formData.HouseHoldId);
            if (!isHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(formData.HouseHoldId, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }           

            var category = Mapper.Map<Category>(formData);           
            category.Created = DateTime.Now;        

            DbContext.Categories.Add(category);
            DbContext.SaveChanges();
           
            var model = Mapper.Map<CategoryViewModel>(category);           

            return Ok(model);
        }

        [HttpPut]
        [Route("Update/{id:int}")]
        public IHttpActionResult Update(int id, CategoryUpdateBidingModel formData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == id);           
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Sorry, This category does not exist");
                return BadRequest(ModelState);
            }

            
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(category.HouseHoldId, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            Mapper.Map(formData, category);
            category.Updated = DateTime.Now;            

            DbContext.SaveChanges();

            var model = Mapper.Map<CategoryViewModel>(category);           

            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id) 
        {
            var userId = User.Identity.GetUserId();
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == id);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Sorry, This category does not exist");
                return BadRequest(ModelState);
            }
           
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(category.HouseHoldId, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            DbContext.Categories.Remove(category);
            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("GetAllCategory/{id:int}")]
        public IHttpActionResult GetAllCategory(int id)
        {
            var userId = User.Identity.GetUserId();
          
            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            var isUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            
            if(!(isUserOnwerOfHouseHold || isUserMemberOfHouseHold))
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }            

            var models = DbContext.Categories.Where(p => p.HouseHoldId == id).Select( p => new CategoryViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated
            }).ToList();

            return Ok(models);
        }

    }
}
