using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebRestaurante.ModelsViews;

namespace WebRestaurante.Models
{
    public class DayMenu
    {
        [Key]
        public int Cod_DayMenu { get; set; }

        public int Cod_Menu { set; get; }

        public int Cod_Dia { get; set; }

        //Relaciones
        public virtual Dias Dias { get; set; }
        public virtual Menu Menu { get; set; }
    }
}