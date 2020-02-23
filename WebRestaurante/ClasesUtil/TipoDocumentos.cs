using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class TipoDocumentos
    {
        public static void ListaTD(WebRestauranteContext db)
        {
            string[] listaN = new string[4];
            string[] listaD = new string[4];
            listaN[1] = "FR"; listaD[1] = "Factura";
            listaN[2] = "PR"; listaD[2] = "Pedido Reserva";
            listaN[3] = "RV"; listaD[3] = "Reserva";
            AddTD(listaN, listaD, db);
        }

        public static void AddTD(string[] listaN, string[] listaD, WebRestauranteContext db)
        {
            for (int i = 1; i <= 3; i++)
            {
                var TipoDocumento = new TipoDocumento
                {
                    Nombre_TDoc = listaN[i],
                    Descripcion_TDoc = listaD[i]
                };
                db.TipoDocumentoes.Add(TipoDocumento);
            }
            db.SaveChanges();
        }
    }
}