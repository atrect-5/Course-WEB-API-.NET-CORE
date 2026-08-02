using AzulSchoolProject.Middleware;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using System.Text;
using AzulSchoolProject.Settings;

var builder = WebApplication.CreateBuilder(args);

// Bind cookie settings from configuration (optional) or use defaults
var cookieSettings = builder.Configuration.GetSection("Cookies").Get<CookieSettings?>() ?? new CookieSettings();
builder.Services.AddSingleton(cookieSettings);

// Add services to the container. 
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationRepository>();
builder.Services.AddScoped<IUserService, UserRepository>();
builder.Services.AddScoped<ICategoryService, CategoryRepository>();
builder.Services.AddScoped<IMoneyAccountService, MoneyAccountRepository>();
builder.Services.AddScoped<ITransactionService, TransactionRepository>();
builder.Services.AddScoped<ITransferService, TransferRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(cookieSettings.AllowedFrontendOrigin)
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register antiforgery for SPA double-submit (header + cookie)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = cookieSettings.XsrfCookieName;
    options.Cookie.HttpOnly = cookieSettings.XsrfHttpOnly;
    options.Cookie.SameSite = cookieSettings.XsrfSameSite;
    options.Cookie.SecurePolicy = cookieSettings.XsrfSecure ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configura Swagger para que use el archivo XML generado por el proyecto.
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // Esquema ApiKey para XSRF para que Swagger permita introducir manualmente el token.
    options.AddSecurityDefinition("XSRF", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-CSRF-TOKEN",
        Description = "Ingrese manualmente el token XSRF (double-submit)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "XSRF"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add conection to the DataBase
string connectionString = builder.Configuration.GetConnectionString("ProjectServer") ?? "";

// Check if DATABASE_URL is set (Heroku/Supabase environment).  If not, try Supabase settings.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")))
{    
    string? supabaseHost = Environment.GetEnvironmentVariable("SUPABASE_DB_HOST");
    string? supabasePort = Environment.GetEnvironmentVariable("SUPABASE_DB_PORT");
    string? supabaseDatabase = Environment.GetEnvironmentVariable("SUPABASE_DB_NAME");
    string? supabaseUser = Environment.GetEnvironmentVariable("SUPABASE_DB_USER");
    string? supabasePassword = Environment.GetEnvironmentVariable("SUPABASE_DB_PASSWORD");

    if (!string.IsNullOrEmpty(supabaseHost) && !string.IsNullOrEmpty(supabaseDatabase) && !string.IsNullOrEmpty(supabaseUser) && !string.IsNullOrEmpty(supabasePassword))
    {
        connectionString = $"Host={supabaseHost};Port={supabasePort ?? "5432"};Database={supabaseDatabase};Username={supabaseUser};Password={supabasePassword};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
    }
    else
    {
        // If no Supabase environment variables are set, try to get the connection string from the configuration (for local development).
        connectionString = builder.Configuration.GetConnectionString("ProjectServer") ?? ""; // Provide a default if all else fails.
    }
} else {
     // Heroku / Supabase with DATABASE_URL
    string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")!;
    databaseUrl = databaseUrl.Replace("postgres://", "postgresql://"); // Ensure correct scheme for Npgsql
    var databaseUri = new Uri(databaseUrl);
    string[] userInfo = databaseUri.UserInfo.Split(':');
     connectionString =  new Npgsql.NpgsqlConnectionStringBuilder {
         Host = databaseUri.Host,
         Database = databaseUri.LocalPath.TrimStart('/'),
         Username = userInfo[0],
         Password = userInfo[1],
         Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
         SslMode = Npgsql.SslMode.Require
    }.ToString();
} 

builder.Services.AddDbContext<ProjectDBContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.CommandTimeout(90)));

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true,ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check if the token is present in the cookie
            if (context.Request.Cookies.TryGetValue(cookieSettings.AuthCookieName, out var token) && !string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// register CSRF middleware BEFORE authentication so invalid requests are rejected early
app.UseMiddleware<CsrfMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzulSchoolProject API V1");
     c.HeadContent = """
        <script>
    (function() {
      // Ensure swagger UI sends cookies (HttpOnly AuthToken) with requests, but DO NOT auto-inject the XSRF cookie into headers.
      const originalFetch = window.fetch;
      window.fetch = function(input, init = {}) {
        init.credentials = init.credentials || 'include';
        return originalFetch(input, init);
      };

      const origOpen = XMLHttpRequest.prototype.open;
      const origSend = XMLHttpRequest.prototype.send;
      XMLHttpRequest.prototype.open = function() {
        this.withCredentials = true;
        return origOpen.apply(this, arguments);
      };
      XMLHttpRequest.prototype.send = function(body) {
        return origSend.apply(this, arguments);
      };
    })();
    </script>
    """;
     
});

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Ruta principal para mostrar un mensaje de bienvenida en HTML con un enlace a Swagger.
app.MapGet("/", () => Results.Content("""
    <html>
        <head>
            <title>Azul School Project API</title>
            <style>
                body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; flex-direction: column; background-color: #f8f9fa; color: #343a40; }
                h1 { font-weight: 300; }
                a { color: #007bff; text-decoration: none; }
                a:hover { text-decoration: underline; }
            </style>
        </head>
        <body>
            <h1>Welcome to the app</h1>
            <p>Go to the <a href="/swagger">API Documentation</a>.</p>
        </body>
    </html>
    """, "text/html")).ExcludeFromDescription();

app.MapControllers();

app.Run();
