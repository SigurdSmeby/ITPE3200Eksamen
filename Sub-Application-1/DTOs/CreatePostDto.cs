namespace Sub_Application_1.DTOs
{
    public class CreatePostDto
    {
        public IFormFile? Image { get; set; } 
        public string? TextContent { get; set; } 
        public int? FontSize { get; set; } 
        public string? TextColor { get; set; } = "#000000";
        public string?  BackgroundColor { get; set; } = "#FFFFFF";
    }
}
