using CartService.Data;
using CartService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedLibraries.Extensions;
using SharedLibraries.UserServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Authentication & Authorization from shared library
builder.Services.AddJwtAuthentication(builder.Configuration);

// 2. Add Swagger with Authentication
builder.Services.AddSwaggerWithAuth("Cart Service");
// Add services to the container.
builder.Services.AddControllers();
// Program.cs (before builder.Build())
builder.Services.AddHttpContextAccessor();

// Add Entity Framework
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddScoped<CartService.Services.CartService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();

var redisConfig = new ConfigurationOptions
{
    EndPoints = { { "redis-12359.c282.east-us-mz.azure.redns.redis-cloud.com", 12359 } },
    User = "default",
    Password = "r1zzGkUrydRZ4uwVdsxE4nYcZryrfvya",
    Ssl = true,
    AbortOnConnectFail = false,
    ConnectTimeout = 10000
};

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));
// Add OpenAPI/Swagger
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
        c.OAuthClientId("swagger-client-id");
        c.OAuthAppName("Swagger");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
