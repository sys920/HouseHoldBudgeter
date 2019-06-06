using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.BankAccount
{
    public class BankAccountViewModel
    {
        public int HouseHoldId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
      
        public string Description { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public Decimal Balance { get; set; }
        
        public bool IsOwner { get; set; }

        public int NumberOfTransaction { get; set; }
    }
}