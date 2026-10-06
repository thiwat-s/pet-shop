using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetShop.Api.Configuration;
using PetShop.Api.Dtos;
using PetShop.Api.Repositories;

namespace PetShop.Api.Services;

public sealed class AuthService(
    IUserRepository users,
    PasswordHasher hasher,
    IOptions<JwtOptions> options
)
{
    private readonly JwtOptions _options = options.Value;

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await users.FindByUsernameAsync(request.Username.Trim());
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
            return null;
        var expires = DateTime.UtcNow.AddHours(_options.ExpiryHours);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256
        );
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            [new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role)],
            expires: expires,
            signingCredentials: credentials
        );
        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            user.Username,
            user.Role,
            expires
        );
    }
}
