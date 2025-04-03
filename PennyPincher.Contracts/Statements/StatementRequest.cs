namespace PennyPincher.Contracts.Statements;

public record StatementRequest(
        int? Id,
        DateTime Date,
        int AccountId,
        double Amount,
        string Description,
        int CategoryId,
        int UserId
    );
