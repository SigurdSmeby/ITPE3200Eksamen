using System;
using System.ComponentModel.DataAnnotations;

namespace Sub_Application_1.Models
{
	public class Comment
	{
		public int CommentId { get; set; }

		[Required]
		public int PostId { get; set; }

		[Required]
		public string  UserId { get; set; }

		[Required]
		public string Content { get; set; }

		public DateTime DateCommented { get; set; } = DateTime.Now;

		// Navigation properties
		public virtual User User { get; set; }
		public virtual Post Post { get; set; }
	}
}
