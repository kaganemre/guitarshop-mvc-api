using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using GuitarShopApp.Application;
using GuitarShopApp.Application.Features.Commands;
using GuitarShopApp.Infrastructure;
using GuitarShopApp.Persistence;
using GuitarShopApp.Persistence.Background;
using GuitarShopApp.Persistence.Context;
using GuitarShopApp.WebAPI.Attributes;
using GuitarShopApp.WebAPI.Middlewares;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5191");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5164", "http://localhost:5167", "https://localhost:7259")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShopAppDbContext>(options =>
        options.UseNpgsql(conn));
        builder.Services.AddDbContext<IdentityContext>(options =>
        options.UseNpgsql(conn));

builder.Services.AddHangfire(x => 
{
    x.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(builder.Configuration["ConnectionStrings:HangfireConnection"]));
    if(5 == 5)
    RecurringJob.AddOrUpdate<Job>("SendNewProductsMailForUsers", y => y.SendNewProductsMailForUsers(), "00 20 * * *");
});
builder.Services.AddHangfireServer();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginUserValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddInfrastructureServices(builder.Configuration);


builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration.GetSection("AppSettings:Secret").Value ?? "")),
        ValidateLifetime = true
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddTransient<LoggerMiddleware>();
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<LoggerMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new [] { new MyAuthorizationFilter() }
});

app.UseExceptionHandler();


using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<ShopAppDbContext>();
if(dbContext.Database.GetPendingMigrations().Any())
await dbContext.Database.MigrateAsync();


IdentitySeedData.IdentityTestUser(app.Services);

app.Run();