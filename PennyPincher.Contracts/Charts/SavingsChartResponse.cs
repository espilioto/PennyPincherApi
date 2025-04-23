namespace PennyPincher.Contracts.Charts;

public record SavingsChartResponse(
        List<SavingsChartYearlyAmountsResponse> AveragesPerYear,
        List<GenericChartResponse> IncomeChart,
        List<GenericChartResponse> ExpensesChart,
        List<GenericChartResponse> SavingsChart
    );