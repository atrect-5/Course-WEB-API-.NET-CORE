using AzulSchoolProject.Middleware;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationRepository>();
builder.Services.AddScoped<IUserService, UserRepository>();
builder.Services.AddScoped<ICategoryService, CategoryRepository>();
builder.Services.AddScoped<IMoneyAccountService, MoneyAccountRepository>();
builder.Services.AddScoped<ITransactionService, TransactionRepository>();
builder.Services.AddScoped<ITransferService, TransferRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configura Swagger para que use el archivo XML generado por el proyecto.
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // Configuracion para que swagger pueda usar autenticacion JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, introduce un token valido",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
        Host = databaseUri.Host, Database = databaseUri.LocalPath.TrimStart('/'), Username = userInfo[0], Password = userInfo[1], Port = databaseUri.Port > 0 ? databaseUri.Port : 5432, SslMode = Npgsql.SslMode.Require, TrustServerCertificate = true
    }.ToString();
} 

builder.Services.AddDbContext<ProjectDBContext>(options =>
    options.UseNpgsql(connectionString));

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
});

var app = builder.Build();

// Aplicar migraciones pendientes al iniciar la aplicación. (Asi heroku hara la migracion al hacer el despliegue)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ProjectDBContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        // En un escenario real, podrías querer manejar este error de forma más robusta.
    }
}

app.UseSwagger();
app.UseSwaggerUI();

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
