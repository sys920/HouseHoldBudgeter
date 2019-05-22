using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.Transaction
{
    public class TransactionUpdateBindingModel
    {
        [Required]
        public int BankAccountId { get; set; }
        [Required]
        public int TransactionId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public Decimal Amount { get; set; }
        public DateTime Updated { get; set; }
    }
}