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
            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no"; // for nginx buffering off

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted);
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
                {
                    Content = new StringContent(JsonSerializer.Serialize(request, _jsonOptions), Encoding.UTF8, "application/json")
                };

                using var upstream = await _client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                // Optionally forward status code if you want the client to know
                if (!upstream.IsSuccessStatusCode)
                {
                    Response.StatusCode = (int)upstream.StatusCode;
                    await Response.WriteAsync($"data: {{\"error\":\"Upstream returned {(int)upstream.StatusCode}\"}}\n\n", cts.Token);
                    await Response.Body.FlushAsync(cts.Token);
                    return;
                }

                await using var stream = await upstream.Content.ReadAsStreamAsync(cts.Token);
                var buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token)) > 0)
                {
                    // Convert chunk to string
                    var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // The upstream may already send SSE-style "data: {...}\n\n" frames.
                    // To be robust, normalize: if chunk already contains "data:" lines, forward them as-is.
                    // Otherwise wrap chunk in a single data frame.
                    if (chunk.Contains("data:"))
                    {
                        // Ensure each data block ends with double newline
                        var blocks = chunk.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var block in blocks)
                        {
                            var trimmed = block.Trim();
                            if (string.IsNullOrEmpty(trimmed)) continue;
                            // If block already starts with "data:", forward directly
                            if (trimmed.StartsWith("data:"))
                            {
                                await Response.WriteAsync(trimmed + "\n\n", cts.Token);
                            }
                            else
                            {
                                // Wrap raw JSON or text
                                await Response.WriteAsync($"data: {trimmed}\n\n", cts.Token);
                            }
                        }
                    }
                    else
                    {
                        // Wrap raw chunk safely
                        await Response.WriteAsync($"data: {chunk}\n\n", cts.Token);
                    }

                    await Response.Body.FlushAsync(cts.Token);

                    // If client disconnected, RequestAborted will be triggered and cancellation will throw
                    if (cts.Token.IsCancellationRequested) break;
                }
            }
            catch (OperationCanceledException){}
            catch (Exception ex)
            {
                if (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    var errJson = JsonSerializer.Serialize(new { error = "Server error" });
                    await Response.WriteAsync($"data: {errJson}\n\n");
                    await Response.Body.FlushAsync();
                }
            }
        }


    }
}
