﻿using AutoMapper;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Categories.Models;
using PennyPincher.Services.Statements.Models;
using PennyPincher.Services.Accounts.Models;

namespace PennyPincher.Services
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Statement, StatementDto>().ReverseMap();
            CreateMap<Statement, LegacyStatementDto>().ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description)).ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Account, AccountDto>().ReverseMap();
        }
    }
}
