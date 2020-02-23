using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class DetalleReserva
    {
        public Clientes Cliente { get; set; }
        public MesasOcupadas MesasOcupada { get; set; }
    }
}