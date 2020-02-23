using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRestaurante.Models
{
    public class Contatenos
    {
        [Key]
        public int Id_Cont { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "ingrese {0} ")]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "ingrese {0} ")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [StringLength(3000, MinimumLength = 50, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "ingrese {0} ")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }
    }
}