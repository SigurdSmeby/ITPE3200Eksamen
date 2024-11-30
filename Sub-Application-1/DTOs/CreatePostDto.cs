namespace Sub_Application_1.DTOs
{
    public class CreatePostDto
    {
        public IFormFile? Image { get; set; } // Optional, only for image posts
        public string? TextContent { get; set; } // Optional, only for text posts
        public int? FontSize { get; set; } // Optional, default to 16 in controller if not provided
        public string? TextColor { get; set; } = "#000000";// Optional, only for text posts
        public string?  BackgroundColor { get; set; } = "#FFFFFF";// Optional, only for text posts
    }
}
