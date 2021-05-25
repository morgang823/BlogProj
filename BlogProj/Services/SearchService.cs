using BlogProj.Data;
using BlogProj.Enums;
using BlogProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public  IQueryable<Post> SearchContent(string searchString)
        {
            //Step 1: Get an IQueryable That contains all the posts, if user doesnt supply a search string

            var result = _context.Posts.Where(p => p.PublishState == PublishState.ProductionReady);

            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString))
            {


            result = result.Where(p => p.Title.ToLower().Contains(searchString) ||
                p.Content.ToLower().Contains(searchString) ||
                p.Abstract.ToLower().Contains(searchString) ||
                p.Blog.Title.ToLower().Contains(searchString) ||
                p.Comments.Any(c => c.Body.ToLower().Contains(searchString) ||
                c.ModeratedBody.ToLower().Contains(searchString) ||
                c.Author.FirstName.ToLower().Contains(searchString) ||
                c.Author.LastName.ToLower().Contains(searchString)));
            }

            return result.OrderByDescending(p => p.Created);
        }

       
    }
}
