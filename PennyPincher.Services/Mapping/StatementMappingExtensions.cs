using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;
using PennyPincher.Domain.Models;

namespace PennyPincher.Services.Mapping;

public static class StatementMappingExtensions
{
    public static Statement ToEntity(this StatementRequest request) =>
        new()
        {
            Date = request.Date,
            AccountId = request.AccountId,
            Amount = request.Amount,
            Description = request.Description,
            CategoryId = request.CategoryId
        };

    public static IQueryable<StatementResponse> SelectAsResponse(this IQueryable<Statement> query) =>
        query.Select(s => new StatementResponse(
            s.Id,
            s.Date,
            s.Amount,
            s.Description,
            s.CheckedAt,
            new CategoryResponse(s.Category!.Id, s.Category.Name),
            new AccountResponseLite(s.Account!.Id, s.Account.Name)
        ));
}
