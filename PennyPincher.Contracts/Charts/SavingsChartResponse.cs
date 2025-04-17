namespace PennyPincher.Contracts.Charts;

public record SavingsChartResponse(
        List<GenericChartResponse> IncomeChart,
        List<GenericChartResponse> ExpensesChart,
        List<GenericChartResponse> SavingsChart
    );