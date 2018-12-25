using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace NEDAW.Models
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindAll();
        Task<IQueryable<T>> Find(Expression<Func<T, bool>> predicate);
        Task<T> FindById(int id);
        Task Add(T newEntity);
        Task Remove(T entity);
        Task SaveChanges();
    }
}