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
}
