using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;

namespace PennyPincher.Contracts.Statements;

public record StatementResponse(
        int Id,
        DateTime Date,
        decimal Amount,
        string Description,
        DateTime? CheckedAt,
        CategoryResponse Category,
        AccountResponseLite Account
    );
