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

namespace NEDAW.Controllers
{
    public class NewsController : Controller
    {
        private readonly GlobalRepository<News> _repository;

        public NewsController()
        {
            _repository = new GlobalRepository<News>();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var news = await _repository.FindAll();
            var newsVM = new NewsVM
            {
                News = news
            };

            return View(newsVM);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var result = await _repository.FindById(id);

            if (result == null)
                return HttpNotFound();

            var newResult = Mapper.Map<News, NewsDtoForUpdate>(result);

            var newsForm = new NewsForm
            {
                Title = newResult.Title,
                Content = newResult.Content,
                Image = newResult.Image,
                NewsCategoryId = newResult.NewsCategoryId,
                NewsCategory = newResult.NewsCategory,
                ModifiedBy = newResult.ModifiedBy,
                ModifiedOn = newResult.ModifiedOn
            };

            return View(newsForm);
        }
    }
}