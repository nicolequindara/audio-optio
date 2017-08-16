using audio_optio.Domain;

namespace audio_optio.Models
{
    public class ContactCommentModel
    {
        public Contact contact { get; set; }
        public Comment comment { get; set; }
        public bool success = false;
    }
}