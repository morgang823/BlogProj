using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProj.Data;
using BlogProj.Services;
using BlogProj.Models;
using Microsoft.Extensions.Configuration;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BlogProj.Models
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;

        public PostsController(ApplicationDbContext context, IConfiguration configuration, IBlogFileService fileService, BasicSlugService slugService, SearchService searchService)
        {
            _context = context;
            _configuration = configuration;
            _fileService = fileService;
            _slugService = slugService;
            _searchService = searchService;
        }

        public async Task<IActionResult> BlogPostIndex(int? id, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }
            var blog = _context.Blogs.Find(id);
            var blogPosts = await _context.Posts.Where(p => p.BlogId == id).ToListAsync();
            //How to get records just for a parent

            ViewData["HeaderText"] = blog.Title;
            ViewData["SubText"] = blog.Description;
            ViewData["HeaderImage"] = _fileService.DecodeImage(blog.ImageData, blog.ContentType);
            return View("Index", blogPosts);
        }
        // GET: Posts
        public async Task<IActionResult> Index(int? page = 1)
        {
            ViewData["HeaderText"] = "The Post Index";
            ViewData["SubText"] = "Read all my glorious posts";
            var applicationDbContext = _context.Posts.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                  .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["HeaderText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            ViewData["HeaderImage"] = _fileService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["AuthorText"] = $"Created by Gianni Morgan on {post.Created.ToString("MMM dd, yyyy")}";

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator")]

        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,PublishState,ImageFile")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;
                post.ImageData = (await _fileService.EncodeFileAsync(post.ImageFile)) ??
               await _fileService.EncodeFileAsync(_configuration["DefaultBlogImage"]);

                post.ContentType = post.ImageFile is null ?
                    _configuration["DefaultPostImage"].Split('.')[1] : _fileService.ContentType(post.ImageFile);

                //slug service
                var slug = _slugService.UrlFriendly(post.Title);
                if(!_slugService.IsUnique(slug))
                {
                    //Checking if Slug is not unique. Will throw a model error and inform user of error
                    ModelState.AddModelError("Title", "This Title is already in use, please choose another one");
                    return View(post);
                }
                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SearchIndex(int? page, string searchString)
        {
            ViewData["SearchString"] = searchString;
            var posts = _searchService.SearchContent(searchString);
            var pageNumber = page ?? 1;
            var pageSize = 2;
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));

        }



        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,Slug,PublishState,ImageFile, ImageData,ContentType")] Post post, IFormFile ImageFile)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if(post.Slug != _slugService.UrlFriendly(post.Title))
                    {
                        if (!_slugService.IsUnique(newSlug))
                        {
                            //Checking if Slug is not unique. Will throw a model error and inform user of error
                            ModelState.AddModelError("Title", "This Title is already in use, please choose another one");
                            return View(post);
                        }

                        post.Slug = newSlug;
                    }
            
                    if(post.ImageFile is not null)
                    {
                        post.ImageData =await _fileService.EncodeFileAsync(post.ImageFile);
                        post.ContentType = _fileService.ContentType(post.ImageFile);
                    }

                    post.Updated = DateTime.Now;


                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
