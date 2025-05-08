using PennyPincher.Contracts.Statements;

namespace PennyPincher.Contracts.Charts;

public record BreakdownDetailsResponse(
        string Title,
        List<GenericKeyValueResponse> DonutData,
        List<StatementResponse> IncomeStatements,
        List<StatementResponse> ExpenseStatements,
        decimal TotalIncome,
        decimal TotalExpenses,
        decimal Balance
    );
