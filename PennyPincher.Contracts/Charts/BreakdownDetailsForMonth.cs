using PennyPincher.Contracts.Statements;

namespace PennyPincher.Contracts.Charts;

public record BreakdownDetailsForMonth(
        List<BreakdownDetailsForMonthDonutData> DonutData,
        List<StatementResponse> IncomeStatements,
        List<StatementResponse> ExpenseStatements,
        decimal Balance
    );
