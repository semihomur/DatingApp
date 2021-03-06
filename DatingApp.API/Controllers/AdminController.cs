using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Identity;
using DatingApp.API.Models;
using System;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using DatingApp.API.Helpers;
using CloudinaryDotNet;
using System.Collections;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinarConfig;
        public AdminController(DataContext context, UserManager<User> userManager, IMapper mapper, IOptions<CloudinarySettings> cloudinarConfig)
        {
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
            _cloudinarConfig = cloudinarConfig;
            Account acc = new Account(
                _cloudinarConfig.Value.CloudName,
                _cloudinarConfig.Value.ApiKey,
                _cloudinarConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in user.UserRoles join role in _context.Roles on userRole.RoleId equals role.Id select role.Name).ToList()
                                  }).ToListAsync();
            return Ok(userList);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModerations")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photos.Include(p => p.User).Where(p => !p.isApproved).IgnoreQueryFilters().ToListAsync();
            var photosForReturn = _mapper.Map<PhotosForReturnWithUserDto[]>(photos);
            return Ok(photosForReturn);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("deletePhoto/{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            if(photo.PublicId!=null) {
            var deleteParams = new DeletionParams(photo.PublicId);
            var result = _cloudinary.Destroy(deleteParams);
            if (result.Result == "ok")
                _context.Remove(photo);
            }
            if(photo.PublicId ==null)
                _context.Remove(photo);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{id}")]
        public async Task<IActionResult> ApprovePhoto(int id)
        {
            var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            photo.isApproved = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var UserRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles != null ? selectedRoles : new string[] { };//selectedRoles= selectedRoles ?? new string[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(UserRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to add to roles");
            result = await _userManager.RemoveFromRolesAsync(user, UserRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to remove the roles");
            return Ok(await _userManager.GetRolesAsync(user));
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("getReports")]
        public async Task<IActionResult> GetReports() {
            var getReports = await _context.Reports.Include(r=> r.ReportedUser).Include(r=> r.Reporter).Where(r=> !r.IsDeleted).ToListAsync();
            var getReportsToReturn = _mapper.Map<ICollection<ReportReturnDto>>(getReports);
            return Ok(getReportsToReturn);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("inActivateUser/{reportId}")]
        public async Task<IActionResult> MakeInActivate(int reportId) {
            var report =await  _context.Reports.FirstOrDefaultAsync(r=> r.Id == reportId);  
            if (report == null) {
                return BadRequest();
            }
            report.IsDeleted = true;
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id == report.ReportedUserId);
            user.InActive = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("getStatistics")] 
        public async Task<IActionResult> GetStatistics() {
            var totalUser = await _context.Users.CountAsync();
            var totalLikes = await _context.Likes.CountAsync();
            var totalPhotoUploaded = await _context.Likes.CountAsync();
            return Ok(new {
                totalUser,
                totalLikes,
                totalPhotoUploaded
            });
        }
    }
}
