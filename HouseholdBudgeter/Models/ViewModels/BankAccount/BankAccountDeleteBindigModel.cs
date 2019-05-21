using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.BankAccount
{
    public class BankAccountDeleteBindigModel
    {
        [Required]
        public int BankAccountId { get; set; }
    }
}