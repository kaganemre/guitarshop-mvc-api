using GuitarShopApp.Application.DTO;
using GuitarShopApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IIdentityService identityService) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserDTO model)
    {
        var result = await _identityService.RegisterAsync(model);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(201, new { token = result.Token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var result = await _identityService.LoginAsync(model);

        if(!result.Succeeded)
        {
            return Unauthorized(result.Errors);
        }

        return Ok(new { token = result.Token });
    }
}