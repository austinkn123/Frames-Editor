using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DI SETUP FOR CLEAN ARCHITECTURE
// DI container is part of the .NET runtime infrastructure, and your provided files configure and register services into this container.
// The actual DI container is built and managed by the framework based on these configurations

//builder.Configuration = A collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.
var settings = new Settings(builder.Configuration);

DiConfiguration.ConfigureServices(builder.Services, settings);

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
//