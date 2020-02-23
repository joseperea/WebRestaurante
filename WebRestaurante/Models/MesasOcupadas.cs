using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRestaurante.Models
{
    public class MesasOcupadas
    {
        [Key]
        public int Cod_MesasO { get; set; }

        public int Cod_Mesa { set; get; }

        [Display(Name = "Fecha de reserva")]
        [Required(ErrorMessage = "Por favor ingrese la {0}")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Por favor ingrese una Fecha")]
        public DateTime Fecha_MesasO { get; set; }

        [Display(Name = "Hora de Ingreso")]
        [Required(ErrorMessage = "Por favor ingrese la {0}")]        
        [DataType(DataType.Time, ErrorMessage ="Por favor ingrese una hora")]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime HoraIngreso_MesasO { get; set; }

        [Display(Name = "Hora de Salida")]
        [Required(ErrorMessage = "Por favor ingrese la {0}")]       
        [DataType(DataType.Time, ErrorMessage = "Por favor ingrese una hora")]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime HoraSalida_MesasO { get; set; }
        
        [Display(Name = "Cantidad de Persona")]
        [Required(ErrorMessage = "Por favor ingrese una {0}")]
        public int CPersonas_Mesas { get; set; }

        [Display(Name = "Confirmar Mesa")]
        public bool ConfirmarMesa { get; set; }

        [Display(Name = "Llegada")]
        public bool Llegada_MesasO { get; set; }

        [Display(Name = "Estado")]
        public bool Estado_MesasO { get; set; }

        [Display(Name = "Reservada")]
        public string Reservada { get; set; }

        //Relaciones
        public virtual Mesas Mesas { get; set; }
        public virtual ICollection<DetalleMesasCliente> DetalleMesasCliente { set; get; }
    }
}