using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRestaurante.Models
{
    public class Mesas
    {
        [Key]
        public int Cod_Mesa { set; get; }

        [Display(Name = "Numero Mesa")]
        [Required(ErrorMessage ="Por favor ingrese la cantida de mesas")]
        public string Numero_Mesa { set; get; }

        [Display(Name = "Estado Mesa")]
        public bool Estado_Mesa { get; set; }

        public bool Eliminado_Mesa { get; set; }
        //Relaciones
        public virtual ICollection<MesasOcupadas> MesasOcupadas { set; get; }
    }
}