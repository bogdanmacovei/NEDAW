using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEDAW.ViewModels
{
    public class UsersVM
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}