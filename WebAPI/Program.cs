using Application.Interfaces;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection("Jwt");
string jwtSecret = jwtSection.GetValue<string>("Key")
    ?? throw new InvalidOperationException("Jwt:Key is not configured");
string jwtIssuer = jwtSection.GetValue<string>("Issuer")
    ?? throw new InvalidOperationException("Jwt:Issuer is not configured");
string jwtAudience = jwtSection.GetValue<string>("Audience")
    ?? throw new InvalidOperationException("Jwt:Audience is not configured");
int jwtExpireMinutes = jwtSection.GetValue<int>("ExpireMinutes");

if (jwtSecret.Length < 32)
{
    throw new InvalidOperationException($"Jwt:Key must be at least 32 characters. Current length: {jwtSecret.Length}");
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ===== REPOSITORIES =====
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// ===== SERVICES =====
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<UserService>(provider =>
{
    var userRepo = provider.GetRequiredService<IUserRepository>();
    var roleRepo = provider.GetRequiredService<IRoleRepository>();
    return new UserService(userRepo, roleRepo, jwtSecret, jwtExpireMinutes);
});

builder.Services.AddAutoMapper(cfg => { }, typeof(Application.Mapping.AutoMapperProfile).Assembly);

var key = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero 
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:5175")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas exitosamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error aplicando migraciones: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce API V1");
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();