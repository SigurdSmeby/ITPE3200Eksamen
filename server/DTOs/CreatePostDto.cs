namespace server.DTOs
{
    public class CreatePostDto
    {
        public string? ImageUrl { get; set; } // Optional, only for image posts
        public string? TextContent { get; set; } // Optional, only for text posts
        public string Title { get; set; }
        
        public int? FontSize { get; set; } // Optional, default to 16 in controller if not provided
        public string TextColor { get; set; } = "#000000"; // Default text color (black)
        public string BackgroundColor { get; set; } = "#FFFFFF"; // Default background color (white)
    }
}
