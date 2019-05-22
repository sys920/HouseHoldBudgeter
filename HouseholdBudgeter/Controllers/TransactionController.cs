using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels.Transaction;
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
    [RoutePrefix("api/Transaction")]
    public class TransactionController : ApiController
    {
        private ApplicationDbContext DbContext;
        private Validation Validation;
        public TransactionController()
        {
            DbContext = new ApplicationDbContext();
            Validation = new Validation();
        }

        [HttpPost]
        [Route("Create/{id:int}")]
        public IHttpActionResult Create(int id, TransactionBindingModel formData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var isMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!isMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }           
            var isBankAccountExist = Validation.IsBankAccountExist(id, formData.BankAccountId);
            if (!isBankAccountExist)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isCategoryExist = Validation.IsCategoryExist(id, formData.CategoryId);
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);
            }

            var transaction = new Transaction();
            transaction.BankAccountId = formData.BankAccountId;            
            transaction.Name = formData.Name;
            transaction.Description = formData.Description;
            transaction.Date = formData.Date;
            transaction.CategoryId = formData.CategoryId;
            transaction.Amount = formData.Amount;          
            transaction.Created = DateTime.Now;
            transaction.OwnerId = userId;
            DbContext.Transactions.Add(transaction);            
            DbContext.SaveChanges();

            CalcurateBalance(transaction.BankAccountId, transaction.Amount);

            var model = new TransactionViewModel();
           
            model.BankAccountId = transaction.BankAccountId;
            model.TransactionId = transaction.Id;
            model.Name = transaction.Name;
            model.Description = transaction.Description;
            model.Date = transaction.Date;
            model.CategoryId = transaction.CategoryId;
            model.Amount =  transaction.Amount;
            model.Created = transaction.Created;

            return Ok(model);
        }

        [HttpPut]
        [Route("Update/{id:int}")]
        public IHttpActionResult Update(int id, TransactionUpdateBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var isMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!isMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }
            var isBankAccountExist = Validation.IsBankAccountExist(id, formData.BankAccountId);
            if (!isBankAccountExist)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isTransactionExist = Validation.IsTransactionExist(formData.BankAccountId, formData.TransactionId);
            if (!isTransactionExist)
            {
                ModelState.AddModelError("TransactionId", "This Transaction doesn't exist");
                return BadRequest(ModelState);
            }            

            var isCategoryExist = Validation.IsCategoryExist(id, formData.CategoryId);
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);
            }

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(formData.TransactionId, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Update");
                return BadRequest(ModelState);

            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == formData.TransactionId && p.BankAccountId == formData.BankAccountId);
            var amount = formData.Amount - transaction.Amount;
            transaction.Name = formData.Name;
            transaction.Description = formData.Description;
            transaction.Date = formData.Date;
            transaction.CategoryId = formData.CategoryId;
            transaction.Amount = formData.Amount;
            transaction.Updated = DateTime.Now;           
            DbContext.SaveChanges();

            CalcurateBalance(transaction.BankAccountId, amount);

            var model = new TransactionViewModel();
            model.BankAccountId = transaction.BankAccountId;
            model.TransactionId = transaction.Id;
            model.Name = transaction.Name;
            model.Description = transaction.Description;
            model.Date = transaction.Date;
            model.CategoryId = transaction.CategoryId;
            model.Amount = transaction.Amount;
            model.Created = transaction.Created;
            model.Updated = transaction.Updated;

            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id, TransactionDeleteBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var isMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!isMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }
            var isBankAccountExist = Validation.IsBankAccountExist(id, formData.BankAccountId);
            if (!isBankAccountExist)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isTransactionExist = Validation.IsTransactionExist(formData.BankAccountId, formData.TransactionId);
            if (!isTransactionExist)
            {
                ModelState.AddModelError("TransactionId", "This Transaction doesn't exist");
                return BadRequest(ModelState);
            }

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(formData.TransactionId, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to delete");
                return BadRequest(ModelState);
            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == formData.TransactionId && p.BankAccountId == formData.BankAccountId);
            var amount = -(transaction.Amount);

            DbContext.Transactions.Remove(transaction);
            DbContext.SaveChanges();

            CalcurateBalance(transaction.BankAccountId, amount);           

            return Ok();
        }

        [HttpPut]
        [Route("Void/{id:int}")]
        public IHttpActionResult Void(int id, TransactionDeleteBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var isMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!isMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }
            var isBankAccountExist = Validation.IsBankAccountExist(id, formData.BankAccountId);
            if (!isBankAccountExist)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isTransactionExist = Validation.IsTransactionExist(formData.BankAccountId, formData.TransactionId);
            if (!isTransactionExist)
            {
                ModelState.AddModelError("TransactionId", "This Transaction doesn't exist");
                return BadRequest(ModelState);
            }

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(formData.TransactionId, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to delete");
                return BadRequest(ModelState);
            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == formData.TransactionId && p.BankAccountId == formData.BankAccountId);
            var amount = -(transaction.Amount);

            transaction.Void = true;
           
            DbContext.SaveChanges();

            CalcurateBalance(transaction.BankAccountId, amount);

            return Ok();
        }

        private void CalcurateBalance(int bankAccountId, decimal amount)
        {
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == bankAccountId);
            bankAccount.Balance = bankAccount.Balance + amount;
            DbContext.SaveChanges();
        }

    }
}
