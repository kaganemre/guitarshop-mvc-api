using GuitarShopApp.Application.DTO;

namespace GuitarShopApp.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<AuthResultDTO> RegisterAsync(UserDTO model);
    Task<AuthResultDTO> LoginAsync(LoginDTO model);
}