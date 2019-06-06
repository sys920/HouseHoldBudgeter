using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.Transaction
{
    public class TransactionViewModel
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
        public DateTime Date { get; set; }      
        public int CategoryId { get; set; }      
        public Decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public int BankAccountId { get; set; }
    }
}