using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.BankAccount
{
    public class BankAccountBindigModel
    {        
        [Required]
        public int HouseHoldId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }    
    
    }
}