using AutoMapper;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels;
using HouseholdBudgeter.Models.ViewModels.BankAccount;
using HouseholdBudgeter.Models.ViewModels.HouseHold;
using HouseholdBudgeter.Models.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Init()
        {
            Mapper.Initialize(p =>
            {
                p.CreateMap<HouseHold, HouseholdBindingModel>().ReverseMap();
                p.CreateMap<HouseHold, HouseHoldViewModel>().ReverseMap();
                p.CreateMap<HouseHold, HouseHoldDetailViewModel>().ReverseMap();
                p.CreateMap<Category, CategoryViewModel>().ReverseMap();
                p.CreateMap<Category, CategoryBindingModel>().ReverseMap();
                p.CreateMap<Category, CategoryUpdateBidingModel>().ReverseMap();
                p.CreateMap<Transaction, TransactionBindingModel>().ReverseMap();
                p.CreateMap<Transaction, TransactionViewModel>().ReverseMap();
                p.CreateMap<Transaction, TransactionUpdateBindingModel>().ReverseMap();
                p.CreateMap<BankAccount, BankAccountBindigModel>().ReverseMap();
                p.CreateMap<BankAccount, BankAccountViewModel>().ReverseMap();
                p.CreateMap<BankAccount, BankAccountUpdateBindigModel>().ReverseMap(); 
            }); 
        }
    }
}