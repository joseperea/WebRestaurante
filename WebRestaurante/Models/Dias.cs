using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRestaurante.Models
{
    public class Dias
    {
        [Key]
        public int Cod_Dia { get; set; }

        [Display(Name ="Dias")]
        public string Nombre_Dia { get; set; }

        [Display(Name = "Dias")]
        public string NombreI_Dia { get; set; }

        public bool MenuDia { get; set; }

        //Relaciones
        public virtual ICollection<DayMenu> DayMenu { get; set; }
    }

 }