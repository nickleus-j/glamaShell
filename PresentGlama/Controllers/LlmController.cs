using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PresentGlama.Models;
using System.Text.Json;

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

            var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<OllamaModelList>(stream);

            return result?.Models.Select(m => m.Name).ToList() ?? new List<string>();
        }
    }
}
