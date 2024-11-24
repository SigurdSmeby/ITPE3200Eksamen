using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sub_Application_1.Models
{
	public class User
	{
		public int UserId { get; set; }

		[Required, MaxLength(50)]
		public string Username { get; set; }

		[Required, MaxLength(100), EmailAddress]
		public string Email { get; set; }

		[Required]
		public string PasswordHash { get; set; }

		public string ProfilePictureUrl { get; set; } = "/images/default_profile.jpg";

		[MaxLength(500)] // Set a limit for the bio length (optional)
		public string Bio { get; set; } = "Welcome to my page";// New Bio field

		public DateTime DateJoined { get; set; } = DateTime.Now;

		// Navigation properties
		public virtual ICollection<Post> Posts { get; set; }
		public virtual ICollection<Like> Likes { get; set; }
		public virtual ICollection<Comment> Comments { get; set; }
	}
}
