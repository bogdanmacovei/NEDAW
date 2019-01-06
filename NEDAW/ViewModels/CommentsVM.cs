using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEDAW.ViewModels
{
    public class CommentsVM
    {
        public int NewsId { get; set; }
        public List<Comment> Comments { get; set; }
    }
}