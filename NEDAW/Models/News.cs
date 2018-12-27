using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NEDAW.Models
{
    public class News : IModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string Image { get; set; }

        public int NewsCategoryId { get; set; }

        public NewsCategory NewsCategory { get; set; }
        
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}