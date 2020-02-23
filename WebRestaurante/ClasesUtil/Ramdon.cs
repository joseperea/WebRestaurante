using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRestaurante.ClasesUtil
{
    public class Ramdon
    {
        public static string Numero()
        {
            var Random = new Random();
            string NReserva = string.Format("{0:06}", Random.Next(999999));
            return NReserva;
        }
    }
}