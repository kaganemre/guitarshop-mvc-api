using System.Text.Json;
using GuitarShopApp.Application.DTO;

namespace GuitarShopApp.WebUI.ApiService;

public sealed class ProductApiService : AuthenticatedApiService<ProductDTO>
{
    public ProductApiService(HttpClient httpClient) : base(httpClient) { }
    public async Task<IEnumerable<ProductDTO>> GetProductsByCategory(string category)
    {
        var response = await _httpClient.GetAsync($"get-by-category/{category}");
        string json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(json, _jsonOptions);

        return products ?? new List<ProductDTO>();
    }

    public async Task DeleteAsync(ProductDTO model) => await DeleteAsync(model.Id);
}