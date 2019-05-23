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
        [Route("Create/{id:int}")]
        public IHttpActionResult Create(int id, TransactionBindingModel formData)
        {
            if(!ModelState.IsValid)
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

            var transaction = new Transaction();
                        
            transaction.Name = formData.Name;
            transaction.Description = formData.Description;
            transaction.Date = formData.Date;
            transaction.CategoryId = formData.CategoryId;
            transaction.Amount = formData.Amount;
            transaction.BankAccountId = id;

            transaction.Created = DateTime.Now;
            transaction.OwnerId = userId;
            DbContext.Transactions.Add(transaction);            
            DbContext.SaveChanges();

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
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }
                        
            var isCategoryExist = Validation.IsCategoryExist(bankAccount.HouseHoldId, formData.CategoryId);
            if (!isCategoryExist)
            {
                ModelState.AddModelError("CategoryId", "This category doesn't exist");
                return BadRequest(ModelState);
            }
            
            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(formData.TransactionId, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Update");
                return BadRequest(ModelState);

            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == formData.TransactionId && p.BankAccountId == bankAccount.Id);

            var amount = formData.Amount - transaction.Amount;
            transaction.Name = formData.Name;
            transaction.Description = formData.Description;
            transaction.Date = formData.Date;
            transaction.CategoryId = formData.CategoryId;
            transaction.Amount = formData.Amount;
            transaction.Updated = DateTime.Now;           
            DbContext.SaveChanges();           

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
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            if (bankAccount == null)
            {
                ModelState.AddModelError("BankAccountId", "This bankAccount doesn't exist");
                return BadRequest(ModelState);
            }
            
            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(formData.TransactionId, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Update");
                return BadRequest(ModelState);
            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == formData.TransactionId && p.BankAccountId == id);
           
            DbContext.Transactions.Remove(transaction);
            DbContext.SaveChanges();           

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
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);           
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

            var isTransactionExist = Validation.IsTransactionExist(id, formData.TransactionId);
            if (!isTransactionExist)
            {
                ModelState.AddModelError("TransactionId", "This Transaction doesn't exist");
                return BadRequest(ModelState);
            }

            var isOwnerOfTransaction = Validation.IsOwnerOfTransaction(formData.TransactionId, userId);
            var isOwnerfHouseHold = Validation.IsOwnerOfHouseHold(bankAccount.HouseHoldId, userId);
            if (!(isOwnerfHouseHold || isOwnerOfTransaction))
            {
                ModelState.AddModelError("TransactionId", "You don't have permission to Void");
                return BadRequest(ModelState);
            }

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == formData.TransactionId && p.BankAccountId == id);
            var amount = -(transaction.Amount);

            transaction.Void = true;
           
            DbContext.SaveChanges();          
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

        
    }
}
