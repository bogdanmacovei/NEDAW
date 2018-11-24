using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NEDAW.ViewModels
{
    public class NewsCategoryForm
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Max Length is 50 characters!")]
        public string Name { get; set; }
    }
}