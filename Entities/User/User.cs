using Entities.Constants;
using Microsoft.AspNetCore.Identity;
using System;

namespace Entities
{
    public class User : IdentityUser<Guid>, IEntity<Guid>
    {
        public User()
        {
            IsActive = true;
        }
        public string Code { get; set; }
        public string ImageLink { get; set; }
        public GenderType Gender { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public DateTime BirthDay { get; set; }
        public string NationalCode { get; set; }
        public string PostalCode { get; set; }

        public string Address { get; set; }

        public DateTimeOffset? LastLoginDate { get; set; }
        public bool IsActive { get; set; }

    }

}
