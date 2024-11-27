using System;

namespace server.DTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DateCommented { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
    }
}
