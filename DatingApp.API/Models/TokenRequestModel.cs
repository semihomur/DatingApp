namespace DatingApp.API.Models
{
    public class TokenRequestModel
    {
        public string GrantType {get;set;} // password or refresh_token
        public string ClientId{get;set;}
        public string Username{get;set;}
        public string RefreshToken{get;set;}
        public string Password{get;set;}
    }
}