using DNTPersianUtils.Core;
using Entities;
using Entities.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace Secretary.Models.EditUser
{
    public class EditUserDto : BaseDto<EditUserDto, User, Guid>
    {
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Fname { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public GenderType Gender { get; set; }
        public IFormFile File { get; set; }
        public string ImageLink { get; set; }


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
        [Required]
        [ValidIranianPostalCode(ErrorMessage = "کد پستی صحیح نیست")]
        public string PostalCode { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
