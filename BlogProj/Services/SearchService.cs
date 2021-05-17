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

            if (!string.IsNullOrEmpty(searchString))
            {


            result = result.Where(p => p.Title.Contains(searchString) ||
                p.Title.Contains(searchString) ||
                p.Abstract.Contains(searchString) ||
                p.Comments.Any(c => c.Body.Contains(searchString) ||
                c.ModeratedBody.Contains(searchString) ||
                c.Author.FirstName.Contains(searchString) ||
                c.Author.LastName.Contains(searchString)));
            }

            return result.OrderByDescending(p => p.Created);
        }

       
    }
}
