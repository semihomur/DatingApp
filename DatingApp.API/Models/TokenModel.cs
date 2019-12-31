using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.API.Models
{
    public class TokenModel
    {
        [Key]
        public int Id {get;set;}
        [Required]
        public string ClientId {get;set;}
        [Required]
        public string Value {get;set;}
        [Required]
        public DateTime CreatedDate {get;set;}
        [Required]
        public int UserId {get;set;}
        [Required]
        public DateTime LastModifiedDate {get;set;}
        [Required]
        public DateTime ExpiryDate {get;set;}
        [ForeignKey("UserId")]
        public virtual User User {get;set;}

    }
}