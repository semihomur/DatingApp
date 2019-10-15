using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);//Async kullanılmamasının sebebi eklediten sonra herhangi bir query yapmamız.
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhoto(int id)
        {
            var photo = await _context.Photos.Where(i => i.UserId ==id).FirstOrDefaultAsync(m=> m.IsMain);
            return photo;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo =await _context.Photos.FirstOrDefaultAsync(p=>p.Id==id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user=await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u=>u.Id==id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users= _context.Users.Include(p=>p.Photos).Where(u=> u.Id !=userParams.UserId).Where(u=>u.Gender==userParams.Gender);
            if(userParams.MinAge !=18 || userParams.MaxAge!=99) {
                var minDod = DateTime.Today.AddYears(-userParams.MaxAge-1);
                var maxDod = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth>=minDod && u.DateOfBirth<=maxDod);
            }
            
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber,userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}