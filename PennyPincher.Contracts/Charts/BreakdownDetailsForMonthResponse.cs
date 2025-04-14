using PennyPincher.Contracts.Statements;

namespace PennyPincher.Contracts.Charts;

public record BreakdownDetailsForMonthResponse(
        string Title,
        List<BreakdownDetailsForMonthDonutData> DonutData,
        List<StatementResponse> IncomeStatements,
        List<StatementResponse> ExpenseStatements,
        decimal TotalIncome,
        decimal TotalExpenses,
        decimal Balance
    );
