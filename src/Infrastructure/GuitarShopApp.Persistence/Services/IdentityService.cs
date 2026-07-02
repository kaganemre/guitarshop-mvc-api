using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GuitarShopApp.Application.DTO;
using GuitarShopApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GuitarShopApp.Persistence.Services;

public class IdentityService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IConfiguration configuration)
    : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly IConfiguration _configuration = configuration;


    public async Task<AuthResultDTO> RegisterAsync(UserDTO model)
    {
        var user = new IdentityUser
        {
            UserName = model.FullName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return AuthResultDTO.Failure(result.Errors.Select(e => e.Description));
        }

        var token = GenerateJwt(user);

        return AuthResultDTO.Success(token);
    }
    
    public async Task<AuthResultDTO> LoginAsync(LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            return AuthResultDTO.Failure("Email is incorrect");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (!result.Succeeded)
        {
            return AuthResultDTO.Failure("Email or password is incorrect");
        }

        var token = GenerateJwt(user);
        return AuthResultDTO.Success(token);
    }
    private string GenerateJwt(IdentityUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
}