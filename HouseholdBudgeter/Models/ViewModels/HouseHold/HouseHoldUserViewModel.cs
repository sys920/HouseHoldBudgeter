using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels
{
    public class HouseHoldUserViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}