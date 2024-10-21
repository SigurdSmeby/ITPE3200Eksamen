// File: DTOs/PostDto.cs

using System;
using System.Collections.Generic;

namespace server.DTOs
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public DateTime DateUploaded { get; set; }
        public UserDto Author { get; set; }
        public List<CommentDto> Comments { get; set; }
        public int LikesCount { get; set; }
    }
}
