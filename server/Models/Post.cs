using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
	public class Post
	{
		public int PostId { get; set; }

		[Required]
		public int UserId { get; set; } // Foreign key

		[Required]
		public string ImageUrl { get; set; }

		[MaxLength(255)]
		public string Title { get; set; }

		public DateTime DateUploaded { get; set; } = DateTime.Now;

		// Navigation properties
		public virtual User User { get; set; }
		public virtual ICollection<Like> Likes { get; set; }
		public virtual ICollection<Comment> Comments { get; set; }
	}
}