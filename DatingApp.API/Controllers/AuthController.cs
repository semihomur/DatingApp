using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            this.repo = repo;
            this.configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request
            //Validation için eğer apicontroller'ı silersek parametre bölümüne [FromBody]eklenmeli daha sonra hata mesajı verdirmek için
            //If(!ModelState.IsValid)
            //  return BadRequest(ModelState);  
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists.");
            var userToCreate = _mapper.Map<User>(userForRegisterDto);//destination-source
            var createdUser = await repo.Register(userToCreate, userForRegisterDto.Password);
            var usertoReturn = _mapper.Map<UserForDetailedDto>(createdUser);
            return CreatedAtRoute("GetUser",new {Controller= "Users", id = createdUser.Id},usertoReturn);//StatusCode(201)--basarılı old mesaj
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            try
            {
                var userFromRepo = await repo.Login(userForLoginDto.Username, userForLoginDto.Password);
                if (userFromRepo == null)
                    return Unauthorized();
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };
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
                var userForReturn = _mapper.Map<UserForListDto>(userFromRepo);
                return Ok(new
                {
                    token = tokenHandler.WriteToken(token),
                    user = userForReturn
                });
            }
            catch
            {
                return StatusCode(500, "Something went wrong!");
            }
        }
    }
}