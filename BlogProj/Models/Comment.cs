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
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string ModeratorId { get; set; }

        public string Body { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Moderated { get; set; }
        public string ModeratedBody { get; set; }

        public ModerationType ModertationType{get; set;}
        public byte[] ImageData { get; set; }

        public string ContentType { get; set; }

        [NotMapped]  //Prevents property from finding its way into database to a column
        [Display(Name = "Choose Post Image")]
        public IFormFile ImageFile { get; set; }
        public virtual Post Post { get; set; }
    public virtual BlogUser Author { get; set; }
     public virtual BlogUser Moderator { get; set; }
    }

}
