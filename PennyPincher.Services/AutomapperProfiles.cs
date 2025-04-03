using AutoMapper;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;
using PennyPincher.Domain.Models;

namespace PennyPincher.Services;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<Statement, StatementRequest>().ReverseMap();
        CreateMap<Statement, StatementResponse>().ReverseMap();

        CreateMap<Category, CategoryRequest>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();

        CreateMap<Account, AccountResponse>().ReverseMap();
        CreateMap<Account, AccountRequest>().ReverseMap();
        CreateMap<Account, AccountResponseLite>().ReverseMap();
    }
}
