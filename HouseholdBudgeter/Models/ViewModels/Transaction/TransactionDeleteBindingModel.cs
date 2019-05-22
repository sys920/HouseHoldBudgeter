using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.Transaction
{
    public class TransactionDeleteBindingModel
    {
        [Required]
        public int BankAccountId { get; set; }
        [Required]
        public int TransactionId { get; set; }
    }
}