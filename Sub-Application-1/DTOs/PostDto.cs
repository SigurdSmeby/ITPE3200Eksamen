namespace Sub_Application_1.DTOs
{
	public class PostDto
	{
		public required int PostId { get; set; }
		public string? ImagePath { get; set; }
		public string? TextContent { get; set; }
		public DateTime DateUploaded { get; set; }
		public UserDto? Author { get; set; }
		public int LikesCount { get; set; }
		public List<CommentDto>? Comments { get; set; }
		public int? CommentsCount { get; set; }
		public int? FontSize { get; set; } = 16;
		public string? TextColor { get; set; } = "#000000";
		public string? BackgroundColor { get; set; } = "#FFFFFF";
		public List<UserDto>? Likes { get; set; } = new List<UserDto>();
	}
}
