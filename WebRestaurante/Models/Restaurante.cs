using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRestaurante.Models
{
    public class Restaurante
    {
       [Key]
        public Guid Id_Res { get; set; }

        [Required(ErrorMessage = "Ingrese Nit del restaurante")]
        [Display(Name = "Nit del restaurante")]
        public int NitEmpresa { get; set; }

        [StringLength(30, MinimumLength = 5,ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese nombre del restaurante")]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre del restaurante")]
        public string NombreEmpresa { get; set; }

        [StringLength(2000, MinimumLength = 50, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese misión del restaurante")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Misión del restaurante")]
        public string MisionEmpresa { get; set; }

        [StringLength(2000, MinimumLength = 50, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese visión del restaurante")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Visión del restaurante")]
        public string VisionEmpresa { get; set; }

        [StringLength(1000, MinimumLength = 50, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese objetivos del restaurante")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Objetivos del restaurante")]
        public string ObjetivosEmpres { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese telefono del restaurante")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefono")]
        public string Telefono  { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese {0} del restaurante")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese {0} del restaurante")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximo {1} y Menor a {2} carateres")]
        [Required(ErrorMessage = "Ingrese {0} del restaurante")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña del correo")]
        public string CorreoContra { get; set; }

        [Display(Name = "Url de ubicación")]
        [Required(ErrorMessage = "Ingrese {0} del restaurante")]
        [DataType(DataType.Url)]
        public string Ubicacion { get; set; }

        [Display(Name = "Hora de apertura")]
        [Required(ErrorMessage = "Por favor ingrese la {0}")]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time, ErrorMessage = "Por favor ingrese una hora de apertura")]
        public DateTime HoraApertura { get; set; }

        [Display(Name = "Hora de cierre")]
        [Required(ErrorMessage = "Por favor ingrese la {0}")]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time, ErrorMessage = "Por favor ingrese una hora de cierre")]
        public DateTime HoraCierre { get; set; }

        [Display(Name = "Tiempo de espera del cliente")]
        [Required(ErrorMessage = "Por favor ingrese la {0}")]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time, ErrorMessage = "Por favor ingrese una hora de espera")]
        public DateTime TiempoEspera { get; set; }
    }
}