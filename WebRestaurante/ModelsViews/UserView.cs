using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRestaurante.ModelsViews
{
    public class UserView
    {
        [Display(Name = "Codigo")]
        public string UserId { get; set; }

        [Display(Name = "Nombre")]
        public string UserName { get; set; }

        [Display(Name = "Correo")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Display(Name = "Nombre del Rol")]
        public string RoleName { get; set; }

        public RolesView Rol { get; set; }

        public List<RolesView> Roles { get; set; }
    }
}