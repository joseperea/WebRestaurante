using System;
using System.ComponentModel.DataAnnotations;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class ListadoReserva
    {
        public string Nombre { get; set; }

        public Guid IdCliente { get; set; }

        [Display(Name = "Cantidad de mesas")]
        public int Cmesas { get; set; }

        [Display(Name = "Cantidad de persona")]
        public int Cpersonas { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora de Ingreso")]
        public DateTime HoraI { get; set; }

        public bool Confirmacion { get; set; }

    }
}