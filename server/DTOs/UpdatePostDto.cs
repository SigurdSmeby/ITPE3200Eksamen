namespace server.DTOs
{
	public class UpdatePostDto
	{
		public string? ImageUrl { get; set; } // Optional, only for image posts
		public string? TextContent { get; set; } // Optional, only for text posts
		public int? FontSize { get; set; } = 16; // Default font size
		public string TextColor { get; set; } = "#000000"; // Default text color (black)
		public string BackgroundColor { get; set; } = "#FFFFFF"; // Default background color (white)
	}
}
