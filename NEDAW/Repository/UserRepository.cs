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
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository()
        {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<ApplicationUser> FindAll()
        {
            return _context.Users.ToList();
        }

        public ApplicationUser FindById(string id)
        {
            return _context.Users.Where(ent => ent.Id == id).FirstOrDefault();
        }

        public IEnumerable<IdentityRole> Roles()
        {
            return _context.Roles.ToList();
        }

        public IDbSet<IdentityRole> AllRoles()
        {
            return _context.Roles;
        }

        public void Remove(ApplicationUser entity)
        {
            _context.Users.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}