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
    [Authorize(Roles = "User, Editor, Administrator")]
    public class NewsController : Controller
    {
        private readonly GlobalRepository<News> _repository;

        public NewsController()
        {
            _repository = new GlobalRepository<News>();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Index(string searchText)
        {
            if (String.IsNullOrEmpty(searchText))
            {
                var allNews = await _repository.FindAllInclude("NewsCategory", "User");
                var result = allNews.Where(n => n.Status == "Approved").OrderByDescending(n => n.ModifiedOn).Take(10);

                var newsVM = new NewsVM
                {
                    News = result
                };

                return View(newsVM);
            }

            else
            {
                var selectedNews = await _repository.FindAllInclude("NewsCategory", "User");
                var result = selectedNews.Where(n => n.Status == "Approved" && (n.Title.ToLower().Contains(searchText.ToLower()) ||
                    n.Content.ToLower().Contains(searchText.ToLower()))).OrderByDescending(n => n.ModifiedOn).Take(10);

                var newsVM = new NewsVM
                {
                    News = result
                };

                return View(newsVM);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Editor, Administrator")]
        public async Task<ActionResult> Pending()
        {
            var news = await _repository.FindAllInclude("NewsCategory", "User");
            var result = news.Where(n => n.Status == "Pending");

            var newsVM = new NewsVM
            {
                News = result
            };

            return View(newsVM);
        }

        public async Task<ActionResult> New()
        {
            var categoriesRepository = new GlobalRepository<NewsCategory>();
            var categories = await categoriesRepository.FindAll();
            var newsForm = new NewsForm
            {
                Categories = GetAllCategories(categories),
                ViewMode = Enums.ViewMode.Add
            };
            return View("Edit", newsForm);
        }

        [Authorize(Roles = "Editor, Administrator")]
        public async Task<ActionResult> Edit(int id)
        {
            var result = await _repository.FindById(id);

            if (result == null)
                return HttpNotFound();

            var newResult = Mapper.Map<News, NewsDtoForUpdate>(result);

            var categoriesRepository = new GlobalRepository<NewsCategory>();
            var categories = await categoriesRepository.FindAll();

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
        public async Task<ActionResult> Save(NewsForm news)
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
                await _repository.Add(newsModel);
            }
            else
            {
                // Update News
                var newsInDb = await _repository.FindById(news.Id);

                // Copy from VM
                newsInDb.Title = news.Title;
                newsInDb.Image = news.Image;
                newsInDb.Content = news.Content;
                newsInDb.NewsCategoryId = news.NewsCategoryId;
                newsInDb.UserId = User.Identity.GetUserId();
                newsInDb.ModifiedOn = DateTime.Now;

                await _repository.SaveChanges();
            }

            return RedirectToAction("Index", "News");
        }

        public async Task<ActionResult> Category(int id, string sortByTitle, string sortByDate = "desc")
        {
            var news = await _repository.Find(n => n.NewsCategoryId == id);

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
            var categories = await _categoriesRepository.Find(c => c.Id == id);

            var titlu = categories.Select(c => c.Name).FirstOrDefault();
            ViewBag.Name = titlu;

            var newsVM = new NewsVM
            {
                News = news
            };

            return View(newsVM);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Show(int id)
        {
            var result = await _repository.Find(n => n.Id == id);
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
            var comments = await commentsRepository.Find(c => c.NewsId == news.Id);

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