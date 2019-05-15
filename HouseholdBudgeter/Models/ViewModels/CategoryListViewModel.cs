using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels
{
    public class CategoryListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
 
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }


    }
}