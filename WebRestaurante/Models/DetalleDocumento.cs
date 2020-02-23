using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRestaurante.Models
{
    public class DetalleDocumento
    {
        [Key]
        public int Id_DDoc { get; set; }

        public int Id_Doc { get; set; }

        public int Id_DMC { get; set; }

        //public int Cod_Menu { set; get; }

        //relaciones
        public virtual DetalleMesasCliente DetalleMesasCliente { get; set; }
        public virtual Documento Documento { get; set; }
        //public virtual Menu Menu { get; set; }
    }
}