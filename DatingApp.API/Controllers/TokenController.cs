using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{   
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        private readonly TokenModel _token;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public TokenController(UserManager<User> userManager, IOptions<AppSettings> appSettings, TokenModel token, DataContext context,IMapper mapper)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _token = token;
            _context = context;
            _mapper = mapper;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Auth([FromBody] TokenRequestModel model) {
            // We will return generic 500 http server status error
            // If we receive an invalid payload
            if(model == null) {
                return new StatusCodeResult(500);
            }
            switch(model.GrantType) {
                case "password":
                    return await GenerateNewToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                default:
                    return new UnauthorizedResult(); // 401
            }
        }
        private async Task<IActionResult> GenerateNewToken(TokenRequestModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password)) {
                var newRToken = CreateRefreshToken(_appSettings.ClientId, user.Id);
                var oldrTokens = _context.Tokens.Where(rt => rt.UserId == user.Id);
                if (oldrTokens !=null) {
                    foreach(var oldrt in oldrTokens) {
                        _context.Tokens.Remove(oldrt);
                    }
                }
                _context.Tokens.Add(newRToken);
                await _context.SaveChangesAsync();
                var accessToken = await CreateAccesToken(user, newRToken.Value);
                var appUser = await _userManager.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u=>u.UserName == model.Username);
                var userForReturn = _mapper.Map<UserForListDto>(appUser);
                return Ok(new {
                    authToken = accessToken,
                    user = userForReturn
                });
            }
            return Unauthorized();
        }
        private TokenModel CreateRefreshToken(string cliendId, int userId) {
            return new TokenModel() {
                ClientId = cliendId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(90)
            };
        }
        private async Task<TokenResponse> CreateAccesToken(User user, string refreshToken)
        {
              double tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);
              var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Token));
              var roles = await _userManager.GetRolesAsync(user);
              var tokenHandler = new JwtSecurityTokenHandler();
               var claims = new List<Claim> 
                {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
                };
            foreach(var role in roles){
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime),
                SigningCredentials = creds
            };
            var newtoken = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(newtoken);
            return new TokenResponse(){
                token = encodedToken,
                expiration = newtoken.ValidTo,
                UserRoles = roles,
                refresh_token = refreshToken,
                username = user.UserName
            };
        }

        private async Task<IActionResult> RefreshToken(TokenRequestModel model)
        {
            try {
                var rt = _context.Tokens.FirstOrDefault(t=>t.ClientId == _appSettings.ClientId && t.Value == model.RefreshToken.ToString());
                if( rt == null) {
                    return new UnauthorizedResult();
                }
                if(rt.ExpiryDate < DateTime.UtcNow) {
                    return new UnauthorizedResult();
                }
                var user = await _userManager.FindByIdAsync(rt.UserId.ToString());
                if (user ==null) {
                    return new UnauthorizedResult();
                }
                var rtNew=CreateRefreshToken(rt.ClientId,rt.UserId);
                _context.Tokens.Remove(rt);
                _context.Tokens.Add(rtNew);
                await _context.SaveChangesAsync();
                var response = await CreateAccesToken(user, rtNew.Value);
                var appUser = await _userManager.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u=>u.UserName == model.Username);
                var userForReturn = _mapper.Map<UserForListDto>(appUser);
                return Ok(new {
                    authToken = response,
                    user = userForReturn
                    });
            }   
            catch(Exception ex) {
                    return new UnauthorizedResult();
            }
        }
    }
}