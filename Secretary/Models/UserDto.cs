using Entities;
using Entities.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebFramework.Api;

namespace Secretary.Models
{
    public class UserDto : BaseDto<UserDto, User, Guid>
    {
        public bool Student { get; set; }
        public string Name { get; set; }
        public string AddressLocation { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Pelak { get; set; }
        public string vahed { get; set; }
        public string PostalCode1 { get; set; }
        public string WhoGetIt { get; set; }
        public string ManagerName { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        public string Role { get; set; }

        public string Email { get; set; }

        [NotMapped]
        public string Password { get; set; }
        [NotMapped]

        public string PasswordConfirm { get; set; }
        [Required]
        public string Fname { get; set; } = null!;
        [Required]
        public string Lname { get; set; } = null!;
        [Required]
        public GenderType Gender { get; set; }
        [Required]
        public string NationalCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string Code { get; set; }
        [Required]
        public string BirthCertificateNumber { get; set; }
        [Required]
        public string BirthDay { get; set; }
        [Required]
        public string BirthCity { get; set; }
        [Required]
        public string CertificateCity { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string PostalCode { get; set; }
        public bool IsActive { get; set; }
    }
    public class UserInfoDto : BaseDto<UserInfoDto, User, Guid>
    {
        [Required]
        public string Fname { get; set; } = null!;
        [Required]
        public string Lname { get; set; } = null!;
        [Required]
        public GenderType Gender { get; set; }
        [Required]
        public Guid LevelId { get; set; }
        [Required]
        public string NationalCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string MotherName { get; set; }
        [Required]
        public string MotherNumber { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string FatherNumber { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string BirthCertificateNumber { get; set; }
        [Required]
        public string BirthDay { get; set; }
        [Required]
        public string BirthCity { get; set; }
        [Required]
        public string CertificateCity { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string PostalCode { get; set; }
    }

    public class ChangePssDto
    {
        [Required]
        public string Pssword { get; set; }
        [Required]
        [Compare(nameof(Pssword), ErrorMessage = "رمز عبور های وارد شده شبیه به هم نیستند!")]
        public string PassWordConfirmation { get; set; }
        public Guid UserId { get; set; }
    }

}
