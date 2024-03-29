﻿using AutoMapper;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels;
using HouseholdBudgeter.Models.ViewModels.HouseHold;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace HouseholdBudgeter.Models
{
    [Authorize]
    [RoutePrefix("api/Household")]
    public class HouseHoldController : ApiController
    {
        private ApplicationDbContext DbContext { get; set; }
        private CustomEmailService CustomEmailService { get; set; }
        private Validation Validation { get; set; }         

        public HouseHoldController()
        {
            DbContext = new ApplicationDbContext();
            CustomEmailService = new CustomEmailService();
            Validation = new Validation();
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(HouseholdBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();            
            var houseHold = Mapper.Map<HouseHold>(formData);
            houseHold.Created = DateTime.Now;
            houseHold.OwnerId = User.Identity.GetUserId();

            DbContext.HouseHolds.Add(houseHold);
            DbContext.SaveChanges();

            var houseHoldUser = new HouseHoldUser();
            houseHoldUser.HouseHoldId = houseHold.Id;
            houseHoldUser.UserId = houseHold.OwnerId;
            DbContext.HouseHoldUsers.Add(houseHoldUser);
            DbContext.SaveChanges();

            var model = Mapper.Map<HouseHoldViewModel>(houseHold);
            return Ok(model);
        }

        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll()
        {
            var userId = User.Identity.GetUserId();

            var models = DbContext.HouseHolds.Where(p => p.HouseHoldUsers.Any(x => x.UserId == userId)).Select(p => new HouseHoldListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated,
                IsOwner = DbContext.HouseHolds.Any(o => o.Id == p.Id && o.OwnerId == userId),
                NumberOfMember = DbContext.HouseHoldUsers.Where(x => x.HouseHoldId == p.Id).Count(),
                NumberOfCategory = DbContext.Categories.Where(x => x.HouseHoldId == p.Id).Count(),
                NumberOfBankAccount = DbContext.BankAccounts.Where(x => x.HouseHoldId == p.Id).Count()

            }).ToList();

            return Ok(models);
        }

        [HttpGet]
        [Route("GetByOwnerId/{id:int}")]
        public IHttpActionResult GetByOwnerId(int id)
        {
            var userId = User.Identity.GetUserId();

            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the owner of this houseHold");
                return BadRequest(ModelState);
            }

            var result = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id);
            var model = Mapper.Map<HouseHoldViewModel>(result);

            return Ok(model);
        }

        [HttpGet]
        [Route("GetById/{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            var userId = User.Identity.GetUserId();

            var IsMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!IsMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, You are not the member of this houseHold");
                return BadRequest(ModelState);
            }

            var result = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id);
            var model = Mapper.Map<HouseHoldDetailViewModel>(result);

            return Ok(model);
        }

        [HttpPut]
        [Route("Update/{id:int}")]
        public IHttpActionResult Update(int id, HouseholdBindingModel formData)
        {
            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

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

            var houseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id); 
            houseHold.Name = formData.Name;
            houseHold.Description = formData.Description;
            houseHold.Updated = DateTime.Now;

            DbContext.SaveChanges();

            var model = new HouseHoldViewModel();
            model.Id = houseHold.Id;
            model.Name = houseHold.Name;
            model.Description = houseHold.Description;

            return Ok(model);
        }

        [HttpPost]
        [Route("InviteUser/{id:int}")]
        public IHttpActionResult InviteUser(int id, InviteUserBidingModel formData)
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
            var isUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (!isUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the owner of this houseHold");
                return BadRequest(ModelState);
            }                        

            var isUserRegistered = DbContext.Users.Any(p => p.Email == formData.Email);
            if (!isUserRegistered)
            {
                ModelState.AddModelError("UserInvite", "This email is not registered yet");
                return BadRequest(ModelState);
            }

            var isUserMemberOfHouseHold = DbContext.HouseHoldUsers.Any(p => p.HouseHoldId == id &&  p.User.Email == formData.Email);
            if (isUserMemberOfHouseHold)
            {
                ModelState.AddModelError("UserInvite", "This user(email) is already in this household");
                return BadRequest(ModelState);
            }

            var invitationExist = DbContext.Invitations.Any(p => p.HouseHoldId == id && p.UserEmail == formData.Email);
            if (invitationExist)
            {
                ModelState.AddModelError("UserInvite", "This Email was on the list of invitation already");
                return BadRequest(ModelState);
            }

            var invitation = new Invitation();
            invitation.UserEmail = formData.Email;
            invitation.HouseHoldId = id;

            var houseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id && p.OwnerId == userId);
            houseHold.Invitations.Add(invitation);
            DbContext.SaveChanges();

            MailAddress userEmailAddress = new MailAddress(formData.Email);
            var message = new MailMessage();
            message.From = new MailAddress("system@mailtrap.io", "MailSystem");
            message.To.Add(userEmailAddress);
            message.Subject = "HouseHold Invitaion";
            message.Body = "HouseHold Name : " + houseHold.Name + ", HouseHold ID : " + houseHold.Id;
            CustomEmailService.Sending(message);

            return Ok();
        }

        [HttpGet]
        [Route("GetAllInvitaion")]
        public IHttpActionResult GetAllInvitaion()
        {            
            var userEmail = User.Identity.GetUserName();

            var invitations = DbContext.Invitations.Where(p => p.UserEmail == userEmail).Select(p => new ListOfInvitation {
            Id = p.HouseHoldId,
            Name =p.HouserHold.Name,
            Description = p.HouserHold.Description
            }).ToList();                     

            return Ok(invitations);
        }

        [HttpGet]
        [Route("AcceptInvitaion/{id:int}")]
        public IHttpActionResult AcceptInvitaion(int id)
        {
            var isHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!isHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userEmail = User.Identity.GetUserName();
            var userId = User.Identity.GetUserId();

            var invitationExist = DbContext.Invitations.Any(p => p.HouseHoldId == id && p.UserEmail == userEmail);
            if (!invitationExist)
            {
                ModelState.AddModelError("UserInvite", "Sorry, User was not invited to this houseHold");
                return BadRequest(ModelState);
            }

            var invitation = DbContext.Invitations.FirstOrDefault(p => p.HouseHoldId == id && p.UserEmail == userEmail);

            var isUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (isUserMemberOfHouseHold)
            {               
                DbContext.Invitations.Remove(invitation);
                DbContext.SaveChanges();

                ModelState.AddModelError("UserId", "You are the member of this houseHold");
                return BadRequest(ModelState);              
            }

            var model = new HouseHoldUser();
            model.HouseHoldId = id;
            model.UserId = userId;

            DbContext.HouseHoldUsers.Add(model);                     
            DbContext.Invitations.Remove(invitation);
            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("GetAllMember/{id:int}")] 
        public IHttpActionResult GetAllMember(int id)
        {
            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var IsUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!IsUserMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }

            var models = DbContext.HouseHoldUsers.Where(p => p.HouseHoldId == id).Select(p => new HouseHoldUserViewModel {
              UserEmail = p.User.Email,
              UserId = p.User.Id
              }).ToList();

            return Ok(models);
        }

        [HttpDelete]
        [Route("Leave/{id:int}")]
        public IHttpActionResult Leave(int id)
        {
            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var IsUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!IsUserMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }

            var IsUserOnwerOfHouseHold = Validation.IsOwnerOfHouseHold(id, userId);
            if (IsUserOnwerOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you can not leave this houseHold because you are the owner of this");
                return BadRequest(ModelState);
            }

            var user = DbContext.HouseHoldUsers.FirstOrDefault(p => p.HouseHoldId == id && p.UserId == userId);
            DbContext.HouseHoldUsers.Remove(user);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if(!IsHouseHoldExit)
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
           
            var houseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id && p.OwnerId == userId);
            DbContext.HouseHolds.Remove(houseHold);

            var categories = DbContext.Categories.Where(p => p.HouseHoldId == id).ToList();            
            foreach(var ele in categories)
            {
                DbContext.Categories.Remove(ele);
            }
                       
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("GetHouseHoldDetail/{id:int}")]
        public IHttpActionResult GetHouseHoldDetail(int id)
        {
            var userId = User.Identity.GetUserId();
            var IsHouseHoldExit = Validation.IsHouseHoldExist(id);
            if (!IsHouseHoldExit)
            {
                ModelState.AddModelError("HouserHoldId", "Sorry, The household does not exist on the database");
                return BadRequest(ModelState);
            }

            var IsUserMemberOfHouseHold = Validation.IsMemberOfHouseHold(id, userId);
            if (!IsUserMemberOfHouseHold)
            {
                ModelState.AddModelError("UserId", "Sorry, you are not the member of this houseHold");
                return BadRequest(ModelState);
            }
            
            var result = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id);

            var model = Mapper.Map<HouseHoldDetailViewModel>(result);
            
             model.bankAccountListForHouseHolds = DbContext.BankAccounts.Where(p => p.HouseHoldId == id).Select(p => new BankAccountListForHouseHold
              {
                BankAccountId = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Updated = p.Updated,        
                Balance = p.Balance,
                SumOfCategories = DbContext.Transactions.Where(x => x.BankAccountId == p.Id && x.Void == false).GroupBy(i => i.CategoryId).Select(i => new
                SumOfCategory
                {                  
                    Total = i.Sum(s => s.Amount),
                    Name = i.FirstOrDefault().Category.Name,                 
                }).ToList()
             }).ToList();


            return Ok(model);
        }

    }
}
