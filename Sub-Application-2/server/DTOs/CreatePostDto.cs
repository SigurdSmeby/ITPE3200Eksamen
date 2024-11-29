namespace server.DTOs
{
    public class CreatePostDto
    {
        public IFormFile? ImageFile { get; set; }
        public string? TextContent { get; set; }
        public int? FontSize { get; set; }
        public string TextColor { get; set; } = "#000000";
        public string BackgroundColor { get; set; } = "#FFFFFF";
    }
}
