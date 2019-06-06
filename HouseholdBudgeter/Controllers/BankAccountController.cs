using AutoMapper;
using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels.BankAccount;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RoutePrefix("api/BankAccount")]
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext DbContext;
        private Validation Validation;
        public BankAccountController()
        {
            DbContext = new ApplicationDbContext();
            Validation = new Validation();
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(BankAccountBindigModel formData)
        {                
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(formData.HouseHoldId, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }
           
            var bankAccount = Mapper.Map<BankAccount>(formData);
            bankAccount.Created = DateTime.Now;       
            bankAccount.Balance = 0;

            DbContext.BankAccounts.Add(bankAccount);
            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountViewModel>(bankAccount);
            return Ok(model);
        }

        [HttpPut]
        [Route("Update/{id:int}")]
        public IHttpActionResult Update(int id, BankAccountUpdateBindigModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            if (bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            Mapper.Map(formData, bankAccount); 
            bankAccount.Updated = DateTime.Now;                
            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountViewModel>(bankAccount);           

            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            if (bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }           

            DbContext.BankAccounts.Remove(bankAccount);
            DbContext.SaveChanges();          

            return Ok();
        }

        [HttpGet]
        [Route("GetAll/{id:int}")]
        public IHttpActionResult GetAll(int id)
        {
            var userId = User.Identity.GetUserId();

            var isUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!isUserMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the memeber of this houseHold");
                return BadRequest(ModelState);
            }

            var models = DbContext.BankAccounts.Where(p => p.HouseHoldId == id).Select(p => new BankAccountViewModel
            {
                HouseHoldId = p.HouseHoldId,
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated,
                Balance =p.Balance,
                IsOwner =(p.HouseHold.Owner.Id == userId),
                NumberOfTransaction = DbContext.Transactions.Where(x => x.BankAccountId == p.Id).Count(),

        }).ToList();

            return Ok(models);
        }

        [HttpGet]
        [Route("GetById/{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            if (bankAccount == null)
            {
                ModelState.AddModelError("CategoryId", "Sorry, This category does not exist");
                return BadRequest(ModelState);
            }

            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var model = Mapper.Map<BankAccountViewModel>(bankAccount);
           
            return Ok(model);
        }

        [HttpGet]
        [Route("CalcurateBalance/{id:int}")]
        public IHttpActionResult CalcurateBalance(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "Sorry, The bankAccount does't exit");
                return BadRequest(ModelState);
            }

            var isOwnerOfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!isOwnerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this bankAccount");
                return BadRequest(ModelState);
            }

            var listOfAmount = DbContext.Transactions.Where(p => p.BankAccountId == id && p.Void == false).Select(p => p.Amount).ToList();

            //var bankAccount.Balance = DbContext.Transactions.Where(p => p.BankAccountId == id && p.Void == false).Sum(p => p.Amount);

            var balance = 0m;
            //decimal balance = 0;

            foreach (var ele in listOfAmount)
            {
                balance = balance + ele;
            }

            bankAccount.Balance = balance;
            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountViewModel>(bankAccount);
            return Ok(model);
        }
    }
}
