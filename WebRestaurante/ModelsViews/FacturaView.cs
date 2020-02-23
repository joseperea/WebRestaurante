using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class FacturaView
    {
        public List<factura> factura { get; set; }
        public Buscar Buscar { get; set; }
    }

    public class factura
    {
        public int Cod_Mesa { set; get; }

        public Guid Cod_Cli { get; set; }

        public int Cod_MesasO { get; set; }

        public string Numero_Mesa { set; get; }

        public bool Estado { set; get; }

        public bool Llegada { get; set; }

        public bool Ocupada { get; set; }

        public string Reservada { get; set; }

        public string NConfirmacion { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal ValorTotal { get; set; }

        public string TipoFactura { get; set; }

        public int CMenu { get; set; }

        public bool Busquedad { get; set; }
    }
    public class Buscar
    {
        public int Cbuscar { get; set; }

        [Required(ErrorMessage ="Selecione el Tipo de Busqueda")]
        public string Tbuscar { get; set; }

        [Required(ErrorMessage = "Ingresa el numero de mesa o nombre del cliente")]
        public string buscar { get; set; }
    }

}