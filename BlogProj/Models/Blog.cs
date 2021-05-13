using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Models
{
    public class Blog
    {
        public int Id { get; set; } //Primary Key
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; } //represents name of Blog
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]

        public string Description { get; set; }  //Descriptive of Blog post
        [DataType(DataType.Date)]
        [Display(Name ="Created Date")]
        public DateTime Created { get; set; } //Time Created
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } // Updated Time 

        public byte[] ImageData{ get; set; }
        public string ContentType { get; set; }

        [NotMapped]
        [Display(Name = "Choose Image")]
        public IFormFile ImageFile { get; set; }

        //Navagational Properties
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();//relational connect. Parent to child. Blog to Posts

      
    }
}
