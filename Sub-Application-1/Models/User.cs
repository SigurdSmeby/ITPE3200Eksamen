using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sub_Application_1.Models
{
	public class User: IdentityUser
	{

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
