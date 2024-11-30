namespace Sub_Application_1.DTOs
{
    public class AddCommentDto
    {
        public required int PostId { get; set; }
        public required string Content { get; set; }
    }
}
