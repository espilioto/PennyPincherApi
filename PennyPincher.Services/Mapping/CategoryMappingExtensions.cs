using PennyPincher.Contracts.Categories;
using PennyPincher.Domain.Models;

namespace PennyPincher.Services.Mapping;

public static class CategoryMappingExtensions
{
    public static Category ToEntity(this CategoryRequest request) =>
        new()
        {
            Name = request.Name
        };
}
