using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.Models
{
    public class Documento
    {
        [Key]
        public int Id_Doc { get; set; }

        public Guid Cod_Cli { get; set; }

        public int Cod_TDoc { get; set; }

        public decimal Valor_Doc { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha_Doc { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime HoraIngreso_Doc { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime HoraSalida_Doc { get; set; }

        public string NConfirmacion_Doc { get; set; }

        public bool Estado_Doc { get; set; }

        public int Consecutivo { get; set; }

        // relaciones
        public virtual TipoDocumento TipoDocumento { get; set; }

    }
}