using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;
using System.Data.Entity;

namespace WebRestaurante.ClasesUtil
{
    public class Reserva
    {
        public static void DetalleMenuCliente(Guid IdCliente, string idMesas, string Nconfirma, WebRestauranteContext db, string[] MenuCantidad, int? IdMesaO, bool? TR)
        {
            string[] idMesa = idMesas.Split(new char[] { ',' });
            foreach (var item in idMesa)
            {
                if (item != "")
                {
                    if (IdMesaO==0)
                    {
                        if (MenuCantidad!=null)
                        {
                            foreach (var item1 in MenuCantidad)
                            {
                                string[] id = item1.Split(new char[] { ',' });
                                if (id[0] != "")
                                {
                                    int cod = Convert.ToInt32(item);

                                    var detalle = new DetalleMesasCliente
                                    {
                                        Cod_MesasO = cod,
                                        Cod_Cli = IdCliente,
                                        NConfirmacion_DMC = Nconfirma,
                                        Cantidad_DMC = Convert.ToInt32(id[1]),
                                        Cod_Menu = Convert.ToInt32(id[0]),
                                        Estado_DMC = true,
                                        Cod_TDoc = 1,
                                        PedidoM = true
                                    };
                                    db.DetalleMesasCliente.Add(detalle);
                                    MesasOcupadas x = db.MesasOcupadas.Find(cod);
                                    x.Reservada = Nconfirma;
                                    x.Llegada_MesasO = true;
                                    db.Entry(x).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            int cod = Convert.ToInt32(item);

                            var detalle = new DetalleMesasCliente
                            {
                                Cod_MesasO = cod,
                                Cod_Cli = IdCliente,
                                NConfirmacion_DMC = Nconfirma,
                                Cantidad_DMC = Convert.ToInt32(0),
                                Cod_Menu = Convert.ToInt32(1),
                                Estado_DMC = true,
                                Cod_TDoc = 1,
                                PedidoM = false                                
                            };
                            db.DetalleMesasCliente.Add(detalle);
                            MesasOcupadas x = db.MesasOcupadas.Find(cod);
                            x.Reservada = Nconfirma;
                            db.Entry(x).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        int i = 0;
                        int id_DMC=0;
                        var CMC = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == IdMesaO).ToList();
                        foreach (var iten in CMC)
                        {
                            var D = db.DetalleMesasCliente.Find(iten.Id_DMC);
                            id_DMC = iten.Id_DMC;
                            db.DetalleMesasCliente.Remove(D);
                            db.SaveChanges();
                        }
                        if (MenuCantidad != null)
                        {
                            var R = db.DetalleMesasCliente.Where(T => T.Id_DMC == id_DMC).ToList();
                            foreach (var item1 in MenuCantidad)
                            {
                                string[] id = item1.Split(new char[] { ',' });
                                if (id[0] != "")
                                {
                                    int cod = Convert.ToInt32(item);

                                    var detalle = new DetalleMesasCliente
                                    {
                                        Cod_MesasO = cod,
                                        Cod_Cli = IdCliente,
                                        NConfirmacion_DMC = Nconfirma,
                                        Cantidad_DMC = Convert.ToInt32(id[1]),
                                        Cod_Menu = Convert.ToInt32(id[0]),
                                        Estado_DMC = true,
                                        Cod_TDoc = 1,
                                        PedidoM = true

                                    };
                                    db.DetalleMesasCliente.Add(detalle);
                                    if (i==0)
                                    {
                                        var mo = db.MesasOcupadas.Where(t => t.Cod_MesasO == cod || t.Reservada == Nconfirma).ToList();
                                        foreach (var item2 in mo)
                                        {
                                            MesasOcupadas x = db.MesasOcupadas.Find(item2.Cod_MesasO);
                                            x.Reservada = Nconfirma;
                                            if (TR != true)
                                            {
                                                x.Llegada_MesasO = true;
                                            }                                                                                        
                                            db.Entry(x).State = EntityState.Modified;
                                            db.SaveChanges();
                                            i++;
                                        }
                                    }                                    
                                }
                            }
                            db.SaveChanges();
                        }
                        else
                        {
                            int cod = Convert.ToInt32(item);

                            var detalle = new DetalleMesasCliente
                            {
                                Cod_MesasO = cod,
                                Cod_Cli = IdCliente,
                                NConfirmacion_DMC = Nconfirma,
                                Cantidad_DMC = Convert.ToInt32(0),
                                Cod_Menu = Convert.ToInt32(1),
                                Estado_DMC = true,
                                Cod_TDoc = 1,
                                PedidoM = false
                            };
                            db.DetalleMesasCliente.Add(detalle);
                            MesasOcupadas x = db.MesasOcupadas.Find(cod);
                            x.Reservada = Nconfirma;
                            db.Entry(x).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }                                       
                }
            }

        }
    }
}