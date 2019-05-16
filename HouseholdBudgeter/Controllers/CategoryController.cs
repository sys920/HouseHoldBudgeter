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
        [Route("CreateCategory/{id:int}")]
        public IHttpActionResult CreateCategory(int id, CreateCategoryViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                return BadRequest("You are not the owner of this houseHolde");
            }

            var category = new Category();
            category.Name = formData.Name;
            category.Description = formData.Description;
            category.Created = DateTime.Now;
            category.HouseHoldId = id;

            DbContext.Categories.Add(category);
            DbContext.SaveChanges();

            return Ok("Category was created successfully!");
        }

        [HttpPut]
        [Route("UpdateCategory/{id:int}")]
        public IHttpActionResult UpdateCategory (int id, UpdaateCategoryViewModel formData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                return BadRequest("Sorry, Household does not exist on the database");
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);           
            if (!IsUserOnwerOfHouseHold)
            {
                return BadRequest("You are not the owner of this houseHolde");
            }

            var IsCategoryExist = Validation.IsCategoryExist(id, formData.CategoryId);            
            if (!IsCategoryExist)
            {
                return BadRequest("Sorry, The category doesn't match to the database!");
            }

            var model = DbContext.Categories.FirstOrDefault(p => p.Id == formData.CategoryId && p.HouseHoldId == id);
            model.Name = formData.Name;
            model.Description = formData.Description;
            model.Updated = DateTime.Now;

            DbContext.SaveChanges();

            return Ok("The category was updated successfully!");
        }

        [HttpPost]
        [Route("DeleteCategory/{id:int}")]
        public IHttpActionResult DeleteCategory (int id, DeleteCategoryViewModel formData) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                return BadRequest("Sorry, Household does not exist on the database");
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                return BadRequest("You are not the owner of this houseHolde");
            }

            var IsCategoryExist = Validation.IsCategoryExist(id, formData.CategoryId);
            if (!IsCategoryExist)
            {
                return BadRequest("Sorry, The category doesn't match to the database!");
            }

            var model = DbContext.Categories.FirstOrDefault(p => p.Id == formData.CategoryId && p.HouseHoldId == id);
            DbContext.Categories.Remove(model);
            DbContext.SaveChanges();

            return Ok("The category was deleted successfully!");
        }

        [HttpGet]
        [Route("GetAllCategoryOfHouseHold/{id:int}")]
        public IHttpActionResult GetAllCategoryOfHouseHold(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                return BadRequest("Sorry, Household does not exist on the database");
            }

            var userId = User.Identity.GetUserId();
            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            var IsUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            
            if(!(IsUserOnwerOfHouseHold || IsUserMemberOfHouseHold))
            {
                return BadRequest("Sorry, You are not member of this houseHold");
            }            

            var categories = DbContext.Categories.Where(p => p.HouseHoldId == id).Select( p => new CategoryListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated
            }).ToList();

            return Ok(categories);
        }

    }
}
