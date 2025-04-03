namespace PennyPincher.Contracts.Accounts;

public record AccountRequest(
        int? Id,
        string Name,
        string userId,
        string ColorHex
    );

