using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public Decimal Amount { get; set; }
        public bool Void { get; set; }

        public BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }

        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }

    }
}