namespace PennyPincher.Contracts.Charts;

public record SavingsChartResponse(
        List<SavingsChartYearlyAmountsResponse> AveragesPerYear,
        List<GenericKeyValueResponse> IncomeChart,
        List<GenericKeyValueResponse> ExpensesChart,
        List<GenericKeyValueResponse> SavingsChart
    );