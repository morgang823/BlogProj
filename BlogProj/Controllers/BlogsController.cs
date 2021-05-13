using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProj.Data;
using BlogProj.Models;
using Microsoft.AspNetCore.Http;
using BlogProj.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace BlogProj.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogFileService _fileService;
        private readonly IConfiguration _configuration;
        public BlogsController(ApplicationDbContext context, IBlogFileService fileService, IConfiguration configuration)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        //[Authorize(Roles = "Administrator")]
        //[Authorize(Roles = "Moderator")]
        //[Authorize(Roles = "Administrator,Moderator")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ImageFile")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Created = DateTime.Now;
                blog.ImageData = (await _fileService.EncodeFileAsync(blog.ImageFile)) ??
                await _fileService.EncodeFileAsync(_configuration["DefaultBlogImage"]);

                blog.ContentType = blog.ImageFile is null ?
                    _configuration["DefaultBlogImage"].Split('.')[1] : _fileService.ContentType(blog.ImageFile);

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,ImageData,ImageFile,ContentType")] Blog blog, IFormFile ImageFile)
        {
            
                if (id != blog.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (ImageFile is not null)
                        {

                        blog.ImageData = await _fileService.EncodeFileAsync(blog.ImageFile);
                                            
                            blog.ContentType = _fileService.ContentType(blog.ImageFile);
                    }

                        blog.Updated = DateTime.Now;
                        _context.Update(blog);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BlogExists(blog.Id))
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
                return View(blog);
            }

            // GET: Blogs/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var blog = await _context.Blogs
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (blog == null)
                {
                    return NotFound();
                }

                return View(blog);
            }

            // POST: Blogs/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var blog = await _context.Blogs.FindAsync(id);
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool BlogExists(int id)
            {
                return _context.Blogs.Any(e => e.Id == id);
            }
     }
}