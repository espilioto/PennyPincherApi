namespace PennyPincher.Contracts.Statements;

public record StatementFilterRequest(
        List<int>? AccountIdsIncluded,
        List<int>? CategoryIdsIncluded,
        List<int>? AccountIdsExcluded,
        List<int>? CategoryIdsExcluded,
        DateTime? DateFrom,
        DateTime? DateTo,
        decimal? MinAmount,
        decimal? MaxAmount,
        string? SearchText
    );
