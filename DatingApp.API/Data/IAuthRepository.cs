using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         void Add<T>(T entity) where T:class;
         void Delete<T>(T entity) where T:class;
         Task<bool> SaveAll();
         Task<User> Register(User user,string password);
         Task<User> Login(string username,string password);
         Task<bool> UserExists(string username);
         Task<EmailCode> GetEmailCode(string email);
         Task<PhoneNumberCode> GetPhoneNumberCode(string phoneNumber);
    }
}