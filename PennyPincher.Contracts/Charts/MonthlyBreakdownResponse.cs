namespace PennyPincher.Contracts.Charts;

public record MonthlyBreakdownResponse(
        string MonthYear,
        decimal Income,
        decimal Expenses,
        decimal Balance
    );
