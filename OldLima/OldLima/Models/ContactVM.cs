using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OldLima.Models
{
    public class ContactVM
    {
        [Required]
        [DisplayName("Naam")]
        public string Name { get; set; }
        [Required]
        [DisplayName("E-mail")]
        public string Email { get; set; }
        [DisplayName("Telefoon")]
        public string PhoneNumber { get; set; }
        [DisplayName("Boodschap")]
        public string Message { get; set; }
    }
}