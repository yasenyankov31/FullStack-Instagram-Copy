using InstagramCopyApi.Data;
using InstagramCopyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InstagramCopyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly InstagramContext _context;
        private readonly IWebHostEnvironment _env;
        private static Random random = new Random();

        public AccountController(InstagramContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        private string GenerateToken(User user)
        {
            string [] roles = {"Admin","User"};
            List<Claim> claims = new List<Claim>
            {
                new Claim("id",user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,roles[user.Role]),
            };
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha384Signature);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], 
                claims: claims,
                expires:DateTime.Now.AddMinutes(20),
                signingCredentials:credentials
                
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        [AllowAnonymous]
        [HttpPost("login")]

        public IActionResult Login([FromForm] User user)
        {

            var response = Unauthorized();
            var dbUser = _context.Users.Where(x => x.Email == user.Email
                && x.Password == user.Password).FirstOrDefault();
            if (dbUser != null)
            {
                user.Id = dbUser.Id;
                var token = GenerateToken(user);
                return Ok(new {token, role=dbUser.Role,profilepicUrl=dbUser.UserProfilePicture});

            }
            return response;

        }
        [Authorize]
        [HttpGet("isAuth")]
        public IActionResult isAuth()
        {

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register")]

        public async Task<IActionResult> Register([FromForm]User user)
        {
            user.Role = 1;
            if (_context.Users == null)
            {
                return BadRequest("Entity set 'InstagramContext.Users'  is null.");
            }
            if (user.Image != null)
            {
                var extension = Path.GetExtension(user.Image.FileName);
                var imageUrl = Path.Combine("Media/Images/", RandomString(16) + extension);
                user.UserProfilePicture = "/" + imageUrl;
                var realPath = Path.Combine(_env.WebRootPath, imageUrl);

                using (var stream = new FileStream(realPath, FileMode.Create))
                {
                    user.Image.CopyTo(stream);
                }
            }
            else
            {
                user.UserProfilePicture = "/Media/deafultUser.png";
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            user.Id =_context.Users.OrderBy(item => item.Id).Last().Id;
            var token = GenerateToken(user);
            return Ok(new { token, role = 1, profilepicUrl = user.UserProfilePicture });

        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
