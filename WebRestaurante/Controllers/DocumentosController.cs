using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;
using WebRestaurante.ClasesUtil;
using System.Net;
using System.Data.Entity;

namespace WebRestaurante.Controllers
{
    public class DocumentosController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();
        private DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private DateTime hora = new DateTime
                            (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                             DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(DetalleView DetalleView, string id)
        {
            string aux = "";
            string[] ID = id.Split(new char[] { ','});            
            var DCM = new List<DetalleMesasCliente>();
            
            using (var transacion = db.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < ID.Length; i++)
                    {
                        if (ID[i] != "")
                        {
                            DCM.Add(db.DetalleMesasCliente.Find(Convert.ToInt32(ID[i])));
                        }
                    }
                    decimal valor = 0;
                    foreach (var item in DCM)
                    {
                        int x = 0;
                        valor = valor + (item.Cantidad_DMC * item.Menu.Valor_Menu);
                        string[] s = aux.Split(new char[] {','});
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (s[i] != "")
                            {
                                x = 0;
                                if (Convert.ToInt32(s[i]) == item.Cod_MesasO)
                                {
                                    x++;
                                }
                            }
                        }
                        if (x==0)
                        {
                            aux = aux + "," + item.Cod_MesasO;
                        }
                    }
                    int consecutivo = Consecutivo.Numero(db);
                    var D = new Documento
                    {
                        Cod_Cli = DCM.FirstOrDefault().Cod_Cli,
                        Cod_TDoc = 1,
                        Estado_Doc = true,
                        Fecha_Doc = DCM.FirstOrDefault().MesasOcupadas.Fecha_MesasO,
                        HoraIngreso_Doc = DCM.FirstOrDefault().MesasOcupadas.HoraIngreso_MesasO,
                        HoraSalida_Doc = hora,
                        NConfirmacion_Doc = DCM.FirstOrDefault().NConfirmacion_DMC,
                        Valor_Doc = valor, 
                        Consecutivo = consecutivo
                    };
                    db.Documento.Add(D);
                    db.SaveChanges();
                    int idD = db.Documento.Where(t => (t.NConfirmacion_Doc==D.NConfirmacion_Doc) && (t.HoraSalida_Doc == D.HoraSalida_Doc) && (t.Id_Doc == D.Id_Doc)).Max(x =>x.Id_Doc);
                    foreach (var item in DCM)
                    {
                        var DC = new DetalleDocumento
                        {
                            Id_DMC = item.Id_DMC,
                            Id_Doc = idD
                        };
                        var ADMC = db.DetalleMesasCliente.Find(item.Id_DMC);
                        ADMC.Estado_DMC = false;
                        ADMC.MesasOcupadas.Estado_MesasO = false;
                        ADMC.MesasOcupadas.Llegada_MesasO = false;
                        db.Entry(ADMC).State = EntityState.Modified;
                        db.DetalleDocumento.Add(DC);
                    }
                    db.SaveChanges();
                    transacion.Commit();

                    var edetalles = new EDetalles
                    {
                        D = 0,
                        IdC = DCM.FirstOrDefault().Cod_Cli,
                        IdMesaO = DCM.FirstOrDefault().Cod_MesasO, 
                        IdM = DCM.FirstOrDefault().MesasOcupadas.Cod_Mesa,
                        NC = DCM.FirstOrDefault().NConfirmacion_DMC,
                        F = true,
                        Add = aux             
                    };
                    return RedirectToAction("Detalles", edetalles);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    transacion.Rollback();
                    throw;
                }
            }           
            //return View();
        }

        public ActionResult Detalles(int? IdM, int? IdC, int? IdMesaO, string NC, int? D, bool? F, string Add) 
        {
            var Cliente = new Clientes(); var edetalle = new EDetalles();
            var DCM = new List<DetalleMesasCliente>();       
            var listMesasO = new List<MesasOcupadas>();
            var addMesasO = new List<MesasOcupadas>();
            var ListMenu = new List<Menu>();
            var TipoDocumento = new TipoDocumento();
            var Detalle = new DetalleView();
            int imesas = 0, imenu=0;
            decimal Valor = 0;
            string aux = "";
            using (var transacion = db.Database.BeginTransaction())
             {
                try
                {
                    if (Add != null)
                    {
                        string aux2 = "";
                        string[] x = Add.Split(new char[] { ',' });
                        for (int i = 0; i < x.Length; i++)
                        {
                            if (x[i] != "")
                            {
                                int add = Convert.ToInt32(x[i]);
                                if (add < 0)
                                {
                                    string[] x2 = x[i].Split(new char[] { '-' });
                                    aux2 = aux2 + "," + x2[1];
                                }
                            }
                        }
                        if (aux2 != "")
                        {
                            string[] x3 = aux2.Split(new char[] { ',' });
                            for (int i = 0; i < x3.Length; i++)
                            {
                                if (x3[i] != "")
                                {
                                    string[] x4 = Add.Split(new char[] { ',' });
                                    Add = "";
                                    for (int i1 = 0; i1 < x4.Length; i1++)
                                    {
                                        if (x4[i1] != "")
                                        {
                                            if (x4[i1] != x3[i] && Convert.ToInt32(x4[i1]) > 0)
                                            {
                                                Add = Add + "," + x4[i1];
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        if (Add != "")
                        {
                            var DCM2 = new List<DetalleMesasCliente>();
                            string[] s = Add.Split(new char[] { ',' });
                            for (int i = 0; i < s.Length; i++)
                            {
                                if (s[i] != "")
                                {
                                    int add = Convert.ToInt32(s[i]);
                                    if (add > 0)
                                    {
                                        DCM2 = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == add).ToList();
                                        foreach (var item in DCM2)
                                        {
                                            if ((item.Estado_DMC == true) || (F != null))
                                            {
                                                DetalleMesasCliente a = db.DetalleMesasCliente.Find(item.Id_DMC);
                                                DCM.Add(a);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                     }
                    if (F == null)
                    {
                        var DCM1 = new List<DetalleMesasCliente>();
                        DCM1 = db.DetalleMesasCliente.Where(t => t.Cod_Cli == IdC && t.NConfirmacion_DMC == NC).ToList();
                        foreach (var item in DCM1)
                        {
                            if ((item.Estado_DMC == true) || (F != null))
                            {
                                DetalleMesasCliente a = db.DetalleMesasCliente.Find(item.Id_DMC);
                                DCM.Add(a);
                            }
                        }
                     }                                                                                                       
                        if (DCM.Count() != 0)
                        {
                        int i = 0, i1 = 0;
                            foreach (var item in DCM)
                            {
                                DateTime hora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
                                if (item.MesasOcupadas.Fecha_MesasO == DateTime.Now.Date && hora >= item.MesasOcupadas.HoraIngreso_MesasO)
                                {                                                                                                                     
                                    if (item.Cantidad_DMC != 0)
                                    {
                                        Menu M = db.Menus.Find(item.Cod_Menu);
                                        Valor = Valor + (M.Valor_Menu * item.Cantidad_DMC);
                                        ListMenu.Add(M);
                                    }
                                    if (i1 != item.MesasOcupadas.CPersonas_Mesas)
                                    {
                                        i1 = item.MesasOcupadas.CPersonas_Mesas;
                                        i = i + item.MesasOcupadas.CPersonas_Mesas; 
                                    }
                                    if (listMesasO.Where(t => t.Cod_MesasO == item.Cod_MesasO).Count() == 0)
                                    {
                                        MesasOcupadas MO = db.MesasOcupadas.Find(item.Cod_MesasO);
                                        imesas++;
                                        aux = aux + " - " + MO.Mesas.Numero_Mesa;
                                        listMesasO.Add(MO);
                                    }
                                    //if (Cliente.Cod_Cli == 0 && TipoDocumento.Cod_TDoc == 0)
                                    //{
                                    //    Cliente = db.Clientes.Find(item.Cod_Cli);
                                    //    TipoDocumento = db.TipoDocumentoes.Find(item.Cod_TDoc);

                                    //}
                                    if (item.PedidoM == true)
                                    {
                                        imenu++;
                                    }
                                if (item.Cod_MesasO == IdMesaO)
                                {
                                    Detalle.Fecha = item.MesasOcupadas.Fecha_MesasO;
                                    Detalle.HoraI = item.MesasOcupadas.HoraIngreso_MesasO;
                                    Detalle.Horas = item.MesasOcupadas.HoraSalida_MesasO;
                                    Cliente = db.Clientes.Find(item.Cod_Cli);
                                    TipoDocumento = db.TipoDocumentoes.Find(item.Cod_TDoc);
                                }
                            }
                                else
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                }
                            }
                            addMesasO = AddMesas.Lista(IdC.Value, IdM.Value, db, fecha, hora);
                            var quitar = addMesasO.Where(t => t.Reservada == NC).ToList();
                            foreach (var item in quitar)
                            {
                             var r = addMesasO.Single(t => t.Cod_MesasO == item.Cod_MesasO);
                            if (r != null)
                            {
                                addMesasO.Remove(r);
                            }                                                            
                            }                        
                            if (F == null)
                            {
                                edetalle = new EDetalles
                                {
                                    D = D.Value,
                                    F = false,
                                    IdC = IdC.Value,
                                    IdM = IdM.Value,
                                    IdMesaO = IdMesaO.Value,
                                    NC = NC,
                                    Add = Add
                                };
                            }
                            else
                            {
                                edetalle = new EDetalles
                                {
                                    D = D.Value,
                                    F = true,
                                    IdC = IdC.Value,
                                    IdM = IdM.Value,
                                    IdMesaO = IdMesaO.Value,
                                    NC = NC,
                                    Add = Add
                                };
                            }
                            Detalle.Cmesas = imesas;
                            Detalle.Nmesas = aux;
                            Detalle.Cmenu = imenu;
                            Detalle.Cpersonas = i;
                            Detalle.Confirmacion = listMesasO.FirstOrDefault().ConfirmarMesa;
                            Detalle.Detalle = D.Value;
                            Detalle.addMesasO = addMesasO;
                            Detalle.Mesas = db.Mesas.ToList();
                            Detalle.ValorTotal = Valor;
                            Detalle.EDetalles = edetalle;
                            Detalle.consecutivo = Consecutivo.Numero(db);
                            if (F == null)
                            {
                                Detalle.F = false;
                            }
                            else
                            {
                                Detalle.F = F.Value;
                            }
                            transacion.Commit();
                        }
                        else
                        {
                            return HttpNotFound();
                        }
                                                 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ViewBag.Error = true;
                }
            }
            Detalle.Cliente = Cliente;
            Detalle.TipoDocumento = TipoDocumento;
            Detalle.Menus = ListMenu.OrderBy(t => t.Cod_TMenu).ToList();
            Detalle.DetalleMesasClientes = DCM;         
            return View(Detalle);
        }


        public ActionResult AnadirMenu(int? IdM, int? IdC, int? IdMesaO, int? add)
        {
            var listMenu = new List<MenuList>();
            var addMenus = new AddMenu();
            try
            {
                if (add == null)
                {
                    if (IdC == null || IdM == null)
                    {
                        //return HttpNotFound();
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    if (IdMesaO != null)
                    {
                        MesasOcupadas MO = db.MesasOcupadas.Find(IdMesaO);
                        addMenus.CPersona = Convert.ToInt32(MO.CPersonas_Mesas);
                        var DMO = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == IdMesaO).ToList();
                        foreach (var item in DMO)
                        {
                            if (item.Cod_Cli != IdC || item.MesasOcupadas.Mesas.Cod_Mesa != IdM)
                            {
                                return HttpNotFound();
                            }
                            if (item .PedidoM == true)
                            {
                                var MC = new MenuList
                                {
                                    checkbox = true,
                                    IdMenus = item.Cod_Menu,
                                    cantidad = item.Cantidad_DMC,
                                    TMenu = item.Menu.Cod_TMenu
                                };
                                listMenu.Add(MC);

                            }
                        }
                    }
                    addMenus.Menus = db.Menus.Where(t => t.Estado_Menu == true).OrderBy(t => t.Cod_TMenu).ToList();
                    addMenus.TipoMenus = db.TipoMenus.Where(t => t.Estado_TMenu == true).ToList();
                    addMenus.IdCliente = Convert.ToInt32(IdC);
                    addMenus.Mesa = db.Mesas.Find(IdM);
                    addMenus.IdMenu = Convert.ToInt32(IdM);
                    addMenus.IdMesasO = Convert.ToInt32(IdMesaO);
                    addMenus.checkbox = listMenu;                   
                }
               
            }
            catch (Exception ex)
            {
                addMenus.Menus = db.Menus.OrderBy(t => t.Cod_TMenu).ToList();
                ModelState.AddModelError(string.Empty, ex.Message);
                addMenus.TipoMenus = db.TipoMenus.ToList();
                addMenus.IdCliente = Convert.ToInt32(IdC);
                addMenus.Mesa = db.Mesas.Find(IdM);
                addMenus.IdMenu = Convert.ToInt32(IdM);
                addMenus.checkbox = listMenu;
                return View(addMenus);
            }
            return View(addMenus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnadirMenu(AddMenu AddMenu, int IdM, int IdC, string[] selectedMenu, string[] DCantida, int? IdMesaO, string CPersona)
        {
            var listMenu = new List<MenuList>();
            var addMenus = new AddMenu();
            using (var Transacion = db.Database.BeginTransaction())
            {
                try
                {
                    if (CPersona == "CANTIDAD DE PERSONAS....")
                    {
                        addMenus.Menus = db.Menus.OrderBy(t => t.Cod_TMenu).ToList();                        
                        addMenus.TipoMenus = db.TipoMenus.ToList();
                        addMenus.IdCliente = Convert.ToInt32(IdC);
                        addMenus.Mesa = db.Mesas.Find(IdM);
                        addMenus.IdMenu = Convert.ToInt32(IdM);
                        addMenus.checkbox = listMenu;
                        ModelState.AddModelError(string.Empty, "Por favor seleccione una cantidad de persona");
                        ViewBag.Error = true;
                        return View(addMenus);
                    }
                    if (selectedMenu == null)
                    {
                        addMenus.Menus = db.Menus.OrderBy(t => t.Cod_TMenu).ToList();
                        addMenus.TipoMenus = db.TipoMenus.ToList();
                        addMenus.IdCliente = Convert.ToInt32(IdC);
                        addMenus.Mesa = db.Mesas.Find(IdM);
                        addMenus.IdMenu = Convert.ToInt32(IdM);
                        addMenus.IdMesasO = IdMesaO.Value;
                        addMenus.CPersona = Convert.ToInt32(CPersona);
                        addMenus.checkbox = listMenu;
                        ModelState.AddModelError(string.Empty, "Por favor seleccione por lo menos un menu");
                        ViewBag.Error = true;
                        return View(addMenus);
                    }
                    AddMenus.ingresar(db, IdM, IdC, selectedMenu, DCantida, IdMesaO, CPersona, null);                    
                    Transacion.Commit();
                    return RedirectToAction("Facturacion");
                }
                catch (Exception ex)
                {
                    addMenus.Menus = db.Menus.OrderBy(t => t.Cod_TMenu).ToList();                
                    addMenus.TipoMenus = db.TipoMenus.ToList();
                    addMenus.IdCliente = Convert.ToInt32(IdC);
                    addMenus.Mesa = db.Mesas.Find(IdM);
                    addMenus.IdMenu = Convert.ToInt32(IdM);
                    addMenus.checkbox = listMenu;
                    addMenus.CPersona = Convert.ToInt32(CPersona);
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ViewBag.Error = true;
                    Transacion.Rollback();
                    return View(addMenus);
                }                   
            }               
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Facturacion()
        {
            var Tbuscar = Request["Cod_Tbuscar"];
            var buscar = Request["CBuscar"];
            var factura = new List<factura>();
            var Blist = new List<Buscar>();
            var mesas = db.Mesas.ToList();
            var mesasEstado = new List<MesasOcupadas>();
            var mesasllegada = new List<MesasOcupadas>();
            var DMC = new List<DetalleMesasCliente>();
            var facturacionView = new FacturaView();
            bool busquedad = false;
            int x = Convert.ToInt32(Tbuscar);
            try
            {
                Blist.Add(new Buscar { Cbuscar = 0, Tbuscar = "TIPO DE BUSQUEDA... " });
                Blist.Add(new Buscar { Cbuscar = 1, Tbuscar = "MESA" });
                Blist.Add(new Buscar { Cbuscar = 2, Tbuscar = "CLIENTE" });
                ViewBag.Cod_Tbuscar = new SelectList(Blist.OrderBy(m => m.Cbuscar).ToList(), "Cbuscar", "Tbuscar");
                foreach (var item in mesas)
                {
                    var mesasOcupada = db.MesasOcupadas.Where(t => t.Cod_Mesa == item.Cod_Mesa).ToList();
                    var mesasFecha = mesasOcupada.Where(t => t.Fecha_MesasO == fecha).ToList();
                    if (mesasFecha.Count() == 0)
                    {
                        var mesasHora = mesasFecha.Where(t => (hora >= t.HoraIngreso_MesasO) && (t.HoraSalida_MesasO >= hora)).ToList();
                        mesasEstado = mesasHora.Where(t => t.Estado_MesasO == true).ToList();
                    }
                    else
                    {
                        if (mesasFecha.FirstOrDefault().Llegada_MesasO == true)
                        {
                            var mesasHora = mesasFecha.Where(t => (hora >= t.HoraIngreso_MesasO)).ToList();
                            mesasEstado = mesasHora.Where(t => t.Estado_MesasO == true).ToList();
                        }
                        else
                        {
                            var mesasHora = mesasFecha.Where(t => (hora >= t.HoraIngreso_MesasO) && (t.HoraSalida_MesasO >= hora)).ToList();
                            mesasEstado = mesasHora.Where(t => t.Estado_MesasO == true).ToList();
                        }
                    }
                    if (mesasEstado.Count() == 0)
                    {

                        var f = new factura
                        {
                            Estado = item.Estado_Mesa,
                            Numero_Mesa = item.Numero_Mesa,
                            Cod_Mesa = item.Cod_Mesa,
                            Ocupada = false,
                        };
                        factura.Add(f);
                    }
                    else if (mesasEstado.Count() > 0)
                    {
                        decimal R = 0;
                        int i = 0, idMesaO;
                        idMesaO = mesasEstado.FirstOrDefault().Cod_MesasO;
                        DMC = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == idMesaO).ToList();
                        string[] Tipo = DMC.FirstOrDefault().NConfirmacion_DMC.Split(new char[] { '-' });
                        foreach (var item1 in DMC)
                        {
                            Menu M = db.Menus.Find(item1.Cod_Menu);
                            R = R + (M.Valor_Menu * item1.Cantidad_DMC);
                            if (item1.PedidoM == true)
                            {
                                i++;
                            }
                        }
                        if (x != 0 && buscar != "")
                        {
                            if (x == 1)
                            {
                                long M = 0;
                                bool B = long.TryParse(Convert.ToString(buscar), out M);
                                if (B == true)
                                {
                                    if (item.Cod_Mesa == M)
                                    {
                                        busquedad = true;
                                        ViewBag.busqueda = true;
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Por favor ingrese un numero de mesa");
                                    ViewBag.Error = true;
                                }
                            }
                            else
                            {
                                string[] Cnombre = DMC.FirstOrDefault().Cliente.Nombres_Cli.ToLower().Split(new char[] { ' ' });
                                string[] Capellido = DMC.FirstOrDefault().Cliente.Apellidos_Cli.ToLower().Split(new char[] { ' ' });
                                string[] s = buscar.Split(new char[] { ' ' });
                                for (int y = 0; y < s.Length; y++)
                                {
                                    if (Cnombre.Count() > 1 && Capellido.Count() > 1)
                                    {
                                        if (Cnombre[0].Contains(s[y].ToLower()) || Capellido[0].Contains(s[y].ToLower()) ||
                                       Cnombre[1].Contains(s[y].ToLower()) || Capellido[1].Contains(s[y].ToLower()))
                                        {
                                            busquedad = true;
                                            ViewBag.busqueda = true;
                                        }
                                    }
                                    else if (Cnombre.Count() == 1 && Capellido.Count() > 1)
                                    {
                                        if (Cnombre[0].Contains(s[y].ToLower()) || Capellido[0].Contains(s[y].ToLower()) ||Capellido[1].Contains(s[y].ToLower()))
                                        {
                                            busquedad = true;
                                            ViewBag.busqueda = true;
                                        }
                                    }
                                    else if (Cnombre.Count() > 1 && Capellido.Count() == 1)
                                    {
                                        if (Cnombre[0].Contains(s[y].ToLower()) || Capellido[0].Contains(s[y].ToLower()) || Cnombre[1].Contains(s[y].ToLower()))
                                        {
                                            busquedad = true;
                                            ViewBag.busqueda = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Cnombre[0].Contains(s[y].ToLower()) || Capellido[0].Contains(s[y].ToLower()))
                                        {
                                            busquedad = true;
                                            ViewBag.busqueda = true;
                                        }
                                    }                                   
                                }
                            }
                        }
                        else if (x == 0)
                        {

                            ModelState.AddModelError(string.Empty,"Selecciona el tipo de busquedad");
                            ViewBag.Error = true;
                        }
                        else if (buscar == "")
                        {
                            if (ModelState.IsValid)
                            {
                                ModelState.AddModelError(string.Empty, "Ingresa el numero de mesa o nombre del cliente");
                                ViewBag.Error = true;
                            }
                        }
                        var f = new factura
                        {
                            Estado = item.Estado_Mesa,
                            Llegada = mesasEstado.FirstOrDefault().Llegada_MesasO,
                            Numero_Mesa = item.Numero_Mesa,
                            Cod_Mesa = item.Cod_Mesa,
                            Ocupada = true,
                            Cod_MesasO = mesasEstado.FirstOrDefault().Cod_MesasO,
                            NConfirmacion = mesasEstado.FirstOrDefault().Reservada,
                            ValorTotal = R,
                            CMenu = i,
                            Cod_Cli = DMC.FirstOrDefault().Cod_Cli,
                            TipoFactura = Tipo[0],
                            Busquedad = busquedad
                        };
                        factura.Add(f);
                        busquedad = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Ha ocurrido un error al mostrar la vista <br/>" + ex.Message));
                facturacionView.factura = factura;
                return View(facturacionView);
            }
            int c = 0, ca = factura.Count();
            foreach (var item in factura)
            {
                if (item.Busquedad == false && x == 2)
                {
                    c++;
                }
            }
            if (c == ca)
            {
                ModelState.AddModelError(string.Empty, "El cliente no ha reservado");
                ViewBag.Error = true;
            }
            facturacionView.factura = factura;
            return View(facturacionView);
        }

        public ActionResult Facturacion( bool? E, string NF, int? IdM)
        {
            var factura= new List<factura>();
            var Blist = new List<Buscar>();
            var mesas = db.Mesas.ToList();
            var mesasEstado = new List<MesasOcupadas>();
            var mesasllegada = new List<MesasOcupadas>();
            var DMC = new List<DetalleMesasCliente>();
            var facturacionView = new FacturaView();            
            try
            {
                Blist.Add(new Buscar { Cbuscar = 0, Tbuscar = "TIPO DE BUSQUEDA... " });
                Blist.Add(new Buscar { Cbuscar = 1, Tbuscar = "MESA" });
                Blist.Add(new Buscar { Cbuscar = 2, Tbuscar = "CLIENTE" });
                ViewBag.Cod_Tbuscar = new SelectList(Blist.OrderBy(m => m.Cbuscar).ToList(), "Cbuscar", "Tbuscar");
                if (E == true)
                {
                    int i = 0;
                    mesasllegada = db.MesasOcupadas.Where(t => t.Fecha_MesasO == fecha).ToList();
                    mesasllegada = mesasllegada.Where(t => t.Reservada == NF).ToList();
                    foreach (var item in mesasllegada)
                    {
                        if (item.Cod_MesasO == IdM)
                        {
                            i++;
                        }
                        var x = db.MesasOcupadas.Find(item.Cod_MesasO);
                        x.Llegada_MesasO = true;
                        db.Entry(x).State = EntityState.Modified;
                    }
                    if (i != 0)
                    {
                        db.SaveChanges();
                    }
                }
                foreach (var item in mesas)
                {
                    var mesasOcupada = db.MesasOcupadas.Where(t => t.Cod_Mesa == item.Cod_Mesa).ToList();
                    var mesasFecha = mesasOcupada.Where(t => t.Fecha_MesasO == fecha).ToList();
                    if (mesasFecha.Count() == 0)
                    {
                        var mesasHora = mesasFecha.Where(t => (hora >= t.HoraIngreso_MesasO) && (t.HoraSalida_MesasO >= hora)).ToList();
                        mesasEstado = mesasHora.Where(t => t.Estado_MesasO == true).ToList();
                    }
                    else
                    {
                        if (mesasFecha.FirstOrDefault().Llegada_MesasO == true)
                        {
                            var mesasHora = mesasFecha.Where(t => (hora >= t.HoraIngreso_MesasO)).ToList();
                            mesasEstado = mesasHora.Where(t => t.Estado_MesasO == true).ToList();
                        }
                        else
                        {
                            var mesasHora = mesasFecha.Where(t => (hora >= t.HoraIngreso_MesasO) && (t.HoraSalida_MesasO >= hora)).ToList();
                            mesasEstado = mesasHora.Where(t => t.Estado_MesasO == true).ToList();
                        }
                    }                             
                    if (mesasEstado.Count() == 0)
                    {
                       
                        var f = new factura
                        {
                            Estado = item.Estado_Mesa,
                            Numero_Mesa = item.Numero_Mesa,
                            Cod_Mesa = item.Cod_Mesa,
                            Ocupada = false,                           
                        };
                        factura.Add(f);
                    }
                    else if (mesasEstado.Count() > 0)
                    {
                        decimal R = 0;
                        int i = 0, idMesaO;
                        idMesaO = mesasEstado.FirstOrDefault().Cod_MesasO;
                        DMC = db.DetalleMesasCliente.Where(t => t.Cod_MesasO == idMesaO).ToList();
                        string[] Tipo = DMC.FirstOrDefault().NConfirmacion_DMC.Split(new char[] { '-' });
                        foreach (var item1 in DMC)
                        {
                            Menu M = db.Menus.Find(item1.Cod_Menu);
                            R = R + (M.Valor_Menu * item1.Cantidad_DMC);
                            if (item1.PedidoM == true)
                            {
                                i++;
                            }
                        }

                        var f = new factura
                        {
                            Estado = item.Estado_Mesa,
                            Llegada = mesasEstado.FirstOrDefault().Llegada_MesasO,
                            Numero_Mesa = item.Numero_Mesa,
                            Cod_Mesa = item.Cod_Mesa,
                            Ocupada = true,
                            Cod_MesasO = mesasEstado.FirstOrDefault().Cod_MesasO,
                            NConfirmacion = mesasEstado.FirstOrDefault().Reservada,
                            ValorTotal = R,
                            CMenu = i,
                            Cod_Cli = DMC.FirstOrDefault().Cod_Cli,
                            TipoFactura = Tipo[0]                    
                        };
                        factura.Add(f);
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Ha Ocurrido un Error al mostrar la vista <br/>"+ex.Message));
                facturacionView.factura = factura;
                return View(facturacionView);
            }
            facturacionView.factura = factura;
            return View(facturacionView);
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