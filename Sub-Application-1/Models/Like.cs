using System;
namespace Sub_Application_1.Models
{
	public class Like
	{
		public required string  UserId { get; set; } // Composite key
		public required int PostId { get; set; } // Composite key

		public DateTime DateLiked { get; set; } = DateTime.Now;

		// Navigation properties
		public virtual User User { get; set; } = default!; // can be nullable because EF should populate it
		public virtual Post Post { get; set; } = default!; // can be nullable because EF should populate it
	}
}
