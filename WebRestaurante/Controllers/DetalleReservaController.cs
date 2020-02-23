using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;
using PagedList;
using System.Net;

namespace WebRestaurante.Controllers
{
    public class DetalleReservaController : Controller
    {
        WebRestauranteContext db = new WebRestauranteContext();

        public ActionResult Desactivar(int idD, int? page)
        {
            page = (page ?? 1);
            var reservas = new List<ListadoReserva>();
            int id = 0;
            var mesaso = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idD).ToList();
            foreach (var item in mesaso)
            {
                MesasOcupadas mesa = db.MesasOcupadas.Find(item.Cod_MesasO);
                mesa.Estado_MesasO = false;
                db.Entry(mesa).State = EntityState.Modified;
                db.SaveChanges();

            }
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var Fmesasocupada = db.MesasOcupadas.Where(mo => (mo.Fecha_MesasO == fecha)).ToList();
            var Hmesasocupada = Fmesasocupada.Where(mo => ((new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds) >= mo.HoraIngreso_MesasO) ||
                                                   (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds) <= mo.HoraSalida_MesasO))).ToList();
            var Fmesasocupadas = db.MesasOcupadas.Where(mo => (mo.Fecha_MesasO > fecha && mo.Estado_MesasO == true)).ToList();
            var Filtro = Hmesasocupada.Where(mo => mo.Estado_MesasO == true).ToList();
            foreach (var item in Filtro)
            {
                var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                int idd = idreserva.FirstOrDefault().Cod_Cli;
                var cliente = db.Clientes.Find(idd);
                if (idreserva.FirstOrDefault().Cod_Cli != id)
                {
                    id = idreserva.FirstOrDefault().Cod_Cli;
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idd).Count();
                    string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                    var reserva = new ListadoReserva
                    {
                        Nombre = Nombre,
                        Cpersonas = item.CPersonas_Mesas,
                        Cmesas = Creserva,
                        Fecha = item.Fecha_MesasO,
                        HoraI = item.HoraIngreso_MesasO,
                        Confirmacion = item.ConfirmarMesa,
                        IdCliente = idd
                    };
                    reservas.Add(reserva);
                }

            }
            foreach (var item in Fmesasocupadas)
            {
                var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                int idd = idreserva.FirstOrDefault().Cod_Cli;
                var cliente = db.Clientes.Find(idd);
                if (idreserva.FirstOrDefault().Cod_Cli != id)
                {
                    id = idreserva.FirstOrDefault().Cod_Cli;
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idd).Count();
                    string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                    var reserva = new ListadoReserva
                    {
                        Nombre = Nombre,
                        Cpersonas = item.CPersonas_Mesas,
                        Cmesas = Creserva,
                        Fecha = item.Fecha_MesasO,
                        HoraI = item.HoraIngreso_MesasO,
                        Confirmacion = item.ConfirmarMesa,
                        IdCliente = idd
                    };
                    reservas.Add(reserva);
                }

            }
            var Reserva = reservas.OrderBy(r => r.Fecha).ToList();
            return View("ListadoReserva",Reserva.ToPagedList((int)page, 5));
        }
        public ActionResult Detalle(int id)
        {
            var detalle = new DetalleView();
            int i = 0;
            var idmesasO = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == id).ToList();
            foreach (var item in idmesasO)
            {
                if (item.Cod_Cli != i)
                {
                    i = item.Cod_Cli;
                    var mesaso = db.MesasOcupadas.Find(item.Cod_MesasO); 
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == id).Count();
                    detalle.Cmesas = Creserva;
                    detalle.Cpersonas = mesaso.CPersonas_Mesas;
                    detalle.Confirmacion = mesaso.ConfirmarMesa;
                    detalle.Fecha = mesaso.Fecha_MesasO;
                    detalle.HoraI = mesaso.HoraIngreso_MesasO;
                    detalle.Horas = mesaso.HoraSalida_MesasO;
                    detalle.IdReserva = id;

                }
            }
            
            detalle.Cliente = db.Clientes.Find(idmesasO.Max(r => r.Cod_Cli));
            return View(detalle);
        }
        public ActionResult DetalleReserva(string Idr, int Idc)
        {
            var DetalleReserva = new DetalleReserva();
            int CMesas = 0;
            var Idreserva = db.DetalleMesasCliente.Where(r => r.Cod_Cli == Idc && r.NConfirmacion_DMC == Idr).ToList();
            foreach (var item in Idreserva)
            {
                MesasOcupadas mesasOcupadas = db.MesasOcupadas.Find(item.Cod_MesasO);
                if (mesasOcupadas.ConfirmarMesa == true)
                {
                    return HttpNotFound();
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                mesasOcupadas.ConfirmarMesa = true;
                db.Entry(mesasOcupadas).State = EntityState.Modified;
                db.SaveChanges();
                DetalleReserva.MesasOcupada = mesasOcupadas;
                CMesas++;
            }
            ViewBag.CMesas = CMesas;                       
            DetalleReserva.Cliente = db.Clientes.Find(Idc);            
            return View(DetalleReserva);
        }

        [HttpPost]
        public ActionResult ListadoReserva(int? page, ListadoReserva lista)
        {
            page = (page ?? 1);
            ViewBag.CurrentFiltre = lista.Fecha;
            var reservas = new List<ListadoReserva>();
            int id = 0;
            DateTime horaI = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            DateTime horaS = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            if (lista.Fecha >= DateTime.Now.Date)
            {
                var filtro = db.MesasOcupadas.Where(mo => mo.Fecha_MesasO == lista.Fecha).OrderBy(mo => mo.Fecha_MesasO).ToList();
                if (filtro.Count == 0)
                {
                    ViewBag.Error = string.Format("No se ha encontrado reserva de esta fecha ({0})", lista.Fecha.ToString("dd/MM/yyyy"));
                }
            var Hmesasocupadas = filtro.Where(mo => (mo.HoraIngreso_MesasO >= horaI) || (mo.HoraSalida_MesasO >= horaS)).ToList();
            var Filtros = Hmesasocupadas.Where(mo => mo.Estado_MesasO == true).ToList();
                foreach (var item in Filtros)
                {
                    var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                    int idr = idreserva.FirstOrDefault().Cod_Cli;
                    var cliente = db.Clientes.Find(idr);
                    if (idreserva.FirstOrDefault().Cod_Cli != id)
                    {
                        id = idreserva.FirstOrDefault().Cod_Cli;
                        var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idr).Count();
                        string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                        var reserva = new ListadoReserva
                        {
                            Nombre = Nombre,
                            Cpersonas = item.CPersonas_Mesas,
                            Cmesas = Creserva,
                            Fecha = item.Fecha_MesasO,
                            HoraI = item.HoraIngreso_MesasO,
                            Confirmacion = item.ConfirmarMesa,
                            IdCliente = idr
                        };
                        reservas.Add(reserva);
                    }
                }
                var Reserva1 = reservas.OrderBy(r => r.Fecha).ToList();
                return View(Reserva1.ToPagedList((int)page, 5));
            }
            if (lista.Fecha != Convert.ToDateTime("01/01/0001 0:00:00"))
            {
                ViewBag.Error = string.Format("({0}) Esta fecha ya paso, no se encontrara reservas", lista.Fecha.ToString("dd/MM/yyyy"));
            }            
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var Fmesasocupada = db.MesasOcupadas.Where(mo => (mo.Fecha_MesasO == fecha)).ToList();
            var Hmesasocupada = Fmesasocupada.Where(mo => (mo.HoraIngreso_MesasO >= horaI) || (mo.HoraSalida_MesasO >= horaS)).ToList();
            var Fmesasocupadas = db.MesasOcupadas.Where(mo => (mo.Fecha_MesasO > fecha && mo.Estado_MesasO == true)).ToList();
            var Filtro = Hmesasocupada.Where(mo => mo.Estado_MesasO == true).ToList();

            foreach (var item in Filtro)
            {
                var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                int idr = idreserva.FirstOrDefault().Cod_Cli;
                var cliente = db.Clientes.Find(idr);
                if (idreserva.FirstOrDefault().Cod_Cli != id)
                {
                    id = idreserva.FirstOrDefault().Cod_Cli;
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idr).Count();
                    string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                    var reserva = new ListadoReserva
                    {
                        Nombre = Nombre,
                        Cpersonas = item.CPersonas_Mesas,
                        Cmesas = Creserva,
                        Fecha = item.Fecha_MesasO,
                        HoraI = item.HoraIngreso_MesasO,
                        Confirmacion = item.ConfirmarMesa,
                        IdCliente = idr
                    };
                    reservas.Add(reserva);
                }

            }
            foreach (var item in Fmesasocupadas)
            {
                var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                int idr = idreserva.FirstOrDefault().Cod_Cli;
                var cliente = db.Clientes.Find(idr);
                if (idreserva.FirstOrDefault().Cod_Cli != id)
                {
                    id = idreserva.FirstOrDefault().Cod_Cli;
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idr).Count();
                    string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                    var reserva = new ListadoReserva
                    {
                        Nombre = Nombre,
                        Cpersonas = item.CPersonas_Mesas,
                        Cmesas = Creserva,
                        Fecha = item.Fecha_MesasO,
                        HoraI = item.HoraIngreso_MesasO,
                        Confirmacion = item.ConfirmarMesa,
                        IdCliente = idr
                    };
                    reservas.Add(reserva);
                }

            }
            var Reserva = reservas.OrderBy(r => r.Fecha).ToList();
            return View(Reserva.ToPagedList((int)page, 5));
        }

        public ActionResult ListadoReserva(int? page)
        {
            page = (page ?? 1);
            var reservas = new List<ListadoReserva>();
            int id = 0;
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime horaI = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            DateTime horaS = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);


            var Fmesasocupada = db.MesasOcupadas.Where(mo => (mo.Fecha_MesasO == fecha)).ToList();
            var Hmesasocupada = Fmesasocupada.Where(mo => ( mo.HoraIngreso_MesasO >= horaI) || (mo.HoraSalida_MesasO >= horaS)).ToList();


            var Fmesasocupadas = db.MesasOcupadas.Where(mo => (mo.Fecha_MesasO > fecha && mo.Estado_MesasO == true)).ToList();           
            var Filtro = Hmesasocupada.Where(mo => mo.Estado_MesasO == true).ToList();
            foreach (var item in Filtro)
            {
                var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                int idr = idreserva.FirstOrDefault().Cod_Cli;
                var cliente = db.Clientes.Find(idr);
                if (idreserva.FirstOrDefault().Cod_Cli != id)
                {
                    id = idreserva.FirstOrDefault().Cod_Cli;
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idr).Count();
                    string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                    var reserva = new ListadoReserva
                    {
                        Nombre = Nombre,
                        Cpersonas = item.CPersonas_Mesas,
                        Cmesas = Creserva,
                        Fecha = item.Fecha_MesasO, 
                        HoraI = item.HoraIngreso_MesasO, 
                        Confirmacion = item.ConfirmarMesa, 
                        IdCliente = idr
                    };
                    reservas.Add(reserva);
                }
                
            }
            foreach (var item in Fmesasocupadas)
            {
                var idreserva = db.DetalleMesasCliente.Where(drm => drm.Cod_MesasO == item.Cod_MesasO).ToList();
                int idr = idreserva.FirstOrDefault().Cod_Cli;
                var cliente = db.Clientes.Find(idr);
                if (idreserva.FirstOrDefault().Cod_Cli != id)
                {
                    id = idreserva.FirstOrDefault().Cod_Cli;
                    var Creserva = db.DetalleMesasCliente.Where(drm => drm.Cod_Cli == idr).Count();
                    string Nombre = string.Format("{0} {1}", cliente.Nombres_Cli, cliente.Apellidos_Cli);
                    var reserva = new ListadoReserva
                    {
                        Nombre = Nombre,
                        Cpersonas = item.CPersonas_Mesas,
                        Cmesas = Creserva,
                        Fecha = item.Fecha_MesasO,
                        HoraI = item.HoraIngreso_MesasO,
                        Confirmacion = item.ConfirmarMesa,
                        IdCliente = idr
                    };
                    reservas.Add(reserva);
                }

            }
            var Reserva = reservas.OrderBy(r => r.Fecha).ToList();
            return View(Reserva.ToPagedList((int)page, 5));
        }
        
    }
}