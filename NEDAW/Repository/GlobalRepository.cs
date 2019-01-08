using Microsoft.AspNet.Identity.EntityFramework;
using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace NEDAW.Repository
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

        public IEnumerable<T> FindAll()
        {
            return _objectSet.ToList();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate);
        }

        public T FindById(int id)
        {
            return _objectSet.Where(ent => ent.Id == id).FirstOrDefault();
        }

        public void Add(T entity)
        {
            _objectSet.Add(entity);
            SaveChanges();
        }

        public void Remove(T entity)
        {
            _objectSet.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<T> FindAllInclude(params string[] entities)
        {
            IQueryable<T> localContext = _objectSet;

            foreach (var entity in entities)
                localContext = localContext.Include(entity);

            return localContext.ToList();
        }
    }
}