namespace PennyPincher.Contracts.Statements;

public record CreateStatementRequest(
        DateTime Date,
        int AccountId,
        double Amount,
        string Description,
        int CategoryId,
        int UserId
    );
