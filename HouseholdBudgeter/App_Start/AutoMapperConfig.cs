using AutoMapper;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModels;
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
            }); 
        }
    }
}