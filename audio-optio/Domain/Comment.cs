using System;

using System.ComponentModel.DataAnnotations;

namespace audio_optio.Domain
{
    public class Comment
    {
        public Comment()
        {
            DateSubmitted = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Comment must be 255 characters or less")]
        public string Text { get; set; }

        public Contact Contact { get; set; }

        public DateTime DateSubmitted { get; set; }
    }
}