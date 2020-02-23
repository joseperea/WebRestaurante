using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRestaurante.Models
{
    public class Menu
    {
        [Key]
        public int Cod_Menu { set; get; }

        [Required(ErrorMessage = "Ingrese nombre del menú")]
        [Display(Name = "Nombre del menú")]
        [StringLength(30, ErrorMessage = "Maximo {1} y Menor a {2}", MinimumLength = 3)]
        public string Nombre_Menu { set; get; }

        [StringLength(200, ErrorMessage = "Maximo {1} y Menor a {2}", MinimumLength = 10)]
        [Display(Name = "Descripcio de menú")]
        [Required(ErrorMessage = "Ingrese descripción del menú")]
        [DataType(DataType.MultilineText)]
        public string Descripcion_Menu { set; get; }

        [Display(Name = "Valor del menu")]
        [Required(ErrorMessage = "Ingrese Valor del Menu")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Valor_Menu { set; get; }

        [Display(Name = "Imagen del menu")]
        //[Required(ErrorMessage = "Ingrese Imagen")]
        public byte[] Imagen_Menu { get; set; }
        
        public int Cod_TMenu { set; get; }

        [Display(Name = "Estado del menu")]
        public bool Estado_Menu { get; set; }

        public bool Eliminado_Menu { get; set; }

        //Relaciones
        public virtual TipoMenu TipoMenu { set; get; }
        public virtual ICollection<DayMenu> DayMenu { get; set; }
    }
}