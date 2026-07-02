using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GuitarShopApp.WebUI.Models;

public class LoginViewModel
{
    [ValidateNever]
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; } = true;
}