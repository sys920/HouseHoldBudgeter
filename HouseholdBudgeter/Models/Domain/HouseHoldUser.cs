using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class HouseHoldUser
    {
        public int Id { get; set; }
        public virtual HouseHold HouseHold {get; set;}
        public int HouseHoldId { get; set;}

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
       
    }
}