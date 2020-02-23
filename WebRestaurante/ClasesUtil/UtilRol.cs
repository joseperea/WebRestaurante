using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class UtilRol : IDisposable 
    {
        private static ApplicationDbContext db = new ApplicationDbContext();


        public static void CrearRoles(string Role)
        {
            var Rolmanager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (!Rolmanager.RoleExists(Role))
            {
                Rolmanager.Create(new IdentityRole(Role));
            }
        }

        public static void AddUserRol(string Email, string contra, string Role)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var User = UserManager.FindByName(Email);
            if (User == null)
            {
                 User = new ApplicationUser
                {
                    UserName = Email,
                    Email = Email
                };
            }
            UserManager.Create(User, contra);
            UserManager.AddToRole(User.Id, Role);


        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}