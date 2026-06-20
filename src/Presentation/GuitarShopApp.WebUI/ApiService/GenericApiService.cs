using System.Text;
using System.Text.Json;

namespace GuitarShopApp.WebUI.ApiService;

public abstract class GenericApiService<T> where T : class
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    protected GenericApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public virtual async Task<IEnumerable<T>> GetAll()
    {
        var response = await _httpClient.GetAsync("");
        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<IEnumerable<T>>(json, _jsonOptions) ?? [];
    }

    public virtual async Task<T?> GetById(int? id)
    {
        var response = await _httpClient.GetAsync($"{id}");
        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    protected StringContent Serialize(T model) =>
        new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
}