using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class Mover
    {
        public static void Mesas(int IdM, WebRestauranteContext db)
        {
            var MesasO = new List<MesasOcupadas>();
            var Mover = new List<MesasOcupadas>();
            DateTime Fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime hora1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);            
            int H = DateTime.Now.TimeOfDay.Hours;
            H = H + 1;
            DateTime hora2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, H, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            MesasO = db.MesasOcupadas.Where(t => t.Fecha_MesasO == Fecha).ToList();
            MesasO = MesasO.Where(t => (t.HoraIngreso_MesasO <= hora2 && hora1 < t.HoraIngreso_MesasO) || t.HoraSalida_MesasO >= hora1).ToList();
            MesasO = MesasO.Where(t => t.Cod_Mesa == IdM).ToList();
            foreach (var item in MesasO)
            {
                Mover = db.MesasOcupadas.Where(t => t.Fecha_MesasO == Fecha).ToList();
                Mover = Mover.Where(t => (t.HoraIngreso_MesasO <= hora2 && hora1 < t.HoraIngreso_MesasO) || t.HoraSalida_MesasO >= hora1).ToList();
                for (int i = 1; i <= db.Mesas.Count() ; i++)
                {
                  var Mover1 = Mover.Where(t => t.Cod_Mesa == i).ToList();
                    if (Mover1.Count == 0)
                    {
                        MesasOcupadas MO = db.MesasOcupadas.Find(item.Cod_MesasO);
                        MO.Cod_Mesa = i;
                        db.Entry(MO).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        i = db.Mesas.Count();
                    }
                }
            }
        }
    }
}