using System;

namespace Sub_Application_1.Models
{
	public class Comment
	{
		public int CommentId { get; set; }

		public required int PostId { get; set; }

		public required string  UserId { get; set; }

		public required string Content { get; set; }

		public  DateTime DateCommented { get; set; } = DateTime.Now;

		// Navigation properties
		public virtual User User { get; set; } = default!;
		public virtual Post Post { get; set; } = default!;
	}
}
