using PennyPincher.Contracts.Accounts;
using PennyPincher.Domain.Models;

namespace PennyPincher.Services.Mapping;

public static class AccountMappingExtensions
{
    public static Account ToEntity(this AccountRequest request) =>
        new()
        {
            Name = request.Name,
            ColorHex = request.ColorHex
        };
}
