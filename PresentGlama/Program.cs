
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using PresentGlama;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOptions<GlammaOptions>().Bind(builder.Configuration.GetSection("GlammaOptions")).ValidateDataAnnotations().ValidateOnStart();
var glammaOptions = new GlammaOptions();
builder.Configuration.GetSection("GlammaOptions").Bind(glammaOptions);
builder.Services.AddHttpClient("ollama", client =>
{
    client.BaseAddress = new Uri(glammaOptions.OllamaAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddControllersWithViews();
// Configure JSON to use camelCase
builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
var CultureEn = new CultureInfo("en");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages + API
app.MapControllers();

app.Run();
