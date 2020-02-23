using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebRestaurante.Models;

namespace WebRestaurante.Models
{
    public class TipoDocumento
    {
        [Key]
        public int Cod_TDoc { set; get; }

        public string Nombre_TDoc { set; get; }

        public string Descripcion_TDoc { get; set; }

        //Relaciones

        public ICollection<Documento> Documento { get; set; }

        public ICollection<DetalleMesasCliente> DetalleMesasCliente { get; set; }
    }
}