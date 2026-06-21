using GuitarShopApp.Application.DTO;

namespace GuitarShopApp.WebUI.ApiService;

public sealed class CategoryApiService : GenericApiService<CategoryDTO>
{
    public CategoryApiService(HttpClient httpClient) : base(httpClient) { }
}