using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using NEDAW.Dtos;
using NEDAW.ViewModels;

namespace NEDAW.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var categories = _context.NewsCategories.ToList();
            var categoriesVM = new NewsCategoryVM
            {
                NewsCategories = categories
            };

            return View(categoriesVM);
        }

        public ActionResult Edit(int id)
        {
            var result = _context.NewsCategories
                .Where(c => c.Id == id)
                .FirstOrDefault();

            if (result == null)
                return HttpNotFound();

            var category = Mapper.Map<NewsCategory, NewsCategoryDtoForUpdate>(result);

            var newsCategoryForm = new NewsCategoryForm
            {
                Name = category.Name
            };

            return View(newsCategoryForm);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Save(NewsCategory newsCategory)
        {
            if (!ModelState.IsValid)
            {
                var newsCategoryForm = new NewsCategoryForm
                {
                    Name = newsCategory.Name
                };

                return View("Edit", newsCategoryForm);
            }

            var categoryInDb = _context.NewsCategories.Single(c => c.Id == newsCategory.Id);
            categoryInDb.Name = newsCategory.Name;

            _context.SaveChanges();

            return RedirectToAction("Index", "Categories");
        }
    }
}