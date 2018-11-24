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
        // GET: Categories
        public ActionResult Index()
        {
            var categories = _context.NewsCategories.ToList();
            var categoriesVM = new NewsCategoryVM
            {
                NewsCategories = categories
            };

            return View(categoriesVM);
        }
    }
}