using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using NEDAW.Dtos;
using NEDAW.ViewModels;
using System.Threading.Tasks;
using NEDAW.Repository;

namespace NEDAW.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private readonly GlobalRepository<NewsCategory> _repository;

        public CategoriesController()
        {
            _repository = new GlobalRepository<NewsCategory>();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var categories = _repository.FindAll();
            var categoriesVM = new NewsCategoryVM
            {
                NewsCategories = categories
            };

            return View(categoriesVM);
        }

        public ActionResult New()
        {
            var category = new NewsCategoryForm
            {
                ViewMode = Enums.ViewMode.Add
            };
            return View("Edit", category);
        }

        public ActionResult Edit(int id)
        {
            var result = _repository.FindById(id);

            if (result == null)
                return HttpNotFound();

            var category = Mapper.Map<NewsCategory, NewsCategoryDtoForUpdate>(result);

            var newsCategoryForm = new NewsCategoryForm
            {
                Name = category.Name,
                ViewMode = Enums.ViewMode.Edit
            };

            return View(newsCategoryForm);
        }

        [HttpPost]
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

            var categoryInDb = _repository.FindById(newsCategory.Id);
            if (categoryInDb == null)
            {
                // New Category
                CreateNewCategoryContext(newsCategory);
                // await _repository.Add(newsCategory);
            }
            else
            {
                // Update Category
                categoryInDb.Name = newsCategory.Name;
                _repository.SaveChanges();
            }

            return RedirectToAction("Index", "Categories");
        }

        private void CreateNewCategoryContext(NewsCategory category) => _repository.Add(category);
    }
}