namespace PennyPincher.Contracts.Charts;

public record MonthlyBreakdownResponse(
        string MonthYear,
        int Month,
        int Year,
        decimal Income,
        decimal Expenses,
        decimal Balance
    );
