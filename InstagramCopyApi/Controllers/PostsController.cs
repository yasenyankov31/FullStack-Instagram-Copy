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
using System.Text.Json;

namespace InstagramCopyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly InstagramContext _context;
        private static Random random = new Random();
        private readonly IWebHostEnvironment _env;

        public PostsController(InstagramContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpPost("toggle-like-post/{postId}")]
        public async Task<IActionResult> ToggleLikePost([FromRoute] int postId)
        {
            int userId = Convert.ToInt32(User.Claims.First(a => a.Type == "id").Value);
            Like likedPost = await _context.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);

            if (likedPost == null)
            {
                // Post is not liked, so add a new like
                Like newLike = new Like
                {
                    PostId = postId,
                    UserId = userId
                };

                // Increment post likes
                var post = await _context.Posts.FindAsync(postId);
                if (post!=null)
                {
                    post.Likes++;
                    _context.Entry(post).State = EntityState.Modified;

                    _context.Likes.Add(newLike);
                }
            }

            else
            {
                // Post is already liked, so remove the like
                // Decrease post likes
                var post = await _context.Posts.FindAsync(postId);
                post.Likes--;
                _context.Entry(post).State = EntityState.Modified;

                _context.Likes.Remove(likedPost);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        // GET: api/Posts
        [HttpGet]
        public ActionResult GetPosts()
        {
            int userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            var query = from user in _context.Users
                        join post in _context.Posts on user.Id equals post.CreatorId
                        join like in _context.Likes.Where(l => l.UserId == userId) on post.Id equals like.PostId into postLikes
                        from like in postLikes.DefaultIfEmpty()
                        select new
                        {
                            postId = post.Id,
                            UserUsername = user.Username,
                            UserProfilePicture = user.UserProfilePicture,
                            PostCreatorId = post.CreatorId,
                            PostLikes = post.Likes,
                            PostUrl = post.Url,
                            PostComments = post.Comments,
                            PostCreationDate = post.CreationDate,
                            PostDescription = post.Description,
                            isLiked = (like != null),
                            isCreatedByCurrentUser = (post.CreatorId == userId)
                        };

            if (_context.Posts == null)
            {
                return NotFound();
            }
            return Ok(new { posts = query });
        }

        // GET: api/Posts/userPosts
        [HttpGet("userPosts/{id}")]
        public async Task<ActionResult> GetUserPosts(int id)
        {
            int userId = Convert.ToInt32(User.Claims.First(x => x.Type == "id").Value);
            var isUserPage = false;
            if (id == 0 || id== userId)
            {
                id = userId;
                isUserPage = true;
            }
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var userInfo = await _context.Users.FindAsync(id);
            if (userInfo == null)
            {
                return BadRequest();
            }

            var query = from post in _context.Posts
                        join user in _context.Users on post.CreatorId equals user.Id
                        join like in _context.Likes on post.Id equals like.PostId into postLikes
                        where post.CreatorId == id
                        select new
                        {
                            postId = post.Id,
                            UserUsername = user.Username,
                            PostCreatorId = post.CreatorId,
                            PostLikes = post.Likes,
                            PostUrl = post.Url,
                            PostComments = post.Comments,
                            PostCreationDate = post.CreationDate,
                            PostDescription = post.Description,
                            isCreatedByCurrentUser = isUserPage,
                            isLiked = postLikes.Any(like => like.UserId == userId && post.Id==like.PostId),
                            Likes = postLikes
                        };

            Dictionary<string, string> userInfoDict = new()
            {
                { "username", userInfo.Username },
                { "userProfilePicture", userInfo.UserProfilePicture },
                { "description", userInfo.Description },
                { "isUserPage",isUserPage?"true":""}
            };
            if (query == null)
            {
                return Ok(new { userInfoDict });
            }

            return Ok(new { userInfoDict, query });
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost([FromRoute]int id, [FromForm] Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            Post oldPost =_context.Posts.Find(id);
            oldPost.Description = post.Description;
            

            
            if (post.Image != null)
            {
                var extension = Path.GetExtension(post.Image.FileName);
                var imageUrl = Path.Combine("Media/Images/", RandomString(16) + extension);
                oldPost.Url = "/" + imageUrl;
                var realPath = Path.Combine(_env.WebRootPath, imageUrl);

                using (var stream = new FileStream(realPath, FileMode.Create))
                {
                    post.Image.CopyTo(stream);
                }
            }


            

            try
            {
                _context.Entry(oldPost).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add-post")]
        public async Task<IActionResult> PostPost([FromForm]Post post)
        {
            post.CreationDate = DateTime.Now;
            if (_context.Posts == null)
            {
                return Problem("Entity set 'InstagramContext.Posts'  is null.");
            }

            if (post.Image != null)
            {
                var extension = Path.GetExtension(post.Image.FileName);
                var imageUrl = Path.Combine("Media/Images/", RandomString(16) + extension);
                post.Url = "/" + imageUrl;
                var realPath = Path.Combine(_env.WebRootPath, imageUrl);

                using (var stream = new FileStream(realPath, FileMode.Create))
                {
                    post.Image.CopyTo(stream);
                }
            }
            post.CreatorId = Convert.ToInt32(User.Claims.First(x => x.Type == "id").Value);
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var comments = _context.Comments.Where(x => x.PostId == post.Id).ToList();
            foreach (var item in comments)
            {
                _context.Comments.Remove(item);
            }
            var likes = _context.Likes.Where(x => x.PostId == post.Id).ToList();
            foreach (var item in likes)
            {
                _context.Likes.Remove(item);
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
