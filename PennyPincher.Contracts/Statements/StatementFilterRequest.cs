namespace PennyPincher.Contracts.Statements;

public record StatementFilterRequest(
        List<int>? AccountIds,
        List<int>? CategoryIds,
        DateTime? DateFrom,
        DateTime? DateTo,
        decimal? MinAmount,
        decimal? MaxAmount,
        string? SearchText
    );
