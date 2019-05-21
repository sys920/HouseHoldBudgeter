using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class AccountBalance
    {
        public int Id { get; set; }
        public BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }
        public Decimal Balance { get; set; }
    }
}