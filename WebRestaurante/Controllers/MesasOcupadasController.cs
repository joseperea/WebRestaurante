using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;
using WebRestaurante.ClasesUtil;
using System.Threading.Tasks;
using BotDetect.Web.Mvc;

namespace WebRestaurante.Controllers
{
    public class MesasOcupadasController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();

        // GET: MesasOcupada
        public ActionResult ConReserva(int IdC, int IdM, int IdMesaO, string NCF, int CPersona, int? add)
        {
            var ListMenu = new List<Menu>();
            var Mesas = new List<Mesas>();
            var MesasO = new List<MesasOcupadas>();
            var Detalles = new List<DetalleMesasCliente>();
            var cliente = new Clientes();
            var DetalleViews = new DetalleView();
            int i = 0, idMesas = 0, idMesasO = 0, LMenu = 0, Cmesas = 0, Cmenu = 0;
            decimal Valor = 0;
            string aux = "";
            try
            {
                if (NCF != "")
                {
                     Detalles = db.DetalleMesasCliente.Where(t => t.NConfirmacion_DMC == NCF).ToList();
                    if (Detalles.Count() >= 0)
                    {
                        foreach (var item in Detalles)
                        {
                            if ((item.Cod_Cli == IdC || item.Cod_MesasO == IdMesaO) && item.Estado_DMC == true)
                            {

                                if (idMesas != item.MesasOcupadas.Cod_Mesa)
                                {
                                    Mesas.Add(item.MesasOcupadas.Mesas);
                                    idMesas = item.MesasOcupadas.Cod_Mesa;
                                    Cmesas++;
                                    aux = aux + "-" + "M" + item.MesasOcupadas.Cod_Mesa;
                                }
                                if (idMesasO != item.Cod_MesasO)
                                {
                                    MesasO.Add(item.MesasOcupadas);
                                    idMesasO = item.Cod_MesasO;
                                }
                                if (LMenu != item.Cod_Menu && item.Cantidad_DMC != 0)
                                {
                                    if (CPersona > 4 && i > 0)
                                    {
                                        Cmenu++;
                                    }
                                    else if (CPersona <= 4)
                                    {
                                        Cmesas++;
                                    }
                                    Valor = Valor + (item.Menu.Valor_Menu * item.Cantidad_DMC);
                                    ListMenu.Add(item.Menu);
                                    LMenu = item.Cod_Menu;
                                }                                                  
                                if (i == 0)
                                {
                                    cliente = new Clientes
                                    {
                                        Apellidos_Cli = item.Cliente.Apellidos_Cli,
                                        Cod_Cli = item.Cliente.Cod_Cli,
                                        Correo_Cli = item.Cliente.Correo_Cli,
                                        Nombres_Cli = item.Cliente.Nombres_Cli,
                                        Telefono_Cli = item.Cliente.Telefono_Cli
                                    };
                                    i++;
                                }
                            }
                        }
                    }
                }               
            }
            catch (Exception)
            {

                throw;
            }
            DetalleViews.addMesasO = MesasO;
            DetalleViews.Cliente = cliente;
            DetalleViews.DetalleMesasClientes = Detalles;
            DetalleViews.Menus = ListMenu;
            DetalleViews.Mesas = Mesas;
            DetalleViews.Cpersonas = CPersona;
            DetalleViews.Cmesas = Cmesas;
            DetalleViews.Nmesas = aux;
            DetalleViews.Cmenu = Cmenu;
            DetalleViews.ValorTotal = Valor;
            ViewBag.Model = add;
            return View(DetalleViews);
        }
        public int idDocumentos(int IdCliente, ReservaView ReservaView, string NReserva)
        {
            int hora = ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Hours;
            hora = hora + 2;
            var documento = new Documento
            {
                Cod_Cli = IdCliente,
                Cod_TDoc = 2,
                NConfirmacion_Doc = NReserva,
                Fecha_Doc = ReservaView.MesasOcupadas.Fecha_MesasO,
                HoraIngreso_Doc = ReservaView.MesasOcupadas.HoraIngreso_MesasO,
                HoraSalida_Doc = new DateTime(ReservaView.MesasOcupadas.Fecha_MesasO.Year, ReservaView.MesasOcupadas.Fecha_MesasO.Month, ReservaView.MesasOcupadas.Fecha_MesasO.Day,
                                                  hora, ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Minutes, ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Seconds)
            };
            db.Documento.Add(documento);
            db.SaveChanges();
            int idDocumento = db.Documento.Where(r => r.Cod_Cli == IdCliente).Max(r => r.Id_Doc);
            return idDocumento;
        }
        public ActionResult Index()
        {
            var mesasOcupadas = db.MesasOcupadas
              .Include(m => m.Mesas);
            return View(mesasOcupadas.ToList());
        }

        // GET: MesasOcupadas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MesasOcupadas mesasOcupadas = db.MesasOcupadas.Find(id);
            if (mesasOcupadas == null)
            {
                return HttpNotFound();
            }
            return View(mesasOcupadas);
        }

        // GET: MesasOcupadas/Create
        public ActionResult Create(bool? C)
        {
            ViewBag.CMesas = CantidadMesas.CMesas(db);
            ViewBag.Cod_Mesa = new SelectList(db.Mesas, "Cod_Mesa", "Numero_Mesa");
            var reservaView = new ReservaView();
            reservaView.Clientes = new Clientes();
            reservaView.MesasOcupadas = new MesasOcupadas();
            return View(reservaView);
        }

        // POST: MesasOcupadas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ReservaView ReservaView, string RCM)
        {
            string idMesa = "";
            int IdCliente = 0;
            int codMesas = 0;
            int CMesas = db.Mesas.ToList().Count();
            var addr = new AddR();
            DateTime hora = DateTime.Now;
            if (RCM == "Reservar con menu")
            {
                RCM = Convert.ToString("1");
            }
            else
            {
                RCM = null;
            }
            if (ReservaView.MesasOcupadas.Fecha_MesasO < DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, "Ingrese una fecha mayor o igual ala actual");
                ViewBag.Error = true;
                ViewBag.CMesas = CantidadMesas.CMesas(db);
                return View(ReservaView);
            }
            //if (db.Restaurantes.FirstOrDefault().HoraCierre < TiempoEspera.Salida(ReservaView.MesasOcupadas.HoraIngreso_MesasO, db))
            //{
            //    ModelState.AddModelError(string.Empty, "Ingrese una fecha mayor posterior ala actual, por que el restaurante cierra alas "+ db.Restaurantes.FirstOrDefault().HoraCierre.ToString("hh:mm:ss"));
            //    ViewBag.Error = true;
            //    ViewBag.CMesas = CantidadMesas.CMesas(db);
            //    return View(ReservaView);
            //}
            if (ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Hours <= hora.TimeOfDay.Hours && 
                ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Minutes <= hora.TimeOfDay.Minutes && 
                DateTime.Now.Date == ReservaView.MesasOcupadas.Fecha_MesasO)
            {
                ModelState.AddModelError(string.Empty, "Ingrese una hora mayor ala actual");
                ViewBag.Error = true;
                ViewBag.CMesas = CantidadMesas.CMesas(db);
                return View(ReservaView);
            } 
            if (ReservaView.MesasOcupadas.CPersonas_Mesas <= 0 )
            {
                ModelState.AddModelError(string.Empty, "Ingrese una cantidad de personas mayor a cero");
                ViewBag.Error = true;
                ViewBag.CMesas = CantidadMesas.CMesas(db);
                return View(ReservaView);
            }
            if (ModelState.IsValid)
            {

                using (var transacion = db.Database.BeginTransaction())
                {
                    try
                    {
                        int cantida = CantidadMesas.CMesa(ReservaView.MesasOcupadas.Fecha_MesasO, ReservaView.MesasOcupadas.HoraIngreso_MesasO, db);
                        int i2 = 1;
                        if ((ReservaView.MesasOcupadas.CPersonas_Mesas % 4) == 0)
                        {
                            IdCliente = Cliente.idCliente(ReservaView.Clientes.Correo_Cli, ReservaView.Clientes.Nombres_Cli, ReservaView.Clientes.Apellidos_Cli, ReservaView.Clientes.Telefono_Cli, db);
                            int CRMesas = (ReservaView.MesasOcupadas.CPersonas_Mesas / 4);
                            if (CRMesas > cantida && ReservaView.MesasOcupadas.Fecha_MesasO == DateTime.Now.Date)
                            {
                                ModelState.AddModelError(string.Empty, string.Format("No hay mesas suficiente para hacer su reserva, por favor seleciones una fecha posterior a esta {0} y maximo de {1} persona", ReservaView.MesasOcupadas.Fecha_MesasO.ToString("dd/MM/yyyy"), (CMesas * 4)));
                                ViewBag.Error = true;
                                ViewBag.CMesas = CantidadMesas.CMesas(db);
                                return View(ReservaView);
                            }
                            else if (CRMesas > cantida && ReservaView.MesasOcupadas.Fecha_MesasO > DateTime.Now.Date)
                            {
                                ModelState.AddModelError(string.Empty, string.Format("No hay mesas suficiente para hacer su reserva, maximo de {0}", (CMesas*4)));
                                ViewBag.Error = true;
                                ViewBag.CMesas = CantidadMesas.CMesas(db);
                                return View(ReservaView);
                            }
                            for (int i = 1; i <= CMesas; i++)
                            {
                                if (i2 <= CRMesas)
                                {
                                    var Nmesa = ("M " + i);
                                    var mesa = db.Mesas.Where(m => m.Numero_Mesa == Nmesa).ToList();
                                    foreach (var item in mesa)
                                    {                                        
                                        int valor = MesasOcupada.Lista(item.Cod_Mesa, ReservaView.MesasOcupadas.Fecha_MesasO, db, ReservaView.MesasOcupadas.HoraIngreso_MesasO).Count();
                                        if (valor == 0)
                                        {
                                            //int HoraS = ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Hours;
                                            //HoraS = HoraS + 2;
                                            var mesasO = new MesasOcupadas
                                            {
                                                Cod_Mesa = item.Cod_Mesa,
                                                Estado_MesasO = true,
                                                ConfirmarMesa = false,
                                                Reservada = "PaginaWeb",
                                                CPersonas_Mesas = ReservaView.MesasOcupadas.CPersonas_Mesas,
                                                Fecha_MesasO = ReservaView.MesasOcupadas.Fecha_MesasO,
                                                HoraIngreso_MesasO = ReservaView.MesasOcupadas.HoraIngreso_MesasO,
                                                HoraSalida_MesasO = TiempoEspera.Salida(ReservaView.MesasOcupadas.HoraIngreso_MesasO, db)
                                                //HoraSalida_MesasO = new DateTime(ReservaView.MesasOcupadas.HoraIngreso_MesasO.Year, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Month, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Day,
                                                //                                  HoraS, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Minute, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Second)
                                            };
                                            db.MesasOcupadas.Add(mesasO);
                                            db.SaveChanges();
                                            codMesas = MesasOcupada.Lista(item.Cod_Mesa, ReservaView.MesasOcupadas.Fecha_MesasO, db, ReservaView.MesasOcupadas.HoraIngreso_MesasO).Max(mo => mo.Cod_MesasO);
                                            idMesa = codMesas + "," + idMesa;
                                            i2++;
                                        }

                                    }
                                }
                                else
                                {
                                    i = CMesas;
                                }
                            }
                        }
                        else
                        {
                            IdCliente = Cliente.idCliente(ReservaView.Clientes.Correo_Cli, ReservaView.Clientes.Nombres_Cli, ReservaView.Clientes.Apellidos_Cli, ReservaView.Clientes.Telefono_Cli, db);
                            int CRMesas = (ReservaView.MesasOcupadas.CPersonas_Mesas / 4) + 1;
                            if (CRMesas >= cantida && ReservaView.MesasOcupadas.Fecha_MesasO == DateTime.Now.Date)
                            {
                                ModelState.AddModelError(string.Empty, string.Format("No hay mesas suficiente para hacer su reserva, por favor seleciones una fecha posterior a esta {0}", ReservaView.MesasOcupadas.Fecha_MesasO.ToString("dd/MM/yyyy")));
                                ViewBag.Error = true;
                                ViewBag.CMesas = CantidadMesas.CMesas(db);
                                return View(ReservaView);
                            }
                            else if (CRMesas > cantida && ReservaView.MesasOcupadas.Fecha_MesasO > DateTime.Now.Date)
                            {
                                ModelState.AddModelError(string.Empty, string.Format("No hay mesas suficiente para hacer su reserva, maximo de {0} personas", (CMesas * 4)));
                                ViewBag.Error = true;
                                ViewBag.CMesas = CantidadMesas.CMesas(db);
                                return View(ReservaView);
                            }
                            for (int i = 1; i <= CMesas; i++)
                            {
                                if (i2 <= CRMesas)
                                {
                                    var Nmesa = ("M " + i);
                                    var mesa = db.Mesas.Where(m => m.Numero_Mesa == Nmesa).ToList();
                                    foreach (var item in mesa)
                                    {
                                        int codMesa = item.Cod_Mesa;
                                        int valor = MesasOcupada.Lista(item.Cod_Mesa, ReservaView.MesasOcupadas.Fecha_MesasO, db, ReservaView.MesasOcupadas.HoraIngreso_MesasO).Count();
                                        if (valor == 0)
                                        {
                                            //int HoraS = ReservaView.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.Hours;
                                            //HoraS = HoraS + 2;
                                            var mesasO = new MesasOcupadas
                                            {
                                                Cod_Mesa = item.Cod_Mesa,
                                                Estado_MesasO = true,
                                                ConfirmarMesa = false,
                                                Reservada = "PaginaWeb",
                                                CPersonas_Mesas = ReservaView.MesasOcupadas.CPersonas_Mesas,
                                                Fecha_MesasO = ReservaView.MesasOcupadas.Fecha_MesasO,
                                                HoraIngreso_MesasO = ReservaView.MesasOcupadas.HoraIngreso_MesasO,
                                                HoraSalida_MesasO = TiempoEspera.Salida(ReservaView.MesasOcupadas.HoraIngreso_MesasO, db)
                                                //HoraSalida_MesasO = new DateTime(ReservaView.MesasOcupadas.HoraIngreso_MesasO.Year, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Month, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Day,
                                                //                                  HoraS, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Minute, ReservaView.MesasOcupadas.HoraIngreso_MesasO.Second)
                                            };
                                            db.MesasOcupadas.Add(mesasO);
                                            db.SaveChanges();
                                            codMesas = MesasOcupada.Lista(item.Cod_Mesa, ReservaView.MesasOcupadas.Fecha_MesasO, db, ReservaView.MesasOcupadas.HoraIngreso_MesasO).Max(mo => mo.Cod_MesasO);
                                            idMesa = codMesas + "," + idMesa;
                                            i2++;
                                        }

                                    }
                                }
                                else
                                {
                                    i = CMesas;
                                }
                            }
                        }
                        string[] MenuCantidad = new string[db.Menus.Count()];
                        int IdMesasO = 0;
                        string ramdo = Ramdon.Numero();
                        Reserva.DetalleMenuCliente(IdCliente, "," + idMesa, "PaginaWeb-" + ramdo, db, SeparadorMenu.Menu(MenuCantidad, MenuCantidad), IdMesasO, null);
                        addr = new AddR
                        {
                            add = RCM,
                            CPersona = ReservaView.MesasOcupadas.CPersonas_Mesas.ToString(),
                            IdC = IdCliente,
                            IdM = codMesas,
                            IdMesaO = codMesas,
                            NCF = "PaginaWeb-" + ramdo

                        };
                        if (RCM == null)
                        {
                           await SendEmail.ConfirmarReserva(IdCliente, "PaginaWeb-" + ramdo, db);
                            transacion.Commit();
                        }                                               
                        else
                        {
                            transacion.Commit();               
                            return RedirectToAction("MenuVista","Menus", addr);
                        }
                        return RedirectToAction("ConReserva", "MesasOcupadas", addr);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.CMesas = CantidadMesas.CMesas(db);
                        transacion.Rollback();
                        ModelState.AddModelError(string.Empty, "Error al Reservar "+ ex.Message);
                        ViewBag.Error = true;
                        return View(ReservaView);
                    }
                }

            }
            ViewBag.CMesas = CantidadMesas.CMesas(db);
            return View(ReservaView);
        }

        // GET: MesasOcupadas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MesasOcupadas mesasOcupadas = db.MesasOcupadas.Find(id);
            if (mesasOcupadas == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cod_Mesa = new SelectList(db.Mesas, "Cod_Mesa", "Numero_Mesa", mesasOcupadas.Cod_Mesa);
            return View(mesasOcupadas);
        }

        // POST: MesasOcupadas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MesasOcupadas mesasOcupadas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mesasOcupadas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cod_Mesa = new SelectList(db.Mesas, "Cod_Mesa", "Numero_Mesa", mesasOcupadas.Cod_Mesa);
            return View(mesasOcupadas);
        }

        // GET: MesasOcupadas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MesasOcupadas mesasOcupadas = db.MesasOcupadas.Find(id);
            if (mesasOcupadas == null)
            {
                return HttpNotFound();
            }
            return View(mesasOcupadas);
        }

        // POST: MesasOcupadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MesasOcupadas mesasOcupadas = db.MesasOcupadas.Find(id);
            db.MesasOcupadas.Remove(mesasOcupadas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
