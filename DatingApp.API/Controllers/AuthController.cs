using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IDatingRepository _repo;
        private readonly IAuthRepository _authRepo;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration, IMapper mapper, IDatingRepository repo, IAuthRepository authRepo)
        {
            _repo = repo;
            _authRepo = authRepo;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            this.configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            if(await _userManager.Users.AnyAsync(x=> (x.Email == userForRegisterDto.Email || (x.UserName == userForRegisterDto.Username))))
            {
                return BadRequest("You already registered to datingapp");
            }
            var emailCode = await _authRepo.GetEmailCode(userForRegisterDto.Email);
            if( userForRegisterDto.EmailCode == emailCode.Code) {
                _repo.Delete(emailCode);
                var userToCreate = _mapper.Map<User>(userForRegisterDto);//destination-source
                var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
                var usertoReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
                if (result.Succeeded)
                {
                    await _repo.SaveAll();
                    return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, usertoReturn);//StatusCode(201)--basarılı old mesaj
                }
                return BadRequest(result.Errors);
            }
            else {
                return BadRequest("Not correct email code");
            }
            // return BadRequest("Email code is not matching.");
        }
        [HttpPost("getEmail")]
        public async Task<IActionResult> GetEmail(EmailDto emailDto)
        {
            Random random = new Random();
            int number = random.Next(100000, 999999);
            MailService.SendMail("Confirmation Mail", emailDto.Email, "Code: <br>" + number);
            var emailCode = new EmailCode() {
                Email = emailDto.Email,
                Code = number
            };
            _repo.Add(emailCode);
             if(await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to send the code");
        }
        /*[HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
                var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
                var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password,false);
                if(result.Succeeded)
                {
                    var appUser = await _userManager.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u=>u.UserName == userForLoginDto.Username);
                    var userForReturn = _mapper.Map<UserForListDto>(appUser);
                    return Ok(new
                    {
                    token = GenerateJwtToken(appUser).Result,
                    user = userForReturn
                    });
                }
                return Unauthorized();
            
        }
        private async Task<string> GenerateJwtToken(User user)
        { 
            var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles){
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }*/
    }
}