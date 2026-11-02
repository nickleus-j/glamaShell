namespace PresentGlama
{
    public class GlammaOptions
    {
        public string DefaultTitle { get; set; } = "Ollama Chat UI wrapper";
        public string OllamaAddress { get; set; } = "http://localhost:11434";
        public string AllowedHosts { get; set; } = string.Empty;
    }
}
