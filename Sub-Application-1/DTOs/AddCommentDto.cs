namespace Sub_Application_1.DTOs
{
    public class AddCommentDto
    {
        // cannot add a comment without a post id and content, therefore required
        public required int PostId { get; set; }
        public required string Content { get; set; }
    }
}
