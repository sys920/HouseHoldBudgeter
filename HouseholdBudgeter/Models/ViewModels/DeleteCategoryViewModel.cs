using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModels
{
    public class DeleteCategoryViewModel
    {
        [Required]
        public int CategoryId { get; set; }
    }
}