using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebRestaurante.ClasesUtil;
using WebRestaurante.Models;
using System;

namespace WebRestaurante
{
    public class MvcApplication : HttpApplication
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private WebRestauranteContext Contextdb = new WebRestauranteContext();
        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion
            <WebRestauranteContext, Migrations.Configuration>());
            CrearRoles();
            CrearSuperUsuario(db);
            AddPermisionToUser(db);
            db.Dispose();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void CrearRoles()
        {
            UtilRol.CrearRoles("Maestro");
            UtilRol.CrearRoles("Administrador");
            UtilRol.CrearRoles("Cajero");
            UtilRol.CrearRoles("Mecero");
        }

        private void AddPermisionToUser(ApplicationDbContext db)
        {
            var RolManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var User = UserManager.FindByName(WebConfigurationManager.AppSettings["mailAccount"]);
            if (!UserManager.IsInRole(User.Id, "Maestro"))
            {
                UserManager.AddToRole(User.Id, "Maestro");
            }
            if (!UserManager.IsInRole(User.Id, "Administrador"))
            {
                UserManager.AddToRole(User.Id, "Administrador");
            }
            if (!UserManager.IsInRole(User.Id, "Cajero"))
            {
                UserManager.AddToRole(User.Id, "Cajero");
            }
            if (!UserManager.IsInRole(User.Id, "Mecero"))
            {
                UserManager.AddToRole(User.Id, "Mecero");
            }
        }

        private void CrearSuperUsuario(ApplicationDbContext db)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var User = UserManager.FindByName(WebConfigurationManager.AppSettings["mailAccount"]);
            if (User == null )
            {
                User = new ApplicationUser
                {
                    UserName = WebConfigurationManager.AppSettings["mailAccount"],
                    Email = WebConfigurationManager.AppSettings["mailAccount"]
                };
                
                UserManager.Create(User, WebConfigurationManager.AppSettings["mailPassword"]);
            }
        }

        //private void CrearRoles(ApplicationDbContext db)
        //{
        //    var RolManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        //    if (!RolManager.RoleExists("Clientes"))
        //    {
        //        RolManager.Create(new IdentityRole("Clientes"));
        //    }
        //    if (!RolManager.RoleExists("Administrador"))
        //    {
        //        RolManager.Create(new IdentityRole("Administrador"));
        //    }
        //    if (!RolManager.RoleExists("Crear"))
        //    {
        //        RolManager.Create(new IdentityRole("Crear"));
        //    }
        //    if (!RolManager.RoleExists("Eliminar"))
        //    {
        //        RolManager.Create(new IdentityRole("Eliminar"));
        //    }
        //    if (!RolManager.RoleExists("Editar"))
        //    {
        //        RolManager.Create(new IdentityRole("Editar"));
        //    }
        //    if (!RolManager.RoleExists("Ver"))
        //    {
        //        RolManager.Create(new IdentityRole("Ver"));
        //    }
        //}
    }
}
