using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class Consecutivo
    {
        public static int Numero(WebRestauranteContext db)
        {
            int N = 0;
            if (db.Documento.ToList().Count() > 0)
            {
                N = db.Documento.Max(t => t.Consecutivo);
            }       
            N = N + 1;
            return N;
        }
    }
}