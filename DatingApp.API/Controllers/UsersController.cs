using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{   [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo =await _repo.GetUser(currentUserId);
            userParams.UserId=currentUserId;
            if(string.IsNullOrEmpty(userParams.Gender)) {
              userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";  
            }
            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.Pagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(usersToReturn);
        }
        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        { 
            User user;
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
             user = await _repo.GetUser(id);   
            }
            else {
                user = await _repo.GetMe(id);
            }
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            var reportExist = await _repo.ReportsExist(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), user.Id);
            return Ok(new {
                userToReturn,
                reportExist

            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdate) {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }
            var userFromRepo= await _repo.GetUser(id);
            _mapper.Map(userForUpdate,userFromRepo);
            if(await _repo.SaveAll())
                return NoContent();
            throw new Exception($"Updating user {id} failed on save");

        }
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id,int recipientId) {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }
            var like = await _repo.GetLike(id,recipientId);
            if(like!=null) {
                return BadRequest("You already liked this user");
            }
            
            if(await _repo.GetUser(recipientId)==null){
                return NotFound();
            }
            like =new Like
            {
                LikerId=id,
                LikeeId=recipientId
            };
            _repo.Add<Like>(like);
            if(await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to like this user");
        }
        [HttpPost("{id}/reported/{reportedUser}")]
        public async Task<IActionResult> ReportUser(int id, int reportedUser, ReportedUserDto reason) {
           if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }
            var reported =await _repo.GetUser(reportedUser);
            var reporter = await _repo.GetUser(id);
            if (reported == null) {
                return BadRequest("User could not find");
            }
            if(await _repo.ReportsExist(id, reportedUser)) {
                return BadRequest("User has been reported before");
            }
            Reports newReports = new Reports{
                ReportedUser = reported,
                Reporter = reporter,
                Reason = reason.ReasonForReport,

            };
            _repo.Add<Reports>(newReports);
            if(await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to report");
        }
    }
}