namespace PennyPincher.Contracts.Charts;

public record YearlyBreakdownResponse(
        int Year,
        decimal Income,
        decimal Expenses,
        decimal Balance
    );
