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
        public async Task<ActionResult> Index()
        {
            var news = await _repository.FindAllInclude("NewsCategory", "User");
            var result = news.Where(n => n.Status == "Approved");

            var newsVM = new NewsVM
            {
                News = result
            };

            return View(newsVM);
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