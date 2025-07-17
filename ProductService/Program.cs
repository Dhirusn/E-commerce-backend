using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductService.Data.Models;
using ProductService.Filters;
using ProductService.Middleware;
using ProductService.Services;
using SharedLibraries;
using SharedLibraries.UserServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-zn7kooyuqtsoiajl.us.auth0.com/";
    options.Audience = "https://product-api";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        RoleClaimType = "https://myapp.com/roles"
    };
});
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
// Add services to the container.
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireClaim("https://dev-zn7kooyuqtsoiajl.us.auth0.com/roles", "Admin"));
//});

//builder.Services.AddAuth0Authentication(builder.Configuration);


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // Optional: Add role-based filtering to Swagger UI
    options.OperationFilter<RoleBasedAuthorizationOperationFilter>();
});


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
