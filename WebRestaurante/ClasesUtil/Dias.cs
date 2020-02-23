using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class Dias
    {

        public static void listaDias(WebRestauranteContext db)
        {
            string[] listaDias = new string[8];
            listaDias[1] = "Lunes-Monday";
            listaDias[2] = "Martes-Tuesday";
            listaDias[3] = "Miercoles-Wednesday";
            listaDias[4] = "Jueves-Thursday";
            listaDias[5] = "Viernes-Friday";
            listaDias[6] = "Sabado-Saturday";
            listaDias[7] = "Domingo-Sunday";
            AddDias(listaDias, db );
        }

        public static void AddDias(string[] lista, WebRestauranteContext db)
        {
            for (int i = 1; i <= 7; i++)
            {
                string[] s = lista[i].Split(new char[] {'-'});                
                var dia = new Models.Dias
                {
                    Nombre_Dia = s[0],
                    NombreI_Dia = s[1]
                };
                db.Dias.Add(dia);
            }
            db.SaveChanges();
        }
    }
}