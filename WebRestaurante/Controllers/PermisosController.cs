using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;

namespace WebRestaurante.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PermisosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Permisos
        public ActionResult Index(int? page)
        {
            try
            {

            }
            catch (System.Exception)
            {

                throw;
            }
            page = (page ?? 1);
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            
            var ListaUser = UserManager.Users.ToList();
            var usersView = new List<UserView>();
            foreach (var User in ListaUser)
            {
                var userView = new UserView
                {
                    UserId = User.Id,
                    UserName = User.UserName,
                    UserEmail = User.Email
                };
                usersView.Add(userView);
                usersView.OrderBy(u => u.UserName).
                          ThenBy(u => u.UserEmail);
            }
            return View(usersView.ToPagedList((int)page, 5));
        }

        public ActionResult Roles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //obtener todos los datos de los usuarios y roles
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            //ordenarlos en una lista, roles y usurarios
            var users = UserManager.Users.ToList();
            var roles = RoleManager.Roles.ToList();

            //ordenar mi modelo 
            var RolesView = new List<RolesView>();

            //hacer una busqueda del usuario
            var User = users.Find(u => u.Id == id);

            if (User == null)
            {
                return HttpNotFound();
            }
            foreach (var R in User.Roles)
            {
                //hacer una busqueda del usuario si tiene roles
                var rol = roles.Find(r => r.Id == R.RoleId);

                //ordenarlo para mi modelo de roles
                var RoleView = new RolesView
                {
                    RoleId = rol.Id,
                    RoleName = rol.Name
                };
                //Añadirlo a mi modelos para poderlo mostrar en la vista
                RolesView.Add(RoleView);
            }

            //ordenarlo para mi modelo de usuarios
            var userView = new UserView
            {
                UserId = User.Id,
                UserName = User.UserName,
                UserEmail = User.Email,
                Roles = RolesView
            };

            //mostrarlo en la vista
            return View(userView);
        }
        public ActionResult AddRoles(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var roles = RoleManager.Roles.ToList();
            var users = UserManager.Users.ToList();
            var user = users.Find(r => r.Id == UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userView = new UserView
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email
            };

            var ListaRoles = RoleManager.Roles.ToList();
            ListaRoles.Add(new IdentityRole { Id = "", Name = "{seleccione el Rol...}" });
            ViewBag.RolesId = new SelectList(ListaRoles.OrderBy(r => r.Name), "Id", "Name");
            return View(userView);
        }

        [HttpPost]
        public ActionResult AddRoles(string UserId, FormCollection From)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var users = UserManager.Users.ToList();
            var user = users.Find(u => u.Id == UserId);
            var listaRol = RoleManager.Roles.ToList();
            var RolesView = new List<RolesView>();

            var userView = new UserView
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email
            };
            var RolId = Request["RolesId"];
            if (string.IsNullOrEmpty(RolId))
            {
                ViewBag.Error = "Por favor seleccione el Rol";
                listaRol.Add(new IdentityRole { Id = "", Name = "{seleccione el Rol...}" });
                ViewBag.RolesId = new SelectList(listaRol.OrderBy(r => r.Name), "Id", "Name");
                return View(userView);
            }
            var rol = listaRol.Find(r => r.Id == RolId);
            if (!UserManager.IsInRole(UserId, rol.Name))
            {
                UserManager.AddToRole(UserId, rol.Name);
            }
            else
            {
                ViewBag.Error = "El Rol (" + rol.Name + ") Ya lo tiene Asignado";
            }
            user = users.Find(r => r.Id == UserId);

            foreach (var R in user.Roles)
            {
                rol = listaRol.Find(r => r.Id == R.RoleId);
                var roleView = new RolesView
                {
                    RoleId = rol.Id,
                    RoleName = rol.Name
                };
                RolesView.Add(roleView);
            }
            userView = new UserView
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                Roles = RolesView
            };

            return View("Roles", userView);
        }

        public ActionResult EliminarRol(string UserId, string Rolid)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if ((string.IsNullOrEmpty(UserId)) || (string.IsNullOrEmpty(UserId)))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = RoleManager.Roles.ToList();
            var users = UserManager.Users.ToList();
            var RolesView = new List<RolesView>();

            var user = users.Find(u => u.Id == UserId);
            var role = roles.Find(r => r.Id == Rolid);

            if (UserManager.IsInRole(user.Id, role.Name))
            {
                UserManager.RemoveFromRole(user.Id, role.Name);
            }
            foreach (var R in user.Roles)
            {
                var rol = roles.Find(r => r.Id == R.RoleId);
                var roleView = new RolesView
                {
                    RoleId = rol.Id,
                    RoleName = rol.Name
                };
                RolesView.Add(roleView);
            }
            var userView = new UserView
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                Roles = RolesView
            };

            return View("Roles", userView);
        }
    }
}