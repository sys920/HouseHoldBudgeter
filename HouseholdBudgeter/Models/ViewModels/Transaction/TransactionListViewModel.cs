﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels.Transaction
{
    public class TransactionListViewModel
    {       
        public int TransactionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public Decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}