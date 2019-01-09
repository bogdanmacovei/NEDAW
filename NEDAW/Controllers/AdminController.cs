using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NEDAW.Models;
using NEDAW.Repository;
using NEDAW.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NEDAW.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserRepository _repository;
        
        public AdminController()
        {
            _repository = new UserRepository();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            var users = _repository.FindAll();
            var usersVM = new UsersVM
            {
                Users = users
            };
            return View(usersVM);
        }

        public ActionResult Edit(string id)
        {
            ApplicationUser user = _repository.FindById(id);

            user.AllRoles = GetAllRoles();

            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            return View(user);
        }

        public ActionResult Delete(string id)
        {
            ApplicationUser user = _repository.FindById(id);

            _repository.Remove(user);

            return RedirectToAction("Users");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = _repository.Roles();

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }

            return selectList;
        }

        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            ApplicationUser user = _repository.FindById(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                if (TryUpdateModel(user))
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;

                    var roles = _repository.Roles();
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }
                    var selectedRole = _repository.AllRoles().Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);
                    _repository.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return View(user);

            }
        }
    }
}