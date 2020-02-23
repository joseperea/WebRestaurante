using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebRestaurante.Models;
using PagedList;
using WebRestaurante.ModelsViews;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Threading.Tasks;

namespace WebRestaurante.Controllers
{
    public class MenusController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();

        public async Task<ActionResult> AñadirMenus(Guid? IdC , int? IdM, int? IdMesaO, string[] selectedMenu, string[] DCantida, string CPersona, string NCF, string add, int? TMenu)
        {
            var addr = new AddR
            {
                IdC = IdC.Value,
                IdM = IdM.Value,
                IdMesaO = IdMesaO.Value,
                CPersona = CPersona,
                NCF = NCF,
                add = add,
                TMenu = TMenu
            };
            using (var transacion = db.Database.BeginTransaction())
            {
                try
                {
                    if (TMenu != null)
                    {
                        ClasesUtil.Cache.cargar(selectedMenu,DCantida);
                        return RedirectToAction("MenuVista", addr);
                    }
                    else
                    {
                        if (IdC != null && IdM != null)
                        {
                            var ramdon = db.MesasOcupadas.Find(IdMesaO);
                            if (ClasesUtil.AddMenus.ingresar(db, IdM.Value, IdC.Value, selectedMenu, DCantida, IdMesaO, CPersona, true))
                            {
                                var lista = db.MesasOcupadas.Where(t => t.Estado_MesasO == ramdon.Estado_MesasO && t.Reservada == ramdon.Reservada &&
                                   t.HoraIngreso_MesasO == ramdon.HoraIngreso_MesasO && t.HoraSalida_MesasO == ramdon.HoraSalida_MesasO &&
                                   t.Fecha_MesasO == ramdon.Fecha_MesasO && t.CPersonas_Mesas == ramdon.CPersonas_Mesas).ToList();
                                foreach (var item in lista)
                                {
                                    int iddmc = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == item.Cod_MesasO).Max(t => t.Id_DMC);
                                    var DMC = db.DetalleMesasCliente.Find(iddmc);
                                    DMC.PedidoM = true;
                                    db.Entry(DMC).State = EntityState.Modified;                                    
                                }
                                db.SaveChanges();
                            }     
                            await ClasesUtil.SendEmail.ConfirmarReserva(IdC.Value, ramdon.Reservada, db);
                            transacion.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ViewBag.Error = true;
                }
            }
            return RedirectToAction("ConReserva","MesasOcupadas", addr);
        }

        public ActionResult ImagenMenu(int idm, int? page, int? id)
        {
            page = (page ?? 1);
            var menu = db.Menus.Where(m => m.Cod_Menu == idm).FirstOrDefault();
            ViewBag.file = File(menu.Imagen_Menu, "image/jpeg");
            if (id != null)
            {
                var MenuVistas = db.TipoMenus.Where(tm => tm.Cod_TMenu == id).OrderBy(tm => tm.Nombre_TMenu).ToList();
                var tipomenus = db.TipoMenus.ToList();
                ViewBag.Cod_TMenu = tipomenus;
                return View("MenuVista", MenuVistas.ToPagedList((int)page, 1));
            }
            var MenuVista = db.TipoMenus.OrderBy(tm => tm.Nombre_TMenu).ToList();            
            var tipomenu = db.TipoMenus.ToList();
            ViewBag.Cod_TMenu = tipomenu;
            return View("MenuVista", MenuVista.ToPagedList((int)page, 1));
        }

        public ActionResult BuscarTMenu(int id, int? page, string add, string CPersona, int? IdMesaO, Guid? IdC, int? IdM, string[] selectedMenu, string[] DCantida)
        {
            page = (page ?? 1);
            ViewBag.idTmenu = id;
            AddR addr = new AddR();
            var MenuVista = db.TipoMenus.Where(tm => tm.Cod_TMenu == id).OrderBy(tm => tm.Nombre_TMenu).ToList();
            var tipomenu = db.TipoMenus.ToList();
            if (add != null)
            {
                addr.add = add;
                addr.CPersona = CPersona;
                addr.IdC = IdC.Value;
                addr.IdMesaO = IdMesaO.Value;
                addr.IdM = IdM.Value;
                ViewBag.add = addr;
                ViewBag.Cod_TMenu = tipomenu;
                ClasesUtil.AddMenus.ingresar(db, IdM.Value, IdC.Value, selectedMenu, DCantida, IdMesaO, CPersona, null);
            }
            else
            {
                ViewBag.add = addr;
                ViewBag.Cod_TMenu = tipomenu;
            }

            return View("MenuVista", MenuVista.ToPagedList((int)page, 1));
        }

        public ActionResult MenuVista(AddR Length)
        {
            if (Convert.ToInt32(Length.add) >= 2)
            {
                bool C = false;
                var RCancelar = db.DetalleMesasCliente.Where(t => t.NConfirmacion_DMC == Length.NCF).ToList();
                if (RCancelar.Count() > 0)
                {
                    foreach (var item in RCancelar)
                    {
                        if (Length.IdMesaO == item.Cod_MesasO)
                        {
                            C = true;
                        }
                    }
                    if (C == true)
                    {
                        foreach (var item in RCancelar)
                        {
                            var R = db.DetalleMesasCliente.Find(item.Id_DMC);
                            var RC = db.MesasOcupadas.Find(item.Cod_MesasO);
                            db.DetalleMesasCliente.Remove(R);
                            db.MesasOcupadas.Remove(RC);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Create","MesasOcupadas");
            }
            if (Length.TMenu != null)
            {
                if (Length.TMenu != 0)
                {
                    ViewBag.MenuVista = Length.TMenu;
                    var Nombre = db.TipoMenus.Find(Length.TMenu);
                    ViewBag.Nmenu = Nombre.Nombre_TMenu;
                }
                string[] selectedMenu = ClasesUtil.Cache.selectedMenu();
                string[] DCantida = ClasesUtil.Cache.DCantida();
                string[] separados = ClasesUtil.SeparadorMenu.Menu(selectedMenu, DCantida);
                ClasesUtil.Cache.cargar(null, null);
                ViewBag.Cache = separados;
            }
            var MenuVista = db.TipoMenus.Where(tm => tm.Estado_TMenu==true).OrderBy(tm => tm.Nombre_TMenu).ToList();
            int menu = db.Menus.Where(tm => tm.Estado_Menu == true).Count();          
            ViewBag.Menu = menu;
            ViewBag.add = Length;
            ViewBag.CMenu = menu;
            return View(MenuVista);
        }

        public ActionResult DeletDiaMenu(int Diaid, int Menuid, bool? E)
        {
            var diamenu = db.DayMenu.SingleOrDefault(md => md.Cod_Dia == Diaid && md.Cod_Menu == Menuid);
            db.DayMenu.Remove(diamenu);
            db.SaveChanges();
            var menu = db.Menus.Find(Menuid);
            var ListDia = db.Dias.ToList();
            ListDia.Add(new Dias { Cod_Dia = 0, Nombre_Dia = "Seleccione el Dia" });
            ViewBag.DiasId = new SelectList(ListDia.OrderBy(d => d.Cod_Dia).ToList(), "Cod_Dia", "Nombre_Dia");
            var dias = new List<Dias>();
            var menudia = db.DayMenu.Where(md => md.Cod_Menu == Menuid).ToList();
            if (menudia != null)
            {
                foreach (var item in menudia)
                {
                    var dia = new Dias
                    {
                        Cod_Dia = item.Cod_Dia,
                        Nombre_Dia = item.Dias.Nombre_Dia
                    };
                    dias.Add(dia);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No tiene Dias Asignados");
                ViewBag.Error = true;
            }
            var diaMenuView = new DiaMenuView();
            diaMenuView.Menu = menu;
            diaMenuView.Dia = new Dias();
            diaMenuView.Dias = dias.OrderBy(d => d.Cod_Dia).ToList();
            if (E == null)
            {
                return View("AddDiaMenu", diaMenuView);
            }
            else
            {
                return RedirectToAction(string.Format("Edit/{0}", Menuid));                
            }         
        }
        public ActionResult AddDiaMenu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult( HttpStatusCode.BadRequest);
            }
            var menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            var dias = new List<Dias>();
            var menudia = db.DayMenu.Where(md => md.Cod_Menu == id).ToList();
            if (menudia != null)
            {
                foreach (var item in menudia)
                {
                    var dia = new Dias
                    {
                        Cod_Dia = item.Cod_Dia,
                        Nombre_Dia = item.Dias.Nombre_Dia
                    };
                    dias.Add(dia);
                }             
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No tiene Dias Asignados");
                ViewBag.Error = true;
            }            
            var ListDia = db.Dias.ToList();
            ListDia.Add(new Dias { Cod_Dia = 0, Nombre_Dia = "Seleccione el Dia" });
            ViewBag.DiasId = new SelectList(ListDia.OrderBy(d => d.Cod_Dia).ToList(), "Cod_Dia", "Nombre_Dia");
            var diaMenuView = new DiaMenuView();
            diaMenuView.Menu = menu;
            diaMenuView.Dia = new Dias();
            diaMenuView.Dias = dias.OrderBy(d => d.Cod_Dia).ToList();             
            return View(diaMenuView);
        }

        [HttpPost]
        public ActionResult AddDiaMenu(int id)
        {
            var dias = new List<Dias>();
            var menudia = db.DayMenu.Where(md => md.Cod_Menu == id).ToList();
            if (menudia != null)
            {
                foreach (var item in menudia)
                {
                    var dia = new Dias
                    {
                        Cod_Dia = item.Cod_Dia,
                        Nombre_Dia = item.Dias.Nombre_Dia
                    };
                    dias.Add(dia);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No tiene Dias Asignados");
                ViewBag.Error = true;
            }
            var ListDia = db.Dias.ToList();
            var menu = db.Menus.Find(id);
            var diaMenuView = new DiaMenuView();
            int diaid = Convert.ToInt32(Request["DiasId"]);
            if (diaid == 0)
            {
                ModelState.AddModelError(string.Empty, "Por favor seleccione el dia");
                ViewBag.Error = true;
            }
            var daymenu = db.DayMenu.Where(dm => dm.Cod_Menu == id && dm.Cod_Dia == diaid).ToList();
            if (daymenu.Count()!=0)
            {
                diaMenuView.Menu = menu;
                diaMenuView.Dia = new Dias();
                diaMenuView.Dias = dias.OrderBy(d => d.Cod_Dia).ToList();
                ModelState.AddModelError(string.Empty, "Ya se añadido ese dia");
                ViewBag.Error = true;
                ListDia.Add(new Dias { Cod_Dia = 0, Nombre_Dia = "{Seleccione el Dia...}" });
                ViewBag.DiasId = new SelectList(ListDia.OrderBy(d => d.Cod_Dia).ToList(), "Cod_Dia", "Nombre_Dia");
                return View(diaMenuView);
            }
            var diamenu = new DayMenu
            {
                Cod_Dia = Convert.ToInt32(diaid),
                Cod_Menu = id 
            };
            try
            {
                db.DayMenu.Add(diamenu);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Error = true;
            }
            if (diaid != 0)
            {
                var Ndia = db.Dias.Find(Convert.ToInt32(diaid));
                var dia1 = new Dias
                {
                    Cod_Dia = Ndia.Cod_Dia,
                    Nombre_Dia = Ndia.Nombre_Dia
                };
                dias.Add(dia1);
            }

            diaMenuView.Menu = menu;
            diaMenuView.Dia = new Dias();
            diaMenuView.Dias = dias.OrderBy(d => d.Cod_Dia).ToList();
            ListDia.Add(new Dias { Cod_Dia = 0, Nombre_Dia = "Seleccione el Dia" });
            ViewBag.DiasId = new SelectList(ListDia.OrderBy(d => d.Cod_Dia).ToList(), "Cod_Dia", "Nombre_Dia");
            return View(diaMenuView);
        }

        public ActionResult ConvertirImagen(int id)
        {
            var menu = db.Menus.Where(m => m.Cod_Menu == id).FirstOrDefault();
            return File(menu.Imagen_Menu, "image/jpeg");
        }

        public ActionResult TMenu(int? id, bool? D, bool? E)
        {
            var Mlist = new MenuView();
            using (var transacion = db.Database.BeginTransaction())
            {
                try
                {
                    if (id != null && E == null)
                    {
                        var tmenu = db.TipoMenus.Find(id);
                        if (tmenu == null)
                        {
                            ModelState.AddModelError(string.Empty, "El tipo de menú no existe");
                            ViewBag.Error = true;
                            Mlist.LTipoMenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Eliminado_TMenu == true).ToList();
                            return View(Mlist);
                        }
                        else if (D != null)
                        {
                            tmenu.Estado_TMenu = D.Value;
                            db.Entry(tmenu).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            tmenu.Eliminado_TMenu = false;
                            tmenu.Estado_TMenu = false;
                            db.Entry(tmenu).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (id != null && E == true)
                    {
                        var tmenu = db.TipoMenus.Find(id);
                        if (tmenu == null)
                        {
                            ModelState.AddModelError(string.Empty, "El tipo de menú " + tmenu.Nombre_TMenu + " no existe");
                            ViewBag.Error = true;
                            Mlist.LTipoMenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Eliminado_TMenu == true).ToList();
                            return View(Mlist);
                        }
                        else if (E != null)
                        {
                            Mlist.TipoMenu = tmenu;
                            ViewBag.Editar = tmenu.Cod_TMenu;
                        }
                    }
                    Mlist.LTipoMenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Eliminado_TMenu == true).OrderBy(t => t.Nombre_TMenu).ToList();
                    transacion.Commit();
                    return View(Mlist);
                }
                catch (Exception ex)
                {
                    if (D != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error al desactivar, " + ex.Message);
                    }
                    else if (id != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error al Eliminar, " + ex.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    ViewBag.Error = true;
                    Mlist.LTipoMenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Eliminado_TMenu == true).OrderBy(t => t.Nombre_TMenu).ToList();
                    transacion.Rollback();
                    return View(Mlist);
                }
            }
        }

        [HttpPost]
        public ActionResult TMenu(TipoMenu TipoMenu, int? Editar)
        {
            var Mlist = new MenuView();
            if (ModelState.IsValid)
            {
                using (var transacion = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (Editar == null)
                        {
                            TipoMenu.Nombre_TMenu = TipoMenu.Nombre_TMenu.ToLower();
                            if (db.TipoMenus.Where(t => t.Nombre_TMenu == TipoMenu.Nombre_TMenu).Count() == 0)
                            {
                                TipoMenu.Estado_TMenu = true;
                                TipoMenu.Eliminado_TMenu = true;
                            }
                            db.TipoMenus.Add(TipoMenu);
                            db.SaveChanges();
                        }
                        else if (Editar != null)
                        {
                            var tmenu = db.TipoMenus.Find(Convert.ToInt32(Editar));
                            tmenu.Nombre_TMenu = TipoMenu.Nombre_TMenu.ToLower();
                            db.Entry(tmenu).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        transacion.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null &&
                            ex.InnerException.InnerException != null &&
                            ex.InnerException.InnerException.Message != null)
                        {
                            if (db.TipoMenus.Where(t => t.Nombre_TMenu == TipoMenu.Nombre_TMenu && t.Eliminado_TMenu == false).Count() != 0)
                            {
                                ModelState.AddModelError(string.Empty, "El tipo de menú " + TipoMenu.Nombre_TMenu + " ya existe, pero se encuentra eliminado");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "El tipo de menú " + TipoMenu.Nombre_TMenu + " ya existe");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.Message);
                        }
                        Mlist.LTipoMenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Eliminado_TMenu == true).OrderBy(t => t.Nombre_TMenu).ToList();
                        ViewBag.Error = true;
                        transacion.Rollback();
                        return View(Mlist);
                    }
                }
            }
            Mlist.LTipoMenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Eliminado_TMenu == true).OrderBy(t => t.Nombre_TMenu).ToList();
            Mlist.TipoMenu = new TipoMenu();
            return View(Mlist);
        }

        // GET: Menus
        public ActionResult Index(int? page)
        {
            page = (page ?? 1);
            var Blist = new List<Buscar>();
            Blist.Add(new Buscar { Cbuscar = 0, Tbuscar = "TIPO DE BUSQUEDA... " });
            Blist.Add(new Buscar { Cbuscar = 1, Tbuscar = "TIPO DE MENÚ" });
            Blist.Add(new Buscar { Cbuscar = 2, Tbuscar = "NOMBRE DEL MENÚ" });
            ViewBag.Cod_Tbuscar = new SelectList(Blist.OrderBy(m => m.Cbuscar).ToList(), "Cbuscar", "Tbuscar");
            var menu = db.Menus.Where(t => t.Cod_Menu != 1).
                OrderBy(m => m.Nombre_Menu);
            return View(menu.ToPagedList((int)page, 5));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexB(int? page)
        {
            page = (page ?? 1);
            ViewBag.CurrentFiltre = Request["NombreM"];
            int Cod_Tbuscar = Convert.ToInt32(Request["Cod_Tbuscar"]);
            var NombreM = Request["NombreM"];
            var Blist = new List<Buscar>();
            var menu = new List<Menu>();
            var Tmenu = new List<TipoMenu>();
            Blist.Add(new Buscar { Cbuscar = 0, Tbuscar = "TIPO DE BUSQUEDA... " });
            Blist.Add(new Buscar { Cbuscar = 1, Tbuscar = "TIPO DE MENÚ" });
            Blist.Add(new Buscar { Cbuscar = 2, Tbuscar = "NOMBRE DEL MENÚ" });            
            if (Cod_Tbuscar != 0)
            {
                if (Cod_Tbuscar == 1 && NombreM != "")
                {
                    Tmenu = db.TipoMenus.Where(t => t.Cod_TMenu != 1 && t.Nombre_TMenu.Contains(NombreM)).ToList();
                    if (Tmenu.Count() != 0)
                    {
                        foreach (var item in Tmenu)
                        {
                            foreach (var item1 in item.Menu.ToList())
                            {
                                menu.Add(item1);
                            }                           
                        }                        
                    }
                    ViewBag.Cod_Tbuscar = new SelectList(Blist.Where(m => m.Cbuscar == Cod_Tbuscar).ToList(), "Cbuscar", "Tbuscar");
                    ViewBag.NombreM = NombreM;
                }
                else if (Cod_Tbuscar == 2 && NombreM != "")
                {
                    menu = db.Menus.Where(t => t.Cod_Menu != 1 && t.Nombre_Menu.Contains(NombreM)).OrderBy(m => m.Nombre_Menu).ToList();
                    ViewBag.Cod_Tbuscar = new SelectList(Blist.Where(m => m.Cbuscar == Cod_Tbuscar).ToList(), "Cbuscar", "Tbuscar");
                    ViewBag.NombreM = NombreM;
                }
                else
                {
                    menu = db.Menus.Where(t => t.Cod_Menu != 1).OrderBy(m => m.Nombre_Menu).ToList();
                    ViewBag.Cod_Tbuscar = new SelectList(Blist.OrderBy(m => m.Cbuscar).ToList(), "Cbuscar", "Tbuscar");
                    var n = Blist.Where(t => t.Cbuscar == Cod_Tbuscar).Select(t => t.Tbuscar);
                    ModelState.AddModelError(string.Empty, "Ingrese el "+n);
                    ViewBag.Error = true;
                }                               
            }
            else
            {
                menu = db.Menus.Where(t => t.Cod_Menu != 1).OrderBy(m => m.Nombre_Menu).ToList();
                ViewBag.Cod_Tbuscar = new SelectList(Blist.OrderBy(m => m.Cbuscar).ToList(), "Cbuscar", "Tbuscar");
                ModelState.AddModelError(string.Empty, "Seleccione el tipo de menú");
                ViewBag.Error = true;
            }            
            return View("Index",menu.ToPagedList((int)page, 5));
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            var Mlist = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
            Mlist.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Menu" });
            ViewBag.Cod_TMenu = new SelectList(Mlist.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu");
            var menuView = new MenuView();
            menuView.Dias = db.Dias.ToList();
            menuView.Menu = new Menu();
            menuView.TipoMenu = new TipoMenu();
            return View(menuView);
        }

        // POST: Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuView MenuView, HttpPostedFileBase Imagen_Menu)
        {
            var Mlist = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
            if (ModelState.IsValid)
            {
                using (var transacion = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (Imagen_Menu != null && Imagen_Menu.ContentLength > 0)
                        {
                            byte[] imagenData = null;
                            using (var binaryMenu = new BinaryReader(Imagen_Menu.InputStream))
                            {
                                imagenData = binaryMenu.ReadBytes(Imagen_Menu.ContentLength);
                            }
                            MenuView.Menu.Imagen_Menu = imagenData;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Por favor Selecciones una Imagen");
                        }
                        var Tmenu = Request["Cod_TMenu"];
                        if (Convert.ToInt32(Tmenu) == 0)
                        {
                            ModelState.AddModelError(string.Empty, "Por favor Selecciones el tipo de menu");
                        }
                        var menu = new Menu
                        {
                            Nombre_Menu = MenuView.Menu.Nombre_Menu.ToLower(),
                            Descripcion_Menu = MenuView.Menu.Descripcion_Menu.ToLower(),
                            Cod_TMenu = Convert.ToInt32(Tmenu),
                            Estado_Menu = true,
                            Eliminado_Menu = true,
                            Imagen_Menu = MenuView.Menu.Imagen_Menu,
                            Valor_Menu = MenuView.Menu.Valor_Menu
                        };
                        db.Menus.Add(menu);
                        db.SaveChanges();
                        var id = db.Menus.Where(t => t.Estado_Menu == true).Max(m => m.Cod_Menu);
                        transacion.Commit();
                        return RedirectToAction(string.Format("AddDiaMenu/{0}", id));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null &&
                            ex.InnerException.InnerException != null &&
                            ex.InnerException.InnerException.Message != null)
                        {
                            if (db.Menus.Where(t => t.Nombre_Menu == MenuView.Menu.Nombre_Menu && t.Eliminado_Menu == false).Count() == 0)
                            {
                                ModelState.AddModelError(string.Empty, "El menú " + MenuView.Menu.Nombre_Menu + " ya existe");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "El menú " + MenuView.Menu.Nombre_Menu + " ya existe, pero se encuentra eliminado");
                            }
                                                 
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.ToString());
                        }
                        Mlist.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Mesa" });
                        ViewBag.Cod_TMenu = new SelectList(Mlist.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu", MenuView.Menu.Cod_TMenu);
                        transacion.Rollback();
                        ViewBag.Error = true;
                        return View(MenuView);
                    }
                }
            }
            Mlist.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Menu" });
            ViewBag.Cod_TMenu = new SelectList(Mlist.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu", MenuView.Menu.Cod_TMenu);
            return View(MenuView);
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            var lisDia = new List<Dias>();
            try
            {              
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                var daymenu = db.DayMenu.ToList();
                foreach (var item in daymenu)
                {
                    if (menu.Cod_Menu == item.Cod_Menu)
                    {
                        var dia = new Dias
                        {
                            Cod_Dia = item.Cod_Dia,
                            Nombre_Dia = item.Dias.Nombre_Dia
                        };
                        lisDia.Add(dia);
                    }
                }
                var Mlist = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
                var Elist = db.Menus.Where(t => t.Estado_Menu == true).ToList();
                Mlist.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Mesa" });
                ViewBag.Cod_TMenu = new SelectList(Mlist.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu", menu.Cod_TMenu);
                var editMenu = new EditMenuView();
                editMenu.Dia = new Dias();
                editMenu.Dias = lisDia;
                editMenu.Menu = menu;
                return View(editMenu);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
                var menu = db.Menus.Find(id);
                var Mlist = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
                var Elist = db.Menus.Where(t => t.Estado_Menu == true).ToList();
                Mlist.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Mesa" });
                ViewBag.Cod_TMenu = new SelectList(Mlist.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu", menu.Cod_TMenu);
                var editMenu = new EditMenuView();
                editMenu.Dia = new Dias();
                editMenu.Dias = lisDia;                
                editMenu.Menu = menu;
                ViewBag.Error = true;
                return View(editMenu);
            }
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditMenuView EditMenu, HttpPostedFileBase Imagen_Menu)
        {
            var lisDia = new List<Dias>();
            int Tmenu = Convert.ToInt32(Request["Cod_TMenu"]);
            using (var transacion = db.Database.BeginTransaction())
            {
                try
                {
                    if (Imagen_Menu != null && Imagen_Menu.ContentLength > 0)
                    {
                        byte[] imagenData = null;
                        using (var binaryMenu = new BinaryReader(Imagen_Menu.InputStream))
                        {
                            imagenData = binaryMenu.ReadBytes(Imagen_Menu.ContentLength);
                        }
                        EditMenu.Menu.Imagen_Menu = imagenData;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Por favor Selecciones una Imagen");
                    }
                    var daymenu = db.DayMenu.ToList();
                    foreach (var item in daymenu)
                    {
                        if (EditMenu.Menu.Cod_Menu == item.Cod_Menu)
                        {
                            var dia = new Dias
                            {
                                Cod_Dia = item.Cod_Dia,
                                Nombre_Dia = item.Dias.Nombre_Dia
                            };
                            lisDia.Add(dia);
                        }
                    }
                                        
                    if (Tmenu == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Por favor selecciones un tipo de menú ");
                    }
                    if (ModelState.IsValid)
                    {
                        var menu = new Menu
                        {
                            Nombre_Menu = EditMenu.Menu.Nombre_Menu,
                            Descripcion_Menu = EditMenu.Menu.Descripcion_Menu,
                            Cod_TMenu = Tmenu,
                            Estado_Menu = true,
                            Valor_Menu = EditMenu.Menu.Valor_Menu,
                            Imagen_Menu = EditMenu.Menu.Imagen_Menu,
                            Cod_Menu = EditMenu.Menu.Cod_Menu
                        };
                        db.Entry(menu).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(null, ex.Message);
                    var Mlists = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
                    Mlists.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Menu" });
                    ViewBag.Cod_TMenu = new SelectList(Mlists.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu", Tmenu);
                    var editMenus = new EditMenuView();
                    editMenus.Dia = new Dias();
                    editMenus.Dias = lisDia;
                    editMenus.Menu = EditMenu.Menu;
                    ViewBag.Error = true;
                    return View(editMenus);
                }
            }
            var Mlist = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
            Mlist.Add(new TipoMenu { Cod_TMenu = 0, Nombre_TMenu = "Seleccione el Tipo Menu" });
            ViewBag.Cod_TMenu = new SelectList(Mlist.OrderBy(m => m.Cod_TMenu).ToList(), "Cod_TMenu", "Nombre_TMenu", Tmenu);
            var editMenu = new EditMenuView();
            editMenu.Dia = new Dias();
            editMenu.Dias = lisDia;
            editMenu.Menu = EditMenu.Menu;
            return View(editMenu);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id, bool? D)
        {
            try
            {
                if (id == null || D == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                else if (D != null)
                {
                    menu.Estado_Menu = D.Value;
                    db.Entry(menu).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    menu.Eliminado_Menu = false;
                    db.Entry(menu).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (D != null)
                {
                    ModelState.AddModelError(null, "Error al desactivar, "+ex.Message);
                }
                else if (id != null)
                {
                    ModelState.AddModelError(null, "Error al Eliminar, " + ex.Message);
                }
                else
                {
                    ModelState.AddModelError(null,ex.Message);
                }
                ViewBag.Error = true;
                return  RedirectToAction("Index");
            }
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
