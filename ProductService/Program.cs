using Microsoft.EntityFrameworkCore;
using ProductService.Data.Models;
using ProductService.Services;
using SharedLibraries.Extensions;
using SharedLibraries.UserServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// In Program.cs or Startup.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Remove this line if you're not using cookies/auth
    });
});

// 1. Add Authentication & Authorization from shared library
builder.Services.AddJwtAuthentication(builder.Configuration);

// 2. Add Swagger with Authentication
builder.Services.AddSwaggerWithAuth("Product Service");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    }); ;
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor(); // Needed for UserContext
builder.Services.AddScoped<IUserContextService, UserContextService>();

var redisConfig = new ConfigurationOptions
{
    EndPoints = { { "redis-12359.c282.east-us-mz.azure.redns.redis-cloud.com", 12359 } },
    User = "default",
    Password = "r1zzGkUrydRZ4uwVdsxE4nYcZryrfvya",
    Ssl = true,
    AbortOnConnectFail = false,
    ConnectTimeout= 10000
};

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));


builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddScoped<ProductsService>();

var app = builder.Build();
app.UseCors("AllowAngularDevClient");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
        c.OAuthClientId("swagger-client-id");
        c.OAuthAppName("Swagger");
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
