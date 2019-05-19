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
        [Route("Create/{id:int}")]
        public IHttpActionResult Create(int id, CategoryBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!isHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var category = new Category();
            category.Name = formData.Name;
            category.Description = formData.Description;
            category.Created = DateTime.Now;
            category.HouseHoldId = id;

            DbContext.Categories.Add(category);
            DbContext.SaveChanges();

            var model = new CategoryViewModel();
            model.Id = category.Id;
            model.Name = category.Name;
            model.Description = category.Description;
            model.Created = category.Created;
            model.Updated = category.Updated;

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

            var isHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!isHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);           
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var isCategoryExist = Validation.IsCategoryExist(id, formData.CategoryId);            
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);
            }

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formData.CategoryId && p.HouseHoldId == id);
            category.Name = formData.Name;
            category.Description = formData.Description;
            category.Updated = DateTime.Now;

            DbContext.SaveChanges();

            var model = new CategoryViewModel();
            model.Id = category.Id;
            model.Name = category.Name;
            model.Description = category.Description;
            model.Created = category.Created;
            model.Updated = category.Updated;

            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id, CategoryDeleteBidingModel formData) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!isHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var isCategoryExist = Validation.IsCategoryExist(id, formData.CategoryId);
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);                
            }

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formData.CategoryId && p.HouseHoldId == id);
            DbContext.Categories.Remove(category);
            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("GetAllCategory/{id:int}")]
        public IHttpActionResult GetAllCategory(int id)
        {
            var isHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!isHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

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
