using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PresentGlama.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
namespace PresentGlama.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatController(IHttpClientFactory factory, IOptions<JsonOptions> jsonOptions)
        {
            _client = factory.CreateClient("ollama");
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }

        [HttpPost]
        public async Task StreamChat([FromBody] ChatRequest request)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            var ollamaResp = await _client.PostAsync(
                "/v1/chat/completions",
                content
            );

            Response.ContentType = "text/event-stream";
            await using var stream = await ollamaResp.Content.ReadAsStreamAsync();

            var buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // SSE format: “data: <payload>\n\n”
                await Response.WriteAsync($"data: {chunk}\n\n");
                await Response.Body.FlushAsync();
            }
        }
        
    }
}
