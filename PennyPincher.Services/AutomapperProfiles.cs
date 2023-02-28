﻿using AutoMapper;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Categories.Models;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Statement, StatementDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}