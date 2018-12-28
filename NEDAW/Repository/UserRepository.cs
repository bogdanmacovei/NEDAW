using Microsoft.AspNet.Identity.EntityFramework;
using NEDAW.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace NEDAW.Repository
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<IEnumerable<ApplicationUser>> FindAll()
        {
            return await Task.Run(() =>
            {
                return _context.Users.ToList();
            });
        }

        public async Task<ApplicationUser> FindById(string id)
        {
            return await Task.Run(() =>
            {
                return _context.Users.Where(ent => ent.Id == id).FirstOrDefault();
            });
        }

        public async Task<IEnumerable<IdentityRole>> Roles()
        {
            return await Task.Run(() =>
            {
                return _context.Roles.ToList();
            });
        }

        public IDbSet<IdentityRole> AllRoles()
        {
            return _context.Roles;
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