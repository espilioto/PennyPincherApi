namespace PennyPincher.Contracts.Accounts;

public record AccountRequest(
        string Name,
        string userId,
        string ColorHex
    );

