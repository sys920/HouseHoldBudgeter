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
          
            var memberOfHouseHold = DbContext.HouseHoldUsers.Any(p => p.HouseHoldId == id && p.UserId == userId);
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
        public bool IsBankAccountExist(int? id, int? bankAccountId)
        {

            if (id == null || bankAccountId == null)
            {
                return false;
            }

            var result = DbContext.BankAccounts.Any(p => p.Id == bankAccountId && p.HouseHoldId == id);
            if (result)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsTransactionExist(int? bankAccountId, int? transactionId)
        {

            if (bankAccountId == null || transactionId == null)
            {
                return false;
            }

            var result = DbContext.Transactions.Any(p => p.Id == transactionId && p.BankAccountId == bankAccountId);
            if (result)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsOwnerOfTransaction(int? transactionId, string userId)
        {
            if (transactionId == null || userId == null)
            {
                return false;
            }

            var ownerOfTransaction = DbContext.Transactions.Any(p => p.Id == transactionId && p.OwnerId == userId);
            if (ownerOfTransaction)
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