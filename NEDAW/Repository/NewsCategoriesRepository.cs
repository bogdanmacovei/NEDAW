using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace NEDAW.Models
{
    public class NewsCategoriesRepository : IRepository<NewsCategory>
    {
        private readonly ApplicationDbContext _context;

        public NewsCategoriesRepository()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<IEnumerable<NewsCategory>> FindAll()
        {
            return await Task.Run(() =>
            {
                return _context.NewsCategories.ToList();
            });
        }

        public async Task<IQueryable<NewsCategory>> Find(Expression<Func<NewsCategory, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return _context.NewsCategories.Where(predicate);
            });
        }

        public async Task<NewsCategory> FindById(int id)
        {
            return await Task.Run(() =>
            {
                return _context.NewsCategories.Where(nc => nc.Id == id).FirstOrDefault();
            });
        }

        public void Add(NewsCategory newsCategory)
        {
            _context.NewsCategories.Add(newsCategory);
            _context.SaveChanges();
        }

        public void Remove(NewsCategory newsCategory)
        {
            _context.NewsCategories.Remove(newsCategory);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}