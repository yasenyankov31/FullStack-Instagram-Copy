using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstagramCopyApi.Data;
using InstagramCopyApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace InstagramCopyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class UsersController : ControllerBase
    {
        private readonly InstagramContext _context;

        public UsersController(InstagramContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAllUsersnames()
        {

            if (_context.Users == null)
            {
                return NotFound();
            }
            var query = from user in _context.Users
                        select new {
                            userId=user.Id,
                            userUsername=user.Username,
                            userProfilePic=user.UserProfilePicture
                        };
            return Ok(new {query });
        }

        // GET: api/Users/5
        [HttpGet("all-users/{page}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllUsers([FromRoute]int page)
        {
            int pageSize = 10;
            if (_context.Users == null)
            {
                return NotFound();
            }

            var totalCount = await _context.Users.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1 || page > totalPages)
            {
                return BadRequest("Invalid page number.");
            }

            var users = await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound();
            }

            return Ok(new
            {
                totalPages,
                users
            });
        }


        [HttpPut("change-role/{id}")]
        public async Task<IActionResult> ChangeUserRole([FromRoute] int id) 
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                user.Role = user.Role == 1 ? 0 : 1;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPut("update-description")]
        public async Task<IActionResult> UpdateDescription([FromForm]string description)
        {
            int userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Description = description;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'InstagramContext.Users'  is null.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute]int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
