using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProj.Data;
using BlogProj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogProj.Controllers
{
    [Authorize]
    public class CommentsController : Controller
{
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;


        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.Author).Include(c => c.Post);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }


        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId, Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Created = DateTime.Now;
                comment.AuthorId = _userManager.GetUserId(User);

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == comment.PostId);
                var slug = post.Slug;


                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { Slug = slug });
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,Author,AuthorId,ImageData,ContentType,ModeratorId,Body,Created,Moderated,ModeratedBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //For adding an edit notification to the text
                    var user = await _userManager.GetUserAsync(User);
                    string editNotice = $"Edited by: {user.FullName} <br />On: {DateTime.Now:MMM/dd/yyyy}<br />";
                    //End of edit notification
                    comment.Moderated = DateTime.Now;
                    if (comment.ModeratedBody is not null)
                    {
                        comment.ModeratedBody = editNotice + comment.ModeratedBody;
                        comment.Moderated = DateTime.Now;
                        comment.ModeratorId = user.Id;
                    }
                    else
                    {
                        comment.Body = editNotice + comment.Body;
                    }
                    _context.Update(comment);
                    await _context.SaveChangesAsync();

                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var post = _context.Posts.Find(comment.PostId);

                return RedirectToAction("Details", "Posts", new { slug = post.Slug }); 
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
            ViewData["ModeratorId"] = new SelectList(_context.Posts, "Id", "Id", comment.ModeratorId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
