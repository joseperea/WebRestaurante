using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class AddMenus 
    {
        public static bool ingresar(WebRestauranteContext db, int IdM, Guid IdC, string[] selectedMenu, string[] DCantida, int? IdMesaO, string CPersona, bool? TR)
        {
            int horaS = DateTime.Now.TimeOfDay.Hours;
            horaS = horaS + 2;
            if (IdMesaO != 0)
            {
                var MO = db.MesasOcupadas.Find(IdMesaO);                
                Reserva.DetalleMenuCliente(IdC, "," + MO.Cod_MesasO, MO.Reservada, db, SeparadorMenu.Menu(selectedMenu, DCantida), IdMesaO, TR);
                return true;
            }
            else
            {
                Mover.Mesas(IdM, db);
                int A = DateTime.Now.Year;
                var mesasO = new MesasOcupadas
                {
                    Cod_Mesa = IdM,
                    Estado_MesasO = true,
                    ConfirmarMesa = true,
                    CPersonas_Mesas = Convert.ToInt32(CPersona),
                    Fecha_MesasO = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    HoraIngreso_MesasO = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds),
                    HoraSalida_MesasO = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, horaS, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds),
                    Llegada_MesasO = true,
                    Reservada = "Restaurante"
                };
                db.MesasOcupadas.Add(mesasO);
                db.SaveChanges();
                var codMesas = MesasOcupada.Lista(IdM, mesasO.Fecha_MesasO, db, mesasO.HoraIngreso_MesasO).Max(mo => mo.Cod_MesasO);
                Reserva.DetalleMenuCliente(IdC, "," + codMesas, "Restaurante-" + Ramdon.Numero(), db, SeparadorMenu.Menu(selectedMenu, DCantida), IdMesaO, TR);
                return true;
            }
        }
    }
}