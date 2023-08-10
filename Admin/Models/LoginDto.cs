using Entities;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace Client.Models
{
    public class LoginDto : BaseDto<LoginDto, User, Guid>
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
