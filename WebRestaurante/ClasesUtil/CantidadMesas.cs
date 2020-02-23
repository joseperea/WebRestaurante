using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;
using WebRestaurante.ClasesUtil;

namespace WebRestaurante.ClasesUtil
{
    public class CantidadMesas
    {

        // cuenta las mesas disponible asta la hora actual
        public static int CMesas(WebRestauranteContext db)
        {
            int i2 = 0;
           MesasOcupada.Desactivar(db); MesasOcupada.Desactivar2(db);
            var mesa = db.Mesas.ToList();
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime hora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            foreach (var item in mesa)
            {
                var MesasOcupada = db.MesasOcupadas.Where(mo => mo.Cod_Mesa == item.Cod_Mesa).ToList();
                var MesasODate = MesasOcupada.Where(mo => (mo.Fecha_MesasO == fecha)).ToList();
                var MesasOHora = MesasODate.Where(mo => (hora >= mo.HoraIngreso_MesasO) /*&& (mo.HoraSalida_MesasO >= hora)*/).ToList();
                var MesasOcupadas = MesasOHora.Where(mo => mo.Estado_MesasO == true).ToList();
                if (MesasOcupadas.Count() == 0)
                {
                    i2++;
                }
            }
            return i2;
        }

        public static int CMesa(DateTime fecha, DateTime hora, WebRestauranteContext db)
        {
            int i2 = 0;
            MesasOcupada.Desactivar(db); MesasOcupada.Desactivar2(db);
            var mesa = db.Mesas.ToList();
            foreach (var item in mesa)
            {
                var MesasOcupada = db.MesasOcupadas.Where(mo => mo.Cod_Mesa == item.Cod_Mesa).ToList();
                var MesasODate = MesasOcupada.Where(mo => (mo.Fecha_MesasO == fecha)).ToList();
                var MesasOHora = MesasODate.Where(mo => (hora >= mo.HoraIngreso_MesasO) && (mo.HoraSalida_MesasO >= hora)).ToList();
                var MesasOcupadas = MesasOHora.Where(mo => mo.Estado_MesasO == true).ToList();
                if (MesasOcupadas.Count() == 0)
                {
                    i2++;
                }
            }
            return i2;
        }

    }
}