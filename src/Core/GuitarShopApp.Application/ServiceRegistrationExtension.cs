using GuitarShopApp.Application.Interfaces.Services;
using GuitarShopApp.Application.Mapping;
using GuitarShopApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarShopApp.Application;

public static class ServiceRegistrationExtension
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IProductService, ProductService>();
        serviceCollection.AddTransient<ICategoryService, CategoryService>();
        serviceCollection.AddTransient<IOrderService, OrderService>();
        serviceCollection.AddTransient<IUserService, UserService>();
        serviceCollection.AddAutoMapper(typeof(GeneralMapping).Assembly);
    }
}