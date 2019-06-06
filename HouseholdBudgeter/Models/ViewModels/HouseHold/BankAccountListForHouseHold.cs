using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.HouseHold
{
    public class BankAccountListForHouseHold
    {
        public int BankAccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public Decimal Balance { get; set; }

        public List<SumOfCategory> SumOfCategories { get; set; }
    }
}