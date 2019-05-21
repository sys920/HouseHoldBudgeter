using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.Transaction
{
    public class TransactionBindingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }


    }
}