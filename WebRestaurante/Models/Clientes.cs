using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRestaurante.Models
{
    public class Clientes
    {
        [Key]
        public int Cod_Cli { set; get; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Ingrese Nombres")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Maximo {1} y Menor a {2}")]
        [DataType(DataType.Text)]
        public string Nombres_Cli { set; get; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Ingrese Apellidos")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Maximo {1} y Menor a {2}")]
        [DataType(DataType.Text)]
        public string Apellidos_Cli { set; get; }

        [Display(Name = "Correo")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Ingrese Correo")]
        public string Correo_Cli { set; get; }

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "Ingrese Telefono")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2}")]
        [DataType(DataType.PhoneNumber)]       
        public string Telefono_Cli { set; get; }

        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha_Cli { set; get; }

        //Relaciones
        public virtual ICollection<DetalleMesasCliente> DetalleMesasCliente { set; get; }
    }
}