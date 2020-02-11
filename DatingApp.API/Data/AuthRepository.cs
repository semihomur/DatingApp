using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;

        public AuthRepository(DataContext context)
        {
            this.context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);//Async kullanılmamasının sebebi eklediten sonra herhangi bir query yapmamız.
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }
        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
                return null;
            //if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
            // return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            //user.PasswordHash=passwordHash;
            //user.PasswordSalt=passwordSalt;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await context.Users.AnyAsync(x => x.UserName == username))
                return true;
            return false;
        }

        public Task<EmailCode> GetEmailCode(string email)
        {
            return context.EmailCodes.LastOrDefaultAsync(x => x.Email == email);
        }

        public Task<PhoneNumberCode> GetPhoneNumberCode(string phoneNumber)
        {
            return context.PhoneNumberCodes.LastOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }
    }
}