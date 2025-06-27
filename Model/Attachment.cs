using System.ComponentModel.DataAnnotations;

namespace net_chat.Model
{
    public class Attachment
    {
        [Required] public int Id { get; set; }
        [Required] public int MessageId { get; set; }
        [Required] public string FileUrl { get; set; } = "";
        [Required] public string FileType { get; set; } = ""; //`image.png` = `image/png`, `video.mp4` = `video/mp4`, `document.pdf` = `application/pdf`, `audio.mp3` = `audio/mpeg`, `archive.zip` = `application/zip`
        [Required] public long? FileSize { get; set; }
        public Messages? Message { get; set; }
    }
}
