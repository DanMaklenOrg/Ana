using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Ana.DataLayer;
using Ana.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Config
builder.Configuration.AddSystemsManager("/ana");

builder.Services.Configure<ServiceConfig>(builder.Configuration);

// DB
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddTransient<IUserRepo, UserRepo>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

// Middleware Pipeline
app.UseCors(b => b.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
app.UseHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
