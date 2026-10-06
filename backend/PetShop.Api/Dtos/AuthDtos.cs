using System.ComponentModel.DataAnnotations;

namespace PetShop.Api.Dtos;

public sealed class LoginRequest
{
    [Required, StringLength(80)]
    public string Username { get; init; } = "";

    [Required, StringLength(200)]
    public string Password { get; init; } = "";
}

public sealed record LoginResponse(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAtUtc
);
