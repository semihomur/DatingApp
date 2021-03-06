using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Models
{
    public class User : IdentityUser<int>
    {
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public int EmailCode { get; set; }
        public int PhoneNumberCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool InActive { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Like> Likers { get; set; }
        public ICollection<Like> Likees { get; set; }
        public ICollection<Messages> MessagesSent { get; set; }
        public ICollection<Messages> MessagesReceived { get; set; }
        public ICollection<Reports> ReportsSent { get; set; }
        public ICollection<Reports> ReportsReceived { get; set; }
        public ICollection<UserRole> UserRoles  { get; set; }
        public virtual List<TokenModel> Tokens {get;set;}

    }
}
