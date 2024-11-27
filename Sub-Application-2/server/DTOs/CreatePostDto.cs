namespace server.DTOs
{
    public class CreatePostDto
    {
        public IFormFile? ImageFile { get; set; }
        public string? TextContent { get; set; } // Optional, only for text posts
        public int? FontSize { get; set; } // Optional, default to 16 in controller if not provided
        public string TextColor { get; set; } = "#000000";
        public string BackgroundColor { get; set; } = "#FFFFFF";
    }
}
