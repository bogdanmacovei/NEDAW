using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NEDAW.ViewModels
{
    public class NewsCategoryVM
    {
        public IEnumerable<NewsCategory> NewsCategories { get; set; }
    }
}