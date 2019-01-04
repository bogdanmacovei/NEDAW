using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NEDAW.Models
{
    public class Comment : IModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Message { get; set; }

        public string UserId { get; set; }

        public int NewsId { get; set; }

        public News News { get; set; }

        public virtual ApplicationUser User { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}