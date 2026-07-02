using GuitarShopApp.Application.Interfaces.Repositories;
using GuitarShopApp.Application.Interfaces.Services;
using GuitarShopApp.Application.Interfaces.UnitOfWork;
using GuitarShopApp.Persistence.Repositories;
using GuitarShopApp.Persistence.Services;
using GuitarShopApp.Persistence.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace GuitarShopApp.Persistence;

public static class ServiceRegistrationExtension
{
    public static void AddPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration = null!)
    {
        
        serviceCollection.AddTransient<IProductRepository, ProductRepository>();
        serviceCollection.AddTransient<ICategoryRepository, CategoryRepository>();
        serviceCollection.AddTransient<IOrderRepository, OrderRepository>();
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddTransient<IIdentityService, IdentityService>();

    }
}

