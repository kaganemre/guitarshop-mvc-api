using System.ComponentModel.DataAnnotations;

namespace GuitarShopApp.WebUI.Models;

public class ResetPasswordModel
{
    [Required(ErrorMessage = "The 'Token' field is required.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "'Email' address is required.")]
    [EmailAddress(ErrorMessage = "A valid email is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "The 'Password' field is required.")]
    [StringLength(
        maximumLength: 10,
        MinimumLength = 3,
        ErrorMessage = "'Password' must be between 3 and 10 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "The 'ConfirmPassword' field is required.")]
    [Compare(nameof(Password), ErrorMessage = "The password does not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}