using net_chat.Enum;

namespace net_chat.Model.Request.Post
{
    public class ReactionRequest
    {
       public int PostId { get; set; }
       public int ReactionType { get; set; }

    }
}
