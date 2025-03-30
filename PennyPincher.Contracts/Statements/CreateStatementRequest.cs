namespace PennyPincher.Contracts.Statements;

public record CreateStatementRequest(
        DateTime Date,
        int AccountId,
        double Amount,
        string Notes,
        int CategoryId
    );
