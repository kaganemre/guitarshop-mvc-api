using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarShopApp.Persistence.Services;

public static class HttpClientExtensions
{
    private static Dictionary<string, string> accessToken = new();
    private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
    public static async Task AddTokenToHeader(this HttpClient httpClient)
    {
        var _accessor = new HttpContextAccessor();
        var env = _accessor.HttpContext?.RequestServices.GetRequiredService<IWebHostEnvironment>();
        
        var baseUri = string.Empty;
        if(env?.EnvironmentName == "Development")
            baseUri="http://localhost:5191/api";
        else
            baseUri="http://core-webapi:5192/api";

        Dictionary<string, string> loginValues = new()
        {
            ["email"] = "info@adminuser.com",
            ["password"] = "Password_536"
        };

        var loginContent = new StringContent(JsonSerializer.Serialize(loginValues), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(baseUri + "/users/login", loginContent);
        var token = await response.Content.ReadAsStringAsync();
        accessToken = JsonSerializer.Deserialize<Dictionary<string, string>>(token, options) ?? new();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken["token"]);
    }
}