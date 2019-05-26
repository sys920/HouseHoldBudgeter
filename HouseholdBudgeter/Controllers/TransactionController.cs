using AutoMapper;
using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels.BankAccount;
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
        [Route("Create")]
        public IHttpActionResult Create(TransactionBindingModel formData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formData.BankAccountId);           
            if (bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }

            var isMemberOfHouseHold = Validation.IsMemberOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!isMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }   
           
            var isCategoryExist = Validation.IsCategoryExist(bankAccount.HouseHoldId, formData.CategoryId);
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);
            }
            
            var transaction = Mapper.Map<Transaction>(formData);           
            transaction.Created = DateTime.Now;
            transaction.OwnerId = userId;

            DbContext.Transactions.Add(transaction);            
            DbContext.SaveChanges();

            var model = Mapper.Map<TransactionViewModel>(transaction);

            CalcurateBalance(transaction.BankAccountId);

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
           
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);            
            if(transaction == null)
            {
                ModelState.AddModelError("TransactionId", "This transactionId doesn't exist");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);

            var isCategoryExist = Validation.IsCategoryExist(bankAccount.HouseHoldId, formData.CategoryId);
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);
            }

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(id, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Update");
                return BadRequest(ModelState);

            }

            Mapper.Map(formData, transaction);
            transaction.Updated = DateTime.Now;           
            DbContext.SaveChanges();

            var model = Mapper.Map<TransactionViewModel>(transaction);

            CalcurateBalance(transaction.BankAccountId);

            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);
            if (transaction == null)
            {
                ModelState.AddModelError("TransactionId", "This transactionId doesn't exist");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);
                       

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(id, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Update");
                return BadRequest(ModelState);
            }          
           
            DbContext.Transactions.Remove(transaction);
            DbContext.SaveChanges();

            CalcurateBalance(transaction.BankAccountId);

            return Ok();
        }

        [HttpPut]
        [Route("Void/{id:int}")]
        public IHttpActionResult Void(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);
            if (transaction == null)
            {
                ModelState.AddModelError("TransactionId", "This transactionId doesn't exist");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(id, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Update");
                return BadRequest(ModelState);
            }
          
            transaction.Void = true;
           
            DbContext.SaveChanges();

            CalcurateBalance(transaction.BankAccountId);

            return Ok();
        }

        [HttpGet]
        [Route("GetAll/{id:int}")]
        public IHttpActionResult GetAll(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);

            if(bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "Sorry, The bankAccount does't exit");
                return BadRequest(ModelState);
            }                      
           
            var isMemberOfHouseHold = Validation.IsMemberOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!isMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }

            var transactions = DbContext.BankAccounts.Where(p => p.Id == id).Select(p => new BankAccountListViewModel
            {
                BankAccountId = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated,
                Balance =p.Balance,
                Transaction = p.Transactions.Select( x => new TransactionListViewModel {
                    TransactionId = x.Id,
                    Name =x.Name,
                    Description = x.Description,
                    Date = x.Date,
                    Amount =x.Amount,
                    Category = x.Category.Name,
                    Created = x.Created,
                    Updated =x.Updated,
                    Void = x.Void,
                }).ToList()
            }).ToList();                                    

            return Ok(transactions);
        }

        private void CalcurateBalance(int id)
        {
            var listOfTransactionAmount = DbContext.Transactions.Where(p => p.BankAccountId == id && p.Void == false).Select(p => p.Amount).ToList();

            decimal balance = 0;

            foreach (var ele in listOfTransactionAmount)
            {
                balance = balance + ele;
            }

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            bankAccount.Balance = balance;

            DbContext.SaveChanges();
        }
    }
}
