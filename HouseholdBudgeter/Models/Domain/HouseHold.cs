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
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }

        public virtual List<Invitation> Invitations { get; set; }
      
        public virtual List<HouseHoldUser> HouseHoldUsers { get; set; }

        public virtual List<Category> Categories { get; set; }

        public virtual List<BankAccount> BankAccounts { get; set; }
        public HouseHold ()
        {
            Invitations = new List<Invitation>();
            HouseHoldUsers = new List<HouseHoldUser>();
            Categories = new List<Category>();
            BankAccounts = new List<BankAccount>();
        }        
    }
}