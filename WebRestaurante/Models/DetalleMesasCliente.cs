using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRestaurante.Models
{
    public class DetalleMesasCliente
    {
        [Key]
        public int Id_DMC { get; set; }

        public int Cod_Cli { set; get; }

        public int Cod_MesasO { get; set; }

        public int Cod_Menu { set; get; }

        public int Cod_TDoc { set; get; }

        public int Cantidad_DMC { get; set; }

        public bool PedidoM { set; get; }

        public string NConfirmacion_DMC { get; set; }

        public bool Estado_DMC { get; set; }

        //Relaciones

        public virtual Clientes Cliente { get; set; }

        public virtual MesasOcupadas MesasOcupadas { get; set; }

        public virtual Menu Menu { get; set; }

        public virtual TipoDocumento TipoDocumento { get; set; }

        public ICollection<DetalleDocumento> DetalleDocumento { get; set; }
    }
}