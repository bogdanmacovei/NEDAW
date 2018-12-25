using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEDAW.ViewModels
{
    public class NewsVM
    {
        public IEnumerable<News> News { get; set; }
    }
}