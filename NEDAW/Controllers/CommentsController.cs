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
        public ActionResult New(int newsId)
        {
            var newCommentModel = new Comment();
            newCommentModel.NewsId = newsId;
            return PartialView("_AddComment", newCommentModel);
        }

        [HttpPost]
        public ActionResult New(Comment _comment)
        {
            _comment.CreatedBy = Guid.Parse(User.Identity.GetUserId());
            _comment.UserId = User.Identity.GetUserId();
            _comment.CreatedOn = _comment.ModifiedOn = DateTime.Now;
            _repository.Add(_comment);
            return RedirectToAction("Show", "News", new { id = _comment.NewsId });
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var entity = _repository.Find(m => m.Id == id);
            var news = entity.FirstOrDefault();

            if (news.UserId != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }

            _repository.Remove(news);


            return RedirectToAction("Show", "News", new { id = news.NewsId });
        }

        public ActionResult Save()
        {
            return View();
        }
    }
}