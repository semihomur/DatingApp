using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
            /*private readonly IDatingRepository _repo;
        public LogUserActivity(IDatingRepository repo)
        {
            _repo = repo;
        }*/
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Console.WriteLine(context.ActionDescriptor.DisplayName); çağırıldıgı action
            var resultContext = await next();
            var userId= int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo= resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive= DateTime.Now;
            await repo.SaveAll();
        }
    }
}