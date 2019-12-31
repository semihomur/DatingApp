using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    public class TokenResponse
    {
        public string token{get;set;} 
        public DateTime expiration{get;set;} 
        public string refresh_token{get;set;}
        public IList<string> UserRoles  { get; set; }
        public string username{get;set;}

    }
}