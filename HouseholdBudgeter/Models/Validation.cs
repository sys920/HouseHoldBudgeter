using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeter.Models
{
    public class Validation
    {
        private ApplicationDbContext DbContext { get; set; }
       
        public Validation()
        {
            DbContext = new ApplicationDbContext();           
        }  

        public bool IsOwnerOfHouseHold (int? id, string userId)
        {
            if (id == null || userId == null)
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
            if (id == null || userId == null)
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
        public bool IsHouseHoldExist(int? id)
        {

            if (id == null)
            {
                return false;
            }

            var result = DbContext.HouseHolds.Any(p => p.Id == id);
            if (result)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsCategoryExist(int? id, int? categoryId)
        {

            if (id == null || categoryId == null)
            {
                return false;
            }

            var result = DbContext.Categories.Any(p => p.Id == categoryId && p.HouseHoldId == id);
            if (result)
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