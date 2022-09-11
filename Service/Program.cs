using Ana.DataLayer;
using Ana.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Config
builder.Configuration.AddSystemsManager("/ana");
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("dev.config.json", true, true);
}

builder.Services.Configure<ServiceConfig>(builder.Configuration);

// DB
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("database"));
builder.Services.AddDbContext<AnaDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

// Middleware Pipeline
app.UseHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
