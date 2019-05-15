using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeter.Models
{
    public class CheckUser
    {
        private ApplicationDbContext DbContext { get; set; }
       
        public CheckUser()
        {
            DbContext = new ApplicationDbContext();           
        }  

       public bool IsOwnerOfHouseHold (int? id, string userId)
        {
            if (id == null)
            {
                return false;
            }
           
            var ownerOfHouseHold = DbContext.HouseHolds.Any(p => p.Id == id && p.OwnerId == userId);
            if (ownerOfHouseHold)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool IsMemberOfHouseHold (int? id, string userId)
        {
            if (id == null)
            {
                return false;
            }
          
            var memberOfHouseHold = DbContext.HouseHoldUsers.Any(p => p.HouserholdId == id && p.UserId == userId);
            if (memberOfHouseHold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}