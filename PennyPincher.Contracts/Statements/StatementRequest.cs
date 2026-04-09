namespace PennyPincher.Contracts.Statements;

public record StatementRequest(
        DateTime Date,
        int AccountId,
        decimal Amount,
        string Description,
        int CategoryId
    );
