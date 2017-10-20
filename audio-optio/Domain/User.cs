using System.ComponentModel.DataAnnotations;

namespace audio_optio.Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}