// File: DTOs/CommentDto.cs

using System;

namespace server.DTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime DateCommented { get; set; }
        public string AuthorUsername { get; set; }
    }
}
