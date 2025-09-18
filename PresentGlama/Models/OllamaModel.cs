namespace PresentGlama.Models
{
    public class OllamaModel
    {
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public long Size { get; set; }
        public string Digest { get; set; } = string.Empty;
    }
}
