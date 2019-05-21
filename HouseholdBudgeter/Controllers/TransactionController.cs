﻿using HouseholdBudgeter.Models;
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
            DbContext.Transactions.Add(transaction);            
            DbContext.SaveChanges();          

            var model = new TransactionViewModel();
            model.CategoryId = transaction.BankAccountId;
            model.Name = transaction.Name;
            model.Description = transaction.Description;
            model.Date = transaction.Date;
            model.CategoryId = transaction.CategoryId;
            model.Amount =  transaction.Amount;
            model.Created = transaction.Created;

            return Ok(model);
        }
    }
}
