using System.Security.Claims;
using AutoMapper;
using GuitarShopApp.Application.Interfaces.Services;
using GuitarShopApp.WebUI.ApiService;
using GuitarShopApp.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebUI.Controllers;

public class AccountController : Controller
{
    private readonly UserApiService _userApiService;
    private readonly IEmailService _emailService;

    public AccountController(
                            IEmailService emailService,
                            UserApiService userApiService)
    {
        _userApiService = userApiService;
        _emailService = emailService;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userApiService.Login(model.Email, model.Password);

            if (user.Email is not null)
            {
                if (!user.EmailConfirmed)
                {
                    TempData["message"] = "Click on the confirmation email sent to your account.";
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.FullName),
                    new("Email", user.Email),
                    new(ClaimTypes.Role, user.RoleName)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { RedirectUri = "/Home/Index", IsPersistent = model.RememberMe };

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
            }
            else
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
            }
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _userApiService.CreateUser(model);

            var token = Guid.NewGuid().ToString();
            var url = Url.Action("ConfirmEmail", "Account", new { model.Email, token });

            await _emailService.SendEmailAsync(model.Email, "Account Verification", $"<a href='http://localhost:5164{url}'>Please click on the link to confirm your email account</a>");

            TempData["message"] = "Click on the confirmation email sent to your account.";
            return RedirectToAction("Login", "Account");
        }

        return View(model);
    }

    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        if (email == null || token == null)
        {
            TempData["message"] = "Invalid verification key.";
            return View();
        }

        var user = await _userApiService.GetByEmail(email);

        if (user != null)
        {
            user.EmailConfirmed = true;
            await _userApiService.UpdateUser(user);

            TempData["message"] = "Your account has been confirmed.";
            return RedirectToAction("Login", "Account");
        }

        TempData["message"] = "User not found.";
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["message"] = "Please enter your email address.";
            return View();
        }

        var user = await _userApiService.GetByEmail(email);

        if (user == null)
        {
            TempData["message"] = "Your email address is not registered in our system";
            return View();
        }

        var token = Guid.NewGuid().ToString();
        var url = Url.Action("ResetPassword", "Account", new { user.Email, token });

        await _emailService.SendEmailAsync(user.Email, "Reset Password", $"<a href='http://localhost:5164{url}'>Click on the link to reset your password</a>");

        TempData["message"] = "You can reset your password with the link sent to your e-mail address";

        return View();
    }

    public IActionResult ResetPassword(string email, string token)
    {
        if (email == null || token == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordModel { Email = email, Token = token };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userApiService.GetByEmail(model.Email);
            if (user == null)
            {
                TempData["message"] = "There are no users matching this email address.";
                return RedirectToAction("Login");
            }

            try
            {
                user.Password = model.Password;
                await _userApiService.UpdateUser(user);
                TempData["message"] = "Your password has been changed.";
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
        }
        return View(model);
    }

}