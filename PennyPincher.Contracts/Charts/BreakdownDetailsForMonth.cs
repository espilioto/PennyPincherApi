using PennyPincher.Contracts.Statements;

namespace PennyPincher.Contracts.Charts;

public record BreakdownDetailsForMonth(
        DonutData DonutData,
        List<StatementResponse> Income,
        List<StatementResponse> Expenses,
        double Balance
    );

public record DonutData(
        double Value,
        string Title
    );