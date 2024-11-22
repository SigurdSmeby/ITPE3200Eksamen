namespace server.DTOs
{
    public class UpdatePostDto
    {
        public string? TextContent { get; set; }
        public int? FontSize { get; set; } = 16;
        public string TextColor { get; set; } = "#000000";
        public string BackgroundColor { get; set; } = "#FFFFFF";
    }
}
