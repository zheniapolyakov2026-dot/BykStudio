using System.Text;
using BykStudio.data;
using BykStudio.data.Interfaces;
using BykStudio.data.Models;
using BykStudio.data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Получение строки подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing in appsettings.json / appsettings.Development.json");
}

// Добавление DbContext с провайдером PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity (используем наши модели и контекст)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;           // or 6 if you prefer
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 1;      // at least one unique character

    // Email confirmation settings (if you keep it)
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Настройка JWT (ключ, издатель и т.д. – добавим в appsettings.json)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ITinkoffPaymentService, TinkoffPaymentService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Development-only endpoints
if (app.Environment.IsDevelopment())
{
    app.MapPost("/dev/clear-users", async (ApplicationDbContext db, ILogger<Program> logger) =>
    {
        try
        {
            logger.LogInformation("Clearing users...");

            // Load all users (and optionally roles) into memory
            var users = await db.Users.ToListAsync();
            int userCount = users.Count;

            // Remove users – EF will cascade delete related Bookings and Payments
            db.Users.RemoveRange(users);
            await db.SaveChangesAsync();

            logger.LogInformation("Deleted {Count} users.", userCount);
            return Results.Ok(new { message = "Users cleared.", deleted = userCount });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error clearing users");
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }
    });
}

app.MapGet("/", () => Results.Ok(new { message = "BykStudio API is running", time = DateTime.UtcNow }));

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"===== GLOBAL EXCEPTION =====");
        Console.WriteLine(ex);
        // Return a 500 so we see something in the client
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            error = ex.Message,
            stackTrace = ex.StackTrace
        }));
    }
});

app.Run();
