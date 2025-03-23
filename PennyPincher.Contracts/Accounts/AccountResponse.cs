namespace PennyPincher.Contracts.Accounts;

public record AccountResponse(
        int Id,
        string Name,
        decimal Balance
    );
