using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRestaurante.Models
{
    public class TipoMenu
    {
        [Key]
        public int Cod_TMenu { set; get; }

        [Display(Name = "Tipo de Menu")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Ingrese un nombre de menu maximo de {1} y mayor que {2}")]
        [Index("TMenu_Nombre", IsUnique = true)]
        [Required(ErrorMessage = "Ingrese el Tipo de Menu")]
        public string Nombre_TMenu { set; get; }

        [Display(Name = "Estado del Tipo Menu")]
        public bool Estado_TMenu { set; get; }

        public bool Eliminado_TMenu { set; get; }

        //Relaciones
        public virtual ICollection<Menu> Menu { set; get; }

        [NotMapped]
        public ICollection<TipoMenu> TipoMenus { get; set; }
    }
}