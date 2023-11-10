using Amazon.DynamoDBv2;
using Ana.DataLayer.Repositories;
using Ana.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Config
builder.Configuration.AddSystemsManager("/ana");

builder.Services.Configure<ServiceConfig>(builder.Configuration);

// DB
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddTransient<IUserRepo, UserRepo>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// Lambda Hosting
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

WebApplication app = builder.Build();
app.UsePathBase("/ana");

// Middleware Pipeline
app.UseCors(b => b.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
app.UseHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.Run();
