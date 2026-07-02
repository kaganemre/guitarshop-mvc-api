using GuitarShopApp.Persistence.Services;

namespace GuitarShopApp.WebUI.ApiService;

public abstract class AuthenticatedApiService<T> : GenericApiService<T> where T : class
{
    protected AuthenticatedApiService(HttpClient httpClient) : base(httpClient) { }

    public virtual async Task CreateAsync(T model)
    {
        await _httpClient.AddTokenToHeader();
        await _httpClient.PostAsync("", Serialize(model));
    }

    public virtual async Task UpdateAsync(T model)
    {
        await _httpClient.AddTokenToHeader();
        await _httpClient.PutAsync("", Serialize(model));
    }

    public virtual async Task DeleteAsync(int id)
    {
        await _httpClient.AddTokenToHeader();
        await _httpClient.DeleteAsync($"{id}");
    }
}