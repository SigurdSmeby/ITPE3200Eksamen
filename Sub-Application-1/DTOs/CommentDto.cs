namespace Sub_Application_1.DTOs

{
    public class CommentDto
    {
        public required int CommentId { get; set; }
        public required string Content { get; set; } 
        public required DateTime DateCommented { get; set; }
        public required string AuthorUsername { get; set; }
    }
}
