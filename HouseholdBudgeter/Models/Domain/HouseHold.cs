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

        public virtual List<Invitation> Invitations { get; set; }
      
        public virtual List<ApplicationUser> HouseHoldUsers { get; set; }

        public virtual List<Category> Categories { get; set; }
        public HouseHold ()
        {
            Invitations = new List<Invitation>();
            HouseHoldUsers = new List<ApplicationUser>();
            Categories = new List<Category>();
        }        
    }
}