using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace NEDAW.Models
{
    public class GlobalRepository<T> : IRepository<T>
        where T : class, IModel
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _objectSet;

        public GlobalRepository()
        {
            _context = new ApplicationDbContext();
            _objectSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> FindAll()
        {
            return await Task.Run(() =>
            {
                return _objectSet.ToList();
            });
        }

        public async Task<IQueryable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return _objectSet.Where(predicate);
            });
        }

        public async Task<T> FindById(int id)
        {
            return await Task.Run(() =>
            {
                return _objectSet.Where(ent => ent.Id == id).FirstOrDefault();
            });
        }

        public async Task Add(T entity)
        {
            await Task.Factory.StartNew(async () =>
            {
                _objectSet.Add(entity);
                await this.SaveChanges();
            });
        }

        public async Task Remove(T entity)
        {
            await Task.Factory.StartNew(async () =>
            {
                _objectSet.Remove(entity);
                await this.SaveChanges();
            });
        }

        public async Task SaveChanges()
        {
            await Task.Factory.StartNew(() =>
            {
                _context.SaveChanges();
            });
        }
    }
}