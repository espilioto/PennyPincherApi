namespace PennyPincher.Contracts.Charts;

public record SavingsChartYearlyAmountsResponse(
        string Year,
        decimal IncomeAmount,
        decimal ExpensesAmount,
        decimal SavingsAmount,
        decimal SavingsPercent
    );