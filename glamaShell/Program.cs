using Microsoft.AspNetCore.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages and Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Register HttpClient for Ollama on localhost:11434
builder.Services.AddHttpClient("ollama", client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Configure JSON to use camelCase
builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Map Razor Pages + API
app.MapRazorPages();
app.MapControllers();

app.Run();
