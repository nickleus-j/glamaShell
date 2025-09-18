using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PresentGlama.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PresentGlama.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LlmController : Controller
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public LlmController(IHttpClientFactory factory, IOptions<JsonOptions> jsonOptions)
        {
            _client = factory.CreateClient("ollama");
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }
        [HttpGet]
        public async Task<List<string>> Names()
        {
            var response = await _client.GetAsync("/api/tags");
            if (!response.IsSuccessStatusCode)
            {
                return [""];
            }
            string s = response.Content.ReadAsStringAsync().Result;
            var result = await DeserializeOllamaModelsAsync(s);
            return result?.Models.Select(m => m.Name).ToList() ?? new List<string>();
        }
        async Task<OllamaModelList> DeserializeOllamaModelsAsync(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return await Task.Run(() =>
                JsonSerializer.Deserialize<OllamaModelList>(json, options)
                ?? new OllamaModelList()
            );
        }
    }
}
