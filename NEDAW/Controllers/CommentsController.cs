using Microsoft.AspNet.Identity;
using NEDAW.Models;
using NEDAW.Repository;
using NEDAW.ViewModels;
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

        [HttpGet]
        public async Task<ActionResult> New(int newsId)
        {
            var newCommentModel = new Comment();
            newCommentModel.NewsId = newsId;
            return PartialView("_AddComment", newCommentModel);
        }

        [HttpPost]
        public async Task<ActionResult> New(Comment _comment)
        {
            _comment.CreatedBy = Guid.Parse(User.Identity.GetUserId());
            _comment.UserId = User.Identity.GetUserId();
            _comment.CreatedOn = _comment.ModifiedOn = DateTime.Now;
            await _repository.Add(_comment);
            return RedirectToAction("Show", "News", _comment.NewsId);
        }

        public async Task<ActionResult> Save()
        {
            return View();
        }
    }
}