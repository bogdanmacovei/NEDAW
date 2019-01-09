using AutoMapper;
using NEDAW.Dtos;
using NEDAW.Models;
using NEDAW.Repository;
using NEDAW.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace NEDAW.Controllers
{
    public class NewsController : Controller
    {
        private readonly GlobalRepository<News> _repository;
        private const int pageSize = 9;

        public NewsController()
        {
            _repository = new GlobalRepository<News>();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string searchText, int? page)
        {
            int pageNumber = (page ?? 1);
            if (String.IsNullOrEmpty(searchText))
            {
                var allNews = _repository.FindAllInclude("NewsCategory", "User");
                var result = allNews.Where(n => n.Status == "Approved").OrderByDescending(n => n.ModifiedOn)
                    .Skip(pageSize * (pageNumber - 1)).Take(pageSize);

                if (result.Count() == 0)
                {
                    return HttpNotFound();
                }

                ViewBag.Pages = (int) allNews.Where(n => n.Status == "Approved").Count() / pageSize + 1;
                if (allNews.Where(n => n.Status == "Approved").Count() % pageSize == 0)
                {
                    ViewBag.Pages = ViewBag.Pages - 1;
                }

                var newsVM = new NewsVM
                {
                    News = result
                };

                return View(newsVM);
            }

            else
            {
                var selectedNews = _repository.FindAllInclude("NewsCategory", "User");
                var result = selectedNews.Where(n => n.Status == "Approved" && (n.Title.ToLower()
                    .Contains(searchText.ToLower()) || n.Content.ToLower().Contains(searchText.ToLower())))
                    .OrderByDescending(n => n.ModifiedOn)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize);

                if (result.Count() == 0)
                {
                    return HttpNotFound();
                }

                ViewBag.Pages = (int) selectedNews.Where(n => n.Status == "Approved").Count() / pageSize + 1;
                if (selectedNews.Where(n => n.Status == "Approved").Count() % pageSize == 0)
                {
                    ViewBag.Pages = ViewBag.Pages - 1;
                }

                var newsVM = new NewsVM
                {
                    News = result
                };

                return View(newsVM);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Pending()
        {
            var news = _repository.FindAllInclude("NewsCategory", "User");
            var result = news.Where(n => n.Status == "Pending");

            var newsVM = new NewsVM
            {
                News = result
            };

            return View(newsVM);
        }

        [Authorize(Roles = "User, Editor, Administrator")]
        public ActionResult New()
        {
            var categoriesRepository = new GlobalRepository<NewsCategory>();
            var categories = categoriesRepository.FindAll();
            var newsForm = new NewsForm
            {
                Categories = GetAllCategories(categories),
                ViewMode = Enums.ViewMode.Add
            };
            return View("Edit", newsForm);
        }

        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Edit(int id)
        {
            var result = _repository.FindById(id);

            if (result == null)
                return HttpNotFound();

            var newResult = Mapper.Map<News, NewsDtoForUpdate>(result);

            var categoriesRepository = new GlobalRepository<NewsCategory>();
            var categories = categoriesRepository.FindAll();

            var newsForm = new NewsForm
            {
                Title = newResult.Title,
                Content = newResult.Content,
                Image = newResult.Image,
                NewsCategoryId = newResult.NewsCategoryId,
                NewsCategory = newResult.NewsCategory,
                ModifiedBy = newResult.ModifiedBy,
                ModifiedOn = newResult.ModifiedOn,
                Categories = GetAllCategories(categories),
                ViewMode = Enums.ViewMode.Edit
            };

            return View(newsForm);
        }

        [Authorize(Roles = "User, Editor, Administrator")]
        [HttpPost]
        public ActionResult Save(NewsForm news)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", news);
            }

            if (news.ViewMode == Enums.ViewMode.Add)
            {
                // New News 
                var newsModel = Mapper.Map<NewsForm, News>(news);

                if (User.IsInRole("Administrator") || User.IsInRole("Editor"))
                {
                    newsModel.Status = "Approved";
                }
                else
                {
                    newsModel.Status = "Pending";
                }

                newsModel.CreatedBy = Guid.Parse(User.Identity.GetUserId());
                newsModel.CreatedOn = DateTime.Now;
                newsModel.UserId = newsModel.CreatedBy.ToString();
                newsModel.ModifiedOn = newsModel.CreatedOn;
                _repository.Add(newsModel);
            }
            else
            {
                // Update News
                var newsInDb = _repository.FindById(news.Id);

                // Copy from VM
                newsInDb.Title = news.Title;
                newsInDb.Image = news.Image;
                newsInDb.Content = news.Content;
                newsInDb.NewsCategoryId = news.NewsCategoryId;
                newsInDb.UserId = User.Identity.GetUserId();
                newsInDb.ModifiedOn = DateTime.Now;

                _repository.SaveChanges();
            }

            return RedirectToAction("Index", "News");
        }

        [AllowAnonymous]
        public ActionResult Category(int id, string sortByTitle = "", string sortByDate = "desc")
        {
            var news = _repository.Find(n => n.NewsCategoryId == id);

            var result = news.Include("User").Include("NewsCategory");
            news = result.Where(n => n.Status == "Approved");

            IQueryable<News> newsLocal = news;

            if (!String.IsNullOrEmpty(sortByDate))
            {
                if (sortByDate == "asc")
                    newsLocal = news.OrderBy(n => n.ModifiedOn);
                else
                    newsLocal = news.OrderByDescending(n => n.ModifiedOn);

                news = newsLocal;
            }

            if (!String.IsNullOrEmpty(sortByTitle))
            {
                if (sortByTitle == "asc")
                    newsLocal = news.OrderBy(n => n.Title);
                else
                    newsLocal = news.OrderByDescending(n => n.Title);

                news = newsLocal;
            }


            GlobalRepository<NewsCategory> _categoriesRepository = new GlobalRepository<NewsCategory>();
            var categories = _categoriesRepository.Find(c => c.Id == id);

            var titlu = categories.Select(c => c.Name).FirstOrDefault();
            ViewBag.Name = titlu;

            ViewBag.Pages = (int)news.Count() / pageSize + 1;
            if (news.Count() % pageSize == 0)
            {
                ViewBag.Pages = ViewBag.Pages - 1;
            }

            var newsVM = new NewsVM
            {
                News = news
            };

            return View(newsVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Show(int id)
        {
            var result = _repository.Find(n => n.Id == id);
            var news = result.FirstOrDefault();

            ViewBag.showButtons = false;

            if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.showButtons = true;
            }

            ViewBag.isAdmin = User.IsInRole("Administrator");
            ViewBag.currentUser = User.Identity.GetUserId();

            var newResult = Mapper.Map<News, NewsDtoForUpdate>(news);

            var commentsRepository = new GlobalRepository<Comment>();
            var comments = commentsRepository.Find(c => c.NewsId == news.Id).OrderByDescending(c => c.CreatedOn);

            var newsForm = new NewsForm
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Image = news.Image,
                NewsCategoryId = news.NewsCategoryId,
                NewsCategory = news.NewsCategory,
                ModifiedBy = Guid.Parse(news.UserId),
                User = news.User,
                ModifiedOn = news.ModifiedOn,
                Comments = comments.ToList()
            };

            return View(newsForm);
        }
        
        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Approve(int articleId)
        {
            var result = _repository.Find(n => n.Id == articleId);
            var news = result.FirstOrDefault();
            news.Status = "Approved";
            _repository.SaveChanges();

            return RedirectToAction("Pending");
        }

        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Ignore(int articleId)
        {
            var result = _repository.Find(n => n.Id == articleId);
            var news = result.FirstOrDefault();
            news.Status = "Ignored";
            _repository.SaveChanges();

            return RedirectToAction("Pending");
        }

        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Delete(int id)
        {
            var result = _repository.Find(n => n.Id == id);
            var news = result.FirstOrDefault();

            _repository.Remove(news);

            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetAllCategories(IEnumerable<NewsCategory> categories)
        {
            var selectList = new List<SelectListItem>();

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name.ToString()
                });
            }
            return selectList;
        }
    }
}