using System.ComponentModel.DataAnnotations;

namespace Secretary.Models.Authentication
{
    public class SignUpOtpDto
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Otp { get; set; }
    }
}
