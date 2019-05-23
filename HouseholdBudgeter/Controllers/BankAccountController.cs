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
        [Route("Create/{id:int}")]
        public IHttpActionResult Create(int id, BankAccountBindigModel formData)
        {                
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var bankAccount = new BankAccount();
            bankAccount.Name = formData.Name;
            bankAccount.Description = formData.Description;
            bankAccount.Created = DateTime.Now;
            bankAccount.HouseHoldId = id;
            bankAccount.Balance = 0;

            DbContext.BankAccounts.Add(bankAccount);
            DbContext.SaveChanges();

            var model = new BankAccountViewModel();
            model.Id = bankAccount.Id;
            model.Name = bankAccount.Name;
            model.Description = bankAccount.Description;
            model.Created = bankAccount.Created;
            model.Balance = bankAccount.Balance;

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

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }
          
            var isBankAccountExist = Validation.IsBankAccountExist(id, formData.BankAccountId);
            if (!isBankAccountExist)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formData.BankAccountId && p.HouseHoldId == id);
            bankAccount.Name = formData.Name;
            bankAccount.Description = formData.Description;
            bankAccount.Updated = DateTime.Now;                
            DbContext.SaveChanges();

            var model = new BankAccountViewModel();
            model.Id = bankAccount.Id;
            model.Name = bankAccount.Name;
            model.Description = bankAccount.Description;
            model.Balance = bankAccount.Balance;

            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id, BankAccountDeleteBindigModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var isBankAccountExist = Validation.IsBankAccountExist(id, formData.BankAccountId);
            if (!isBankAccountExist)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formData.BankAccountId && p.HouseHoldId == id);

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
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated,
                Balance =p.Balance,
            }).ToList();

            return Ok(models);
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

            decimal balance = 0;

            foreach (var ele in listOfAmount)
            {
                balance = balance + ele;
            }

            bankAccount.Balance = balance;
            DbContext.SaveChanges();


            var model = new BankAccountViewModel();
            model.Id = bankAccount.Id;
            model.Name = bankAccount.Name;
            model.Description = bankAccount.Description;
            model.Created = bankAccount.Created;
            model.Balance = bankAccount.Balance;

            return Ok(model);
        }
    }
}
