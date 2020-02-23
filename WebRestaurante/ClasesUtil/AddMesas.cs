using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class AddMesas
    {
        public static List<MesasOcupadas> Lista(Guid IdC, int IdM, WebRestauranteContext db, DateTime fecha, DateTime hora)
        {
            var lista = new List<MesasOcupadas>();   
            foreach (var item in db.Mesas.ToList())
            {                
                var MesasOcupada = db.MesasOcupadas.Where(mo => mo.Cod_Mesa == item.Cod_Mesa).ToList();
                var MesasODate = MesasOcupada.Where(mo => (mo.Fecha_MesasO >= fecha)).ToList();
                var MesasOHora = MesasODate.Where(mo => ((mo.HoraIngreso_MesasO <= hora) || (mo.HoraSalida_MesasO >= hora))).ToList();
                var MesasOcupadas = MesasOHora.Where(mo => mo.Estado_MesasO == true).ToList();
                if (MesasOcupadas.Count > 0)
                {
                    string Mo = MesasOcupadas.Where(t => t.Cod_Mesa == IdM).Select(t => t.Reservada).FirstOrDefault();
                    foreach (var item1 in MesasOcupadas)
                    {
                        
                        if (item1.Reservada != Mo )
                        {
                            if (item1.Llegada_MesasO == true)
                            {
                                var mo = new MesasOcupadas
                                {
                                    Cod_Mesa = item1.Cod_Mesa,
                                    Cod_MesasO = item1.Cod_MesasO,
                                    ConfirmarMesa = item1.ConfirmarMesa,
                                    CPersonas_Mesas = item1.CPersonas_Mesas,
                                    Estado_MesasO = item1.Estado_MesasO,
                                    Fecha_MesasO = item1.Fecha_MesasO,
                                    HoraIngreso_MesasO = item1.HoraIngreso_MesasO,
                                    HoraSalida_MesasO = item1.HoraSalida_MesasO,
                                    Llegada_MesasO = item1.Llegada_MesasO,
                                    Reservada = item1.Reservada
                                };
                                lista.Add(mo);
                            }
                        }                       
                    }
                }
            }
            return lista;
        }
    }
}