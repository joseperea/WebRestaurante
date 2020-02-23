using System;
using System.ComponentModel.DataAnnotations;
using WebRestaurante.Models;
using System.Collections.Generic;

namespace WebRestaurante.ModelsViews
{
    public class DetalleView
    {
        public int IdReserva { get; set; }

        public List<Menu> Menus { get; set; }

        public List<Mesas> Mesas { get; set; }

        public List<MesasOcupadas> addMesasO { get; set; }

        public List<DetalleMesasCliente> DetalleMesasClientes { get; set; }

        public Clientes Cliente { set; get; }

        public TipoDocumento TipoDocumento { get; set; }

        [Display(Name = "Cantidad de mesas")]
        public int Cmesas { get; set; }

        [Display(Name = "Nombre Mesas")]
        public string Nmesas { get; set; }

        [Display(Name = "Cantidad de persona")]
        public int Cpersonas { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora de Ingreso")]
        public DateTime HoraI { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Tiempo de espera")]
        public DateTime Horas { get; set; }

        public int Cmenu { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal ValorTotal { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal SubTotal { get; set; }

        public int Detalle { get; set; }

        public decimal resultado { get; set; }

        public decimal cambio { get; set; }

        public int consecutivo { get; set; }

        public bool F { get; set; }

        public bool Confirmacion { get; set; }

        public EDetalles EDetalles { get; set; }
    }

    public class EDetalles
    {
        public int IdM { get; set; }
        public int IdC { get; set; }
        public int IdMesaO { get; set; }
        public string NC { get; set; }
        public int D { get; set; }
        public bool F { get; set; }
        public string Add { get; set; }
    }

}