using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZwajApp.API.Data;
using ZwajApp.API.Dtos;
using ZwajApp.API.Models;

namespace ZwajApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthrepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthrepository repo,IConfiguration config)
        {
           _config=config;
           _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto){
           userForRegisterDto.username=userForRegisterDto.username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.username))
            return BadRequest("this user is exsit");

            var userToCreate=new User{
                Username=userForRegisterDto.username
            };
            var CreatedUser=_repo.Register(userToCreate,userForRegisterDto.password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto){
            var userFromRepo= await _repo.Login(userForLoginDto.username.ToLower(),userForLoginDto.password);
            if(userFromRepo == null) return Unauthorized();
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds =new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
            var tokenDescriptror= new SecurityTokenDescriptor{
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token= tokenHandler.CreateToken(tokenDescriptror);
            return Ok(new{
             token=tokenHandler.WriteToken(token)
            });
        }


    }
}