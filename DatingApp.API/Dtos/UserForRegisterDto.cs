using System.ComponentModel.DataAnnotations;
using System;
namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(12,MinimumLength=6,ErrorMessage="You must specify password between 6 and 12")]
        public string Password { get; set; }
        [Required]
        public string Gender {get;set;}
        [Required]
        public string KnownAs {get;set;}
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City {get;set;}
        [Required]
        public string Country {get;set;}
        public DateTime Created {get;set;}
        public DateTime LastActive {get;set;}
        public UserForRegisterDto(){
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}