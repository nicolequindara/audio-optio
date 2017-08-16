using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace audio_optio.Domain
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public List<Order> Orders { get; set; }

        public List<Comment> Comments { get; set; }


        public void Format()
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            this.FirstName = textInfo.ToTitleCase(this.FirstName.Trim());
            this.LastName = textInfo.ToTitleCase(this.LastName.Trim());
            this.Email = textInfo.ToLower(this.Email.Trim());

            Regex regex_numbers_only = new Regex("[^0-9]");
            this.Phone = regex_numbers_only.Replace(this.Phone, "");
        }

    }

}