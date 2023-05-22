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
using Microsoft.Extensions.Hosting;

namespace InstagramCopyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly InstagramContext _context;

        public CommentsController(InstagramContext context)
        {
            _context = context;
        }
        [HttpPost("toggle-like-comment/{commentId}")]
        public async Task<IActionResult> ToggleLikeComment([FromRoute]int commentId)
        {
            int userId = Convert.ToInt32(User.Claims.First(a => a.Type == "id").Value);
            Like existingLike = await _context.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.CommentId == commentId);

            if (existingLike == null)
            {
                Like newLike = new Like
                {
                    CommentId = commentId,
                    UserId = userId
                };

                // Increment comment likes
                var comment = await _context.Comments.FindAsync(commentId);
                comment.Likes++;
                _context.Entry(comment).State = EntityState.Modified;

                _context.Likes.Add(newLike);
            }
            else
            {
                // Decrease comment likes
                var comment = await _context.Comments.FindAsync(commentId);
                comment.Likes--;
                _context.Entry(comment).State = EntityState.Modified;

                _context.Likes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Comments/5
        [HttpGet("{postId}")]
        public IActionResult GetCommentsFromPost([FromRoute]int postId)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
           
            int userId = Convert.ToInt32(User.Claims.First(x => x.Type == "id").Value);
            var query = from user in _context.Users
                        join comments in _context.Comments on user.Id equals comments.CommentatorId
                        join likes in _context.Likes.Where(l => l.UserId == userId) on comments.Id equals likes.CommentId into commentLikes
                        from like in commentLikes.DefaultIfEmpty()
                        where comments.PostId == postId
                        select new
                        {
                            commentId=comments.Id,
                            userUsername=user.Username,
                            commentLikes=comments.Likes,
                            userProfilePicture=user.UserProfilePicture,
                            commentContent=comments.CommentContent,
                            dateCreated=comments.DateCreated,
                            isLiked = like != null,
                            isCreatedByUser= userId==comments.CommentatorId

                        };

            if (query == null)
            {
                return NotFound();
            }

            return Ok(new { query });
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, string content)
        {
            Comment comment = await _context.Comments.FindAsync(id);
            var userId = Convert.ToInt32(User.Claims.First(a => a.Type == "id").Value);
            if (comment.CommentatorId == userId)
            {
                comment.CommentContent = content;
                _context.Entry(comment).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok();
            }
            return BadRequest();

        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add-comment/{postId}")]
        public async Task<ActionResult<Comment>> PostComment([FromRoute]int postId, [FromForm]string content)
        {
            Comment comment = new Comment
            {
                PostId = postId,
                Likes=0,
                CommentatorId = Convert.ToInt32(User.Claims.First(a => a.Type == "id").Value),
                CommentContent = content,
                DateCreated = DateTime.Now,

            };
            if (_context.Comments == null)
            {
                return Problem("Entity set 'InstagramContext.Comments'  is null.");
            }
            var post = await _context.Posts.FindAsync(postId);
            post.Comments++;
            _context.Entry(post).State = EntityState.Modified;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Comments/5
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute]int commentId)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment!=null)
            {
                var post = await _context.Posts.FindAsync(comment.PostId);
                post.Comments--;
                if (comment == null)
                {
                    return NotFound();
                }
                if (Convert.ToInt32(User.Claims.First(a => a.Type == "id").Value) == comment.CommentatorId)
                {
                    _context.Entry(post).State = EntityState.Modified;
                    _context.Comments.Remove(comment);
                    await _context.SaveChangesAsync();
                }

            }

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return (_context.Comments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
