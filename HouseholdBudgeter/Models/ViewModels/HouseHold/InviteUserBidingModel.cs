using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels
{
    public class InviteUserBidingModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}