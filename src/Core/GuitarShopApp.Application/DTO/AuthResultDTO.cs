namespace GuitarShopApp.Application.DTO;

public class AuthResultDTO
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    public static AuthResultDTO Success(string token) =>
        new() { Succeeded = true, Token = token };

    public static AuthResultDTO Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };

    public static AuthResultDTO Failure(string error) =>
        new() { Succeeded = false, Errors = new[] { error } };
}