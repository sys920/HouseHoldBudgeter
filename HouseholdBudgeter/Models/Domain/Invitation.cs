using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class Invitation
    {
        public int Id { get; set; }
        public virtual HouseHold HouserHold { get; set; }
        public int HouseHoldId { get; set;  }
        public string UserEmail { get; set; }
    }
}