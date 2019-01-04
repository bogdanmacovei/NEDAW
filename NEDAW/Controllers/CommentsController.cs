using NEDAW.Models;
using NEDAW.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NEDAW.Controllers
{
    public class CommentsController : Controller
    {
        private readonly GlobalRepository<Comment> _repository;

        public CommentsController()
        {
            _repository = new GlobalRepository<Comment>();
        }

        public async Task<ActionResult> Save()
        {
            return View();
        }
    }
}