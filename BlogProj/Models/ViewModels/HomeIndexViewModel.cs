using BlogProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace BlogProj.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Post> FeaturedPosts { get; set; }
        public IPagedList<Blog> Blogs { get; set; }
    }
}
