using Microsoft.AspNetCore.Mvc;
using PetShop.Api.Dtos;
using PetShop.Api.Services;

namespace PetShop.Api.Controllers;

[ApiController, Route("api/auth")]
public sealed class AuthController(AuthService service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var response = await service.LoginAsync(request);
        return response is null
            ? Unauthorized(new { message = "Invalid username or password." })
            : Ok(response);
    }
}
