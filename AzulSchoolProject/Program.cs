using AzulSchoolProject.Middleware;
using Data;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
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
});

// Add conection to the DataBase
builder.Services.AddDbContext<ProjectDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProjectServer")));

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

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
