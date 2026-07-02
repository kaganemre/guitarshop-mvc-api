using System.Text.Json;
using GuitarShopApp.Application.DTO;
using GuitarShopApp.WebUI.Models;

namespace GuitarShopApp.WebUI.ApiService;

public sealed class ProductApiService : AuthenticatedApiService<ProductViewModel>
{
    public ProductApiService(HttpClient httpClient) : base(httpClient) { }
    public async Task<IEnumerable<ProductViewModel>> GetProductsByCategory(string category)
    {
        var response = await _httpClient.GetAsync($"get-by-category/{category}");
        string json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<IEnumerable<ProductViewModel>>(json, _jsonOptions);

        return products ?? new List<ProductViewModel>();
    }

    public async Task DeleteAsync(ProductViewModel model) => await DeleteAsync(model.Id);
}