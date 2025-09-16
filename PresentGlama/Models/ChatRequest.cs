namespace PresentGlama.Models
{
    public class ChatRequest
    {
        public string Model { get; set; } = "llama3.2";
        public List<Message> Messages { get; set; } = new();
        public bool Stream { get; set; } = true;
    }
}
