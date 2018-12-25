﻿using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NEDAW.ViewModels
{
    public class NewsForm
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Max Length is 50 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required!")]
        public string Content { get; set; }

        public string Image { get; set; }

        public int NewsCategoryId { get; set; }

        public NewsCategory NewsCategory { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}