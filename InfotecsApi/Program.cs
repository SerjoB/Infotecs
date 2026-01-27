using InfotecsApi.Data;
using InfotecsApi.Data.Repositories;
using InfotecsApi.Services;
using InfotecsApi.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



if (!builder.Environment.IsEnvironment("Testing"))  // We use SqlLite for tests
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

//  SERVICES

builder.Services.AddScoped<IResultCalculatorService, ResultCalculatorService>();
builder.Services.AddScoped<ICsvReaderService, CsvReaderService>();
builder.Services.AddScoped<ICsvDataRepository, CsvDataRepository>();
builder.Services.AddScoped<CsvImportService>();
builder.Services.AddScoped<CsvValidator>();
builder.Services.AddScoped<ValuesQueryService>();
builder.Services.AddScoped<ResultsQueryService>();

// Shows <summary> comments from code in the Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Customizing default model validation behavior.
// Instead of returning the standard validation response,
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var firstError = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();
            return new BadRequestObjectResult(new { error = firstError ?? "Invalid data" });
        };
    });
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Applies migrations and creates a database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}

app.Run();
public partial class Program { } 