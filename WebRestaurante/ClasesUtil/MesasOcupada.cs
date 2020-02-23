using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class MesasOcupada 
    {       
        // verifica si la mesa esta ocupada o no
        public static List<MesasOcupadas> Lista(int codMesa, DateTime Fecha, WebRestauranteContext db, DateTime horaI)
        {
        Desactivar(db); /*Desactivar2(db);*/
        DateTime hora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
        var MesasOcupada = db.MesasOcupadas.Where(mo => mo.Cod_Mesa == codMesa).ToList();
        var MesasODate = MesasOcupada.Where(mo => (mo.Fecha_MesasO >= new DateTime(Fecha.Year, Fecha.Month, Fecha.Day))).ToList();
        var MesasOHora = MesasODate.Where(mo => ((mo.HoraIngreso_MesasO <= horaI) /*|| (mo.HoraSalida_MesasO >= horaI) */)).ToList();
        var MesasOcupadas = MesasOHora.Where(mo => mo.Estado_MesasO == true).ToList();
        return MesasOcupadas;
        }

        // desactiva las mesas que tengan inferior ala actual
        public static void Desactivar(WebRestauranteContext db)
        {
            var MesasOcupada = new List<MesasOcupadas>();
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime Hora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            MesasOcupada = db.MesasOcupadas.Where(t => t.Fecha_MesasO <= fecha).ToList();
            MesasOcupada = MesasOcupada.Where(t => Hora > t.HoraSalida_MesasO).ToList();
            MesasOcupada = MesasOcupada.Where(t => t.Llegada_MesasO == false).ToList();
            MesasOcupada = MesasOcupada.Where(t => t.Estado_MesasO == true).ToList();
            if (MesasOcupada.Count() != 0)
            {
                foreach (var item in MesasOcupada)
                {
                        MesasOcupadas MO = db.MesasOcupadas.Find(item.Cod_MesasO);
                        var Detalle = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == item.Cod_MesasO).ToList();
                        foreach (var item1 in Detalle)
                        {
                            DetalleMesasCliente DMC = db.DetalleMesasCliente.Find(item1.Id_DMC);
                            DMC.Estado_DMC = false;
                            db.Entry(DMC).State = System.Data.Entity.EntityState.Modified;
                        }

                        MO.Estado_MesasO = false;
                        db.Entry(MO).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                  
                }
            }
        }
        //desativar estado dependiendo si a llegado al restaurante o no
        public static void Desactivar2(WebRestauranteContext db)
        {
            var MesasOcupada = new List<MesasOcupadas>();
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime Hora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            MesasOcupada = db.MesasOcupadas.Where(t => t.Fecha_MesasO == fecha).ToList();            
            MesasOcupada = MesasOcupada.Where(t => Hora > t.HoraSalida_MesasO).ToList();
            MesasOcupada = MesasOcupada.Where(t => t.Llegada_MesasO == false).ToList();
            MesasOcupada = MesasOcupada.Where(t => t.Estado_MesasO == true).ToList();
            if (MesasOcupada.Count() != 0)
            {
                foreach (var item in MesasOcupada)
                {
                    MesasOcupadas MO = db.MesasOcupadas.Find(item.Cod_MesasO);
                    var Detalle = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == item.Cod_MesasO).ToList();
                    foreach (var item1 in Detalle)
                    {
                        DetalleMesasCliente DMC = db.DetalleMesasCliente.Find(item1.Id_DMC);
                        DMC.Estado_DMC = false;
                        db.Entry(DMC).State = System.Data.Entity.EntityState.Modified;
                    }

                    MO.Estado_MesasO = false;
                    db.Entry(MO).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
        }
    }
}