namespace PennyPincher.Contracts.Categories;

public record CategoryRequest(
        int? Id,
        string Name,
        string userId
    );
