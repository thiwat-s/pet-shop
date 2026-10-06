using System.ComponentModel.DataAnnotations;

namespace PetShop.Api.Dtos;

public sealed class SavePetRequest
{
    [Required, StringLength(80)]
    public string Name { get; init; } = "";

    [Required, StringLength(50)]
    public string Species { get; init; } = "";

    [Required, StringLength(80)]
    public string Breed { get; init; } = "";

    [Range(0, 100)]
    public int Age { get; init; }

    [Range(typeof(decimal), "0", "1000000")]
    public decimal Price { get; init; }

    [Range(0, 100000)]
    public int Stock { get; init; }
}

public sealed record PetResponse(
    Guid Id,
    string Name,
    string Species,
    string Breed,
    int Age,
    decimal Price,
    int Stock,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
