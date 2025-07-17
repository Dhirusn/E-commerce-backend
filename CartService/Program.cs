using Microsoft.EntityFrameworkCore;
using CartService.Data;
using CartService.Services;
using SharedLibraries.Extensions;
using SharedLibraries.UserServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Program.cs (before builder.Build())
builder.Services.AddHttpContextAccessor();
var tes = builder.Configuration.GetConnectionString("DefaultConnection");
// Add Entity Framework
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add services
builder.Services.AddScoped<CartService.Services.CartService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();

// Add Redis for caching
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("Redis");
//});
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
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
