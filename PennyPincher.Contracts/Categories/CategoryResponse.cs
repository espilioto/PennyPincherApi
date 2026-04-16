namespace PennyPincher.Contracts.Categories;

public record CategoryResponse(
        int Id,
        string Name,
        int SortOrder = 0
    );
