using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class TiempoEspera
    {
        public static DateTime Salida(DateTime hora, WebRestauranteContext db)
        {
            var TE = db.Restaurantes.ToList();
            int UH = hora.TimeOfDay.Hours + TE.FirstOrDefault().TiempoEspera.TimeOfDay.Hours;
            int UM = hora.TimeOfDay.Minutes + TE.FirstOrDefault().TiempoEspera.TimeOfDay.Minutes;
            int US = hora.TimeOfDay.Seconds + TE.FirstOrDefault().TiempoEspera.TimeOfDay.Seconds;
            if (US > 59)
            {
                int USs = US - 59;
                UM = UM + 1;
                UM = USs;
            }
            if (UM > 59)
            {
               int UMm = UM - 59;
                UH = UH + 1;
                UM = UMm;
            }
            if (UH > 23)
            {
                UH = 00;
            }
            DateTime Utiempo = new DateTime(hora.Year, hora.Month, hora.Day, UH, UM, US);
            return Utiempo;
        }
    }
}