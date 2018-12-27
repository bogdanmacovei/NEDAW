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
    public class CategoriesController : Controller
    {
        private readonly GlobalRepository<NewsCategory> _repository;

        // trebuie modificat incat sa nu mai ating contextul
        private readonly ApplicationDbContext _context;

        public CategoriesController()
        {
            _repository = new GlobalRepository<NewsCategory>();
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            // var categories = await _repository.FindAll(); 
            var categories = _context.NewsCategories.ToList();
            var categoriesVM = new NewsCategoryVM
            {
                NewsCategories = categories
            };

            return View(categoriesVM);
        }

        public async Task<ActionResult> New()
        {
            var category = new NewsCategoryForm
            {
                ViewMode = Enums.ViewMode.Add
            };
            return View("Edit", category);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var result = await _repository.FindById(id);

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
        public async Task<ActionResult> Save(NewsCategory newsCategory)
        {
            if (!ModelState.IsValid)
            {
                var newsCategoryForm = new NewsCategoryForm
                {
                    Name = newsCategory.Name
                };

                return View("Edit", newsCategoryForm);
            }

            var categoryInDb = await _repository.FindById(newsCategory.Id);
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
                await _repository.SaveChanges();
            }

            return RedirectToAction("Index", "Categories");
        }

        private void CreateNewCategoryContext(NewsCategory category)
        {
            _context.NewsCategories.Add(category);
            _context.SaveChanges();
        }
    }
}