using BlogProj.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Models
{
    public class Post
    {
        public int Id { get; set; } //Primary key

        public int BlogId { get; set; } //Foreign Key
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]

        public string Title { get; set; } //Title Post
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]

        public string Abstract { get; set; } //Description
        [Required]
        public string Content { get; set; } //Main information 
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } //Time Created
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } // Updated Time 

        public string Slug { get; set; }

        [Required]
        [Display(Name = "Publish State")]
        public PublishState PublishState { get; set; }

        public byte[] ImageData { get; set; }

        public string ContentType {get; set;}

        [NotMapped]  //Prevents property from finding its way into database to a column
        [Display(Name = "Choose Post Image")]
        public IFormFile ImageFile { get; set; }

        public virtual Blog Blog { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
