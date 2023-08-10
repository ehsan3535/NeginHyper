using DNTPersianUtils.Core;
using Entities;
using Entities.Constants;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace Client.Models
{
    public class UserDto : BaseDto<UserDto, User, Guid>
    {

        //*********************************************************************************
        //این چند تا فیلد بی ربط زیر برای صفحه جزئیات پرداخت هست و فقط داخل دی تی او هستند
        public int PostPrice { get; set; }
        public int TotalPrice { get; set; }
        public int Price { get; set; }
        public int ShopCardDetailCount { get; set; }
        public Guid ShopcardId { get; set; }
        public string RuturnUrl { get; set; }
        //**********************************************************************************
        public string Address { get; set; }
        public string PostalCode { get; set; }

        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "رمز عبور های وارد شده شبیه به هم نیستند!")]
        public string PassWordConfirmation { get; set; }
        public string Fname { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public GenderType Gender { get; set; }
        public string ImageLink { get; set; }
        public IFormFile Image { get; set; }

        public string NationalCode { get; set; }
        [Required]
        [ValidIranianMobileNumber(ErrorMessage = "شماره تلفن همراه صحیح نیست")]
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public string BirthCertificateNumber { get; set; }
        public DateTime BirthDay { get; set; }
        public string BirthCity { get; set; }
        public string CertificateCity { get; set; }
        public string Phone { get; set; }
        //[ValidIranianPostalCode(ErrorMessage = "کد پستی صحیح نیست")]
        public bool IsActive { get; set; }
    }
    public class UserInfoDto : BaseDto<UserInfoDto, User, Guid>
    {
        public string Fname { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public GenderType Gender { get; set; }
        public Guid LevelId { get; set; }
        public string NationalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string MotherName { get; set; }
        public string MotherNumber { get; set; }
        public string FatherName { get; set; }
        public string FatherNumber { get; set; }
        public string Code { get; set; }
        public string BirthCertificateNumber { get; set; }
        public string BirthDay { get; set; }
        public string BirthCity { get; set; }
        public string CertificateCity { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
    }

    public class ChangePssDto
    {
        public string Pssword { get; set; }
        public Guid UserId { get; set; }
    }

}
