using Entities.TempUser;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace Client.Models.Dtos
{
    public class SignUpDto : BaseDto<SignUpDto, TempUser>
    {
        public string FName { get; set; }
        public string LName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "رمز عبور های وارد شده شبیه به هم نیستند!")]
        public string PassWordConfirmation { get; set; }

        public int OTP { get; set; }

    }
}
