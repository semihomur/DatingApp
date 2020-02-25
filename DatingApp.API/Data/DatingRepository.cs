using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u =>u.LikerId==userId && u.LikeeId ==recipientId);
        }

        public async Task<Photo> GetMainPhoto(int id)
        {
            var photo = await _context.Photos.Where(i => i.UserId ==id).FirstOrDefaultAsync(m=> m.IsMain);
            return photo;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo =await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p=>p.Id==id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user=await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u=>u.Id==id);
            return user;
        }
        public async Task<User> GetMe(int id)
        {
            var user=await _context.Users.Include(p=> p.Photos).IgnoreQueryFilters().FirstOrDefaultAsync(u=>u.Id==id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users= _context.Users.Include(p=>p.Photos).Where(u=> u.Id !=userParams.UserId).Where(u=>u.Gender==userParams.Gender && !u.InActive).OrderByDescending(u => u.LastActive).AsQueryable();
            if(userParams.Likers) {
                var userLikers= await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u=> userLikers.Contains(u.Id));
            }
            if(userParams.Likees) {
                var userLikees= await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u=> userLikees.Contains(u.Id));
            }
            if(userParams.MinAge !=18 || userParams.MaxAge!=99) {
                var minDod = DateTime.Today.AddYears(-userParams.MaxAge-1);
                var maxDod = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth>=minDod && u.DateOfBirth<=maxDod);
            }
            if(!string.IsNullOrEmpty(userParams.OrderBy)) {
                switch(userParams.OrderBy) {
                    case "created": 
                        users =users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users= users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber,userParams.PageSize);
        }
        private async Task<IEnumerable<int>> GetUserLikes(int userId,bool likers) {
            var user = await _context.Users.Include(x=>x.Likers).Include(x=>x.Likees)
                .FirstOrDefaultAsync(x=>x.Id==userId);
            if(likers) {
                return user.Likers.Where(u=> u.LikeeId == userId).Select(x=>x.LikerId);
            }
            return user.Likees.Where(u=>u.LikerId==userId).Select(x=>x.LikeeId);
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Messages> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(i=>i.Id==id);
        }

        public async Task<PagedList<Messages>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages=  _context.Messages.Include(u=>u.Sender).ThenInclude(p=>p.Photos).Include(u=>u.Recipient).ThenInclude(p=>p.Photos).AsQueryable();
            switch(messageParams.MessageContainer) {
                case "Inbox":
                    messages=messages.Where(u=>u.RecipientId==messageParams.UserId && u.IsDeletedByRecipient ==false);
                break;
                case "Outbox":
                    messages=messages.Where(u=>u.SenderId==messageParams.UserId && u.IsDeletedBySender ==false);
                break;
                default:
                messages =messages.Where(u=>u.RecipientId==messageParams.UserId && u.IsDeletedByRecipient==false && u.IsRead ==false);
                break;
            }
            messages =messages.OrderByDescending(d=> d.SendDate);
            return await PagedList<Messages>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<Messages>> GetMessagesThread(int userId, int recipientId)
        {
            var messages= await  _context.Messages.Include(u=>u.Sender).ThenInclude(p=>p.Photos).Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
                .Where(m=>(m.RecipientId==userId && m.IsDeletedByRecipient==false && m.SenderId==recipientId) || (m.SenderId==userId && m.IsDeletedBySender==false && m.RecipientId==recipientId))
                .OrderByDescending(m=>m.SendDate).ToListAsync();
            return messages;
        }

        public async Task<bool> ReportsExist(int userId, int reportedUserId)
        {
            if (await _context.Reports.AnyAsync(x => x.ReportedUserId == reportedUserId && x.ReporterId == userId))
                return true;
            return false;
        }
    }
}