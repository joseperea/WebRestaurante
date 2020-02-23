using System.ComponentModel.DataAnnotations;

namespace WebRestaurante.ModelsViews
{
    public class RolesView
    {
        [Display(Name = "Codigo")]
        public string RoleId { get; set; }

        [Display(Name = "Nombre")]
        public string RoleName { get; set; }
    }
}