namespace PennyPincher.Contracts.Accounts;

public record AccountRequest(
        string Name,
        string UserId,
        string ColorHex
    );

