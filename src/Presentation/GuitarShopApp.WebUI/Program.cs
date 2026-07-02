using GuitarShopApp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using GuitarShopApp.WebUI.ApiService;
using GuitarShopApp.Application.Interfaces.Services;
using GuitarShopApp.Application.Mapping;
using GuitarShopApp.WebUI.Models;
using GuitarShopApp.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(GeneralMapping).Assembly);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartViewModel>(scs => SessionCartService.GetCart(scs));

var baseUri = "http://localhost:5191/api/";

if (!builder.Environment.IsDevelopment())
{
    baseUri = "http://core-webapi:5192/api/";
}

builder.Services.AddHttpClient<ProductApiService>(options =>
    options.BaseAddress = new Uri(baseUri + "products/")
);
builder.Services.AddHttpClient<CategoryApiService>(options =>
    options.BaseAddress = new Uri(baseUri + "categories/")
);
builder.Services.AddHttpClient<OrderApiService>(options =>
    options.BaseAddress = new Uri(baseUri + "orders")
);
builder.Services.AddHttpClient<UserApiService>(options =>
    options.BaseAddress = new Uri(baseUri + "shop/")
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute("products", "products/{category?}", new { controller = "Home", action = "List" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
