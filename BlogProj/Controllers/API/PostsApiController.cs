using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogProj.Data;
using BlogProj.Models;

namespace BlogProj.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetTopXPosts(int num)
        {
            return await _context.Posts.OrderByDescending(p => p.Created).Take(num).ToListAsync();
        }

        // GET: api/PostsApi
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        //{
            //return await _context.Posts.ToListAsync();
        //}

        // GET: api/PostsApi/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Post>> GetPost(int id)
        //{
            //var post = await _context.Posts.FindAsync(id);

            //if (post == null)
            //{
                //return NotFound();
            //}

            //return post;
        //}

        // PUT: api/PostsApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPost(int id, Post post)
        //{
            //if (id != post.Id)
            //{
                //return BadRequest();
            //}

            //_context.Entry(post).State = EntityState.Modified;

            //try
            //{
                //await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
                //if (!PostExists(id))
                //{
                    //return NotFound();
                //}
                //else
                //{
                    //throw;
                //}
            //}

            //return NoContent();
        //}

        // POST: api/PostsApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Post>> PostPost(Post post)
        //{
            //_context.Posts.Add(post);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetPost", new { id = post.Id }, post);
        //}

        // DELETE: api/PostsApi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePost(int id)
        //{
            //var post = await _context.Posts.FindAsync(id);
            //if (post == null)
            //{
                //return NotFound();
            //}

            //_context.Posts.Remove(post);
            //await _context.SaveChangesAsync();

            //return NoContent();
        //}

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
