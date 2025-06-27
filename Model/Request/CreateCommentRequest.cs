namespace net_chat.Model.Request
{
    public class CreateCommentRequest
    {
        public int PostId { get; set; }
        public string Content { get; set; } = "";
    }
}
