using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels;
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

        public HouseHoldController()
        {
            DbContext = new ApplicationDbContext();
            CustomEmailService = new CustomEmailService();
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(CreateHouseholdViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var houseHold = new HouseHold();
            houseHold.Name = formData.Name;
            houseHold.Desrcipton = formData.Description;
            houseHold.Created = DateTime.Now;
            houseHold.OwnerId = HttpContext.Current.User.Identity.GetUserId();

            DbContext.HouseHolds.Add(houseHold);
            DbContext.SaveChanges();

            return Ok("HouseHold was Created sucessfully!");
        }
        [HttpPut]
        [Route("Update/{id:int}")]
        public IHttpActionResult Update(int id, CreateHouseholdViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id && p.OwnerId == userId);
            if (houseHold == null)
            {
                return NotFound();
            }

            houseHold.Name = formData.Name;
            houseHold.Desrcipton = formData.Description;
            houseHold.Updated = DateTime.Now;

            DbContext.SaveChanges();

            return Ok("The household was updated Successfully");
        }

        [HttpPost]
        [Route("InviteUser/{id:int}")]
        public IHttpActionResult InviteUser(int id, InviteUserByEmailViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id && p.OwnerId == userId);
            if (houseHold == null)
            {
                return NotFound();
            }

            var userRegistered = DbContext.Users.Any(p => p.Email == formData.Email);
            if (!userRegistered)
            {
                return BadRequest("This email is not registered yet.");
            }


            var invitationExist = DbContext.Invitations.Any(p => p.HouseHoldId == id && p.UserEmail == formData.Email);
            if (invitationExist)
            {
                return BadRequest("Already, You had invited this email");
            }

            var invitation = new Invitation();
            invitation.UserEmail = formData.Email;
            invitation.HouseHoldId = id;

            houseHold.Invitations.Add(invitation);
            DbContext.SaveChanges();

            MailAddress userEmailAddress = new MailAddress(formData.Email);
            var message = new MailMessage();
            message.From = new MailAddress("system@mailtrap.io", "MailSystem");
            message.To.Add(userEmailAddress);
            message.Subject = "HouseHold Invitaion";
            message.Body = "HouseHold Name : " + houseHold.Name + ", HouseHold ID : " + houseHold.Id;
            CustomEmailService.Sending(message);

            return Ok("Your invitaion was sent successfully!");
        }

        [HttpPost]
        [Route("AcceptInvitaion/{id:int}")]
        public IHttpActionResult AcceptInvitaion(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userEmail = User.Identity.GetUserName();
            var userId = User.Identity.GetUserId();

            var InvitationExist = DbContext.Invitations.FirstOrDefault(p => p.HouseHoldId == id && p.UserEmail == userEmail);
            if (InvitationExist==null)
            {
                return BadRequest("Sorry, You are not invited to this houseHold");
            }

            var userExist = DbContext.HouseHoldUsers.Any(p => p.HouserholdId == id && p.UserId == userId);
            if(userExist)
            {
                DbContext.Invitations.Remove(InvitationExist);
                DbContext.SaveChanges();
                return BadRequest("You are already the member of this houseHold!");
            }
            var model = new HouseHoldUser();
            model.HouserholdId = id;
            model.UserId = userId;

            DbContext.HouseHoldUsers.Add(model);                     
            DbContext.Invitations.Remove(InvitationExist);
            DbContext.SaveChanges();

            return Ok("Thank You, you joined this houserHold");
        }

        [HttpGet]
        [Route("GetAllMemberOfHouserHold/{id:int}")] 
        public IHttpActionResult GetAllMemberOfHouserHold(int id)
        {
            var userId = User.Identity.GetUserId();

            var OwnerOFHouseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id && p.OwnerId == userId);
            if (OwnerOFHouseHold == null)
            {
                var userExist = DbContext.HouseHoldUsers.Any(p => p.HouserholdId == id && p.UserId == userId);
                if (!userExist)
                {
                    return BadRequest("Sorry, You are not member of this houseHold");
                }
            }                  
                       
            var model = DbContext.HouseHoldUsers.Where(p => p.HouserholdId == id).Select(p => new HouseHoldUserViewModel {
              UserEmail = p.User.Email,
              UserId = p.User.Id
              }).ToList();

            return Ok(model);
        }

        [HttpGet]
        [Route("LeaveHouseHold/{id:int}")]
        public IHttpActionResult LeaveHouseHold(int id)
        {
            var userId = User.Identity.GetUserId();

            var user = DbContext.HouseHoldUsers.FirstOrDefault(p => p.HouserholdId == id && p.UserId == userId);
            if (user == null)
            {
                return BadRequest("Sorry, You are not member of this houseHold");
            }

            DbContext.HouseHoldUsers.Remove(user);
            DbContext.SaveChanges();
            return Ok("Thnakyou, You are not a member of this houseHold anymore!");
        }

        [HttpGet]
        [Route("DeleteHouseHold/{id:int}")]
        public IHttpActionResult DeleteHouseHold(int id)
        {
            var userId = User.Identity.GetUserId();

            var OwnerOFHouseHold = DbContext.HouseHolds.FirstOrDefault(p => p.Id == id && p.OwnerId == userId);
            if (OwnerOFHouseHold == null)
            {
                return BadRequest("Sorry, You are not the owner of this houseHold");
            }

            DbContext.HouseHolds.Remove(OwnerOFHouseHold);
            DbContext.SaveChanges();
            return Ok("This houseHold was deleted !");
        }

        [HttpPost]
        [Route("CreateCategory/{id:int}")] 
        public IHttpActionResult CreateCategory(int id, CreateCategoryViewModel formData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var OwnerOfHouseHold = DbContext.HouseHolds.Any(p => p.Id == id && p.OwnerId == userId);
            if(!OwnerOfHouseHold)
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
    }
}
