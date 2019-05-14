using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class HouseHold
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desrcipton { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }

    }
}