using System.Text;
using System.Text.Json;
using GuitarShopApp.Application.DTO;
using GuitarShopApp.Persistence.Services;
using GuitarShopApp.WebUI.Models;

namespace GuitarShopApp.WebUI.ApiService;

public class OrderApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
    public OrderApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<OrderModel> CreateAsync(OrderModel model, string userId, List<CartItemModel> cartItems)
    {
        // Token information is added to the header with this extension method.
        await _httpClient.AddTokenToHeader();
        
        var payload = new
        {
            name = model.Name,
            phone = model.Phone,
            email = model.Email,
            addressLine = model.AddressLine,
            city = model.City,
            userId = userId,
            orderDate = DateTime.UtcNow,
            orderItems = cartItems.Select(i => new
            {
                productId = i.Product.Id,
                price = i.Product.Price,
                quantity = i.Quantity
            })
        };
        
        var createContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("", createContent);
        string apiResponse = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<OrderModel>(apiResponse, options);

        return order ?? new OrderModel();
    }
}