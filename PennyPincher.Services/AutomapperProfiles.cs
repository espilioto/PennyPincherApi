using AutoMapper;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Categories.Models;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<Statement, StatementDto>().ReverseMap();
        CreateMap<Statement, StatementDtoV2>().ReverseMap();
        CreateMap<Statement, LegacyStatementDto>()
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description))
            .ReverseMap();
        CreateMap<Statement, StatementResponse>().ReverseMap();
        CreateMap<Statement, StatementRequest>().ReverseMap();
        CreateMap<StatementDtoV2, StatementResponse>().ReverseMap();
        CreateMap<StatementDto, StatementRequest>().ReverseMap();


        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryResponseLite>().ReverseMap();
        CreateMap<CategoryDto, CategoryResponseLite>().ReverseMap();

        CreateMap<Account, AccountResponse>().ReverseMap();
        CreateMap<Account, AccountRequest>().ReverseMap();
        CreateMap<Account, AccountResponseLite>().ReverseMap();
    }
}
