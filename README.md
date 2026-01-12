# 🎸 GuitarShopApp – ASP.NET Core 8 Onion Architecture E-Commerce

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![Architecture](https://img.shields.io/badge/Onion-Architecture-blueviolet)
![License](https://img.shields.io/github/license/kaganemre/guitarshop-mvc-api)

🧱 ASP.NET Core Web API + MVC  

💼 A multi-layered .NET 8 e-commerce application featuring product management, user roles, and a complete payment flow.

This project is an **ASP.NET Core 8** application structured according to **Onion Architecture** principles.  
The Onion Architecture minimizes dependencies between application layers, allowing changes to be made with minimal cost and impact.

The Web API and MVC (UI) layers are kept separate, providing a multi-layered structure that supports CRUD operations through the API.  
The MVC project consumes the API and renders dynamic views based on the retrieved data.

Product listing, creation, updating, and deletion operations are performed based on user roles.  
Products can be added to the cart from the products page, and payments can be completed through the checkout flow.  
The payment process is integrated with **iyzico**.  
The project can be easily started using **Docker Compose**.

---

## 🚀 Technologies & Features

* 🧩 .NET 8
* 🧅 Onion Architecture (Domain / Application / Infrastructure / Web)
* 🗃️ Entity Framework Core (PostgreSQL)
* 🧰 Repository & Unit of Work Pattern
* 🔐 Identity (Cookie + JWT-based authentication)
* 🧾 Authorization
* 🔁 AutoMapper
* ✅ FluentValidation
* ⚙️ Hangfire
* 📜 Swagger
* 🔄 CORS configuration
* 💻 Bootstrap 5 + AJAX UI integration
* 🐳 Dockerfile (WebAPI / WebUI) and `docker-compose.yml`

📁 **Main files:** `GuitarShopApp.sln`, `docker-compose.yml`, `Dockerfile-WebAPI`, `Dockerfile-WebUI`

---

## 🧱 Architecture & Project Structure

The project is structured according to **Onion Architecture**:

* 🧩 **Domain** → Entities.
* 🧠 **Application** → Use cases, DTOs, service interfaces, validation, AutoMapper profiles.
* 💾 **Infrastructure** → EF Core implementations, Repository, Unit of Work, Identity, Hangfire.
* 🌍 **WebAPI / WebUI** → Controllers, Middleware, Swagger, Program.cs configuration.

> 💡 By inverting dependencies between layers (Dependency Inversion), **loose coupling** is achieved.

---

## 🌐 Web API

### Retrieving specific fields from the database in the `GetProducts()` method

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts() // Fetching selected fields via DTOs
    {
        return Ok(_mapper.Map<IEnumerable<ProductDTO>>(await _productService.GetAll()));
    }
    // Other action methods are defined here
}
```
---

## 🖥️ Web UI (MVC)

### Fetching all products from the API using HttpClient

```csharp
public class ProductApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProductDTO>> GetAll()
    {
        var response = await _httpClient.GetAsync("");
        string apiResponse = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(apiResponse, options);

        return products ?? [];
    }
    // Other service methods are defined here
}
```
### Returning the product list received from the API as a view

```csharp
[Authorize(Roles = "admin")]
public class HomeController : Controller
{
    private readonly ProductApiService _productApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly IMapper _mapper;

    public HomeController(
        ProductApiService productApiService,
        CategoryApiService categoryApiService,
        IMapper mapper)
    {
        _productApiService = productApiService;
        _categoryApiService = categoryApiService;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<IActionResult> List(string category)
    {
        ViewBag.Categories = await _categoryApiService.GetAll();
        ViewBag.SelectedCategory = RouteData?.Values["category"];

        return View(await _productApiService.GetProductsByCategory(category));
    }
    // Other action methods are defined here
}
```

### 🖼️ Products Page

#### Products can be listed based on category
<img width="1637" height="982" alt="image" src="https://github.com/user-attachments/assets/7dab3046-10ae-42d2-b6bb-426e5a8eb8b0" />

---

## 📦 Getting Started

### Clone the repository

```bash
$ git clone https://github.com/kaganemre/guitarshop-mvc-api.git
```
### Web API Project
Before running the project, an empty PostgreSQL database named HangfireDB must be created.
After starting the project using dotnet run, migrations will be applied automatically to the database.

### MVC Project
```bash
$ dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

After installing the LibMan package manager, restore the libraries using:

```bash
$ libman restore
```

---

## 📄 License

This project is licensed under the **MIT License**.  
See the [LICENSE](LICENSE) file for details.
