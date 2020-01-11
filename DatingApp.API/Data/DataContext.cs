using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext: IdentityDbContext<User, Role, int , 
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options):base(options){}
        public DbSet<Photo> Photos  { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<EmailCode> EmailCodes { get; set; }
        public DbSet<TokenModel> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {

            base.OnModelCreating(builder);
            builder.Entity<UserRole>(userRole => {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});
                userRole.HasOne(ur=> ur.Role)
                        .WithMany(r=>r.UserRoles)
                        .HasForeignKey(ur=>ur.RoleId)
                        .IsRequired();
                userRole.HasOne(ur=> ur.User)
                        .WithMany(r=>r.UserRoles)
                        .HasForeignKey(ur=>ur.UserId)
                        .IsRequired();
            });
            builder.Entity<Like>()
                .HasKey(k=>new {k.LikerId,k.LikeeId});
            builder.Entity<Like>()
                .HasOne(u=>u.Likee)
                .WithMany(u=>u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
                .HasOne(u=>u.Liker)
                .WithMany(u=>u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Messages>()
                .HasOne(u=>u.Sender)
                .WithMany(u=>u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Messages>()
                .HasOne(u=>u.Recipient)
                .WithMany(u=>u.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
             builder.Entity<Photo>().HasQueryFilter(p => p.isApproved);
        }
    }
}