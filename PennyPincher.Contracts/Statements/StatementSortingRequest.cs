namespace PennyPincher.Contracts.Statements;

public record StatementSortingRequest(
        string SortBy = "date",
        string Direction = "desc"
    );