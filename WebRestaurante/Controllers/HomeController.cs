using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;
using WebRestaurante.ClasesUtil;
using System.Web.Configuration;

namespace WebRestaurante.Controllers
{
    public class HomeController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectDia(int? id)
        {
            var menus = new List<Menu>();
            var lisCDias = new List<CDias>();
            var HomeView = new HomeView();
            DateTime Fecha = DateTime.Now.Date.Date;
            var listDias = db.Dias.ToList();
            if (id != null)
            {
                var daymenu = db.DayMenu.Where(dm => dm.Cod_Dia == id).ToList();
                foreach (var item in daymenu)
                {
                    var Menus = db.Menus.Find(item.Cod_Menu);
                    menus.Add(Menus);
                }
            }
            foreach (var item in listDias)
            {
                var Cdias = new CDias
                {
                    Cantidad = db.DayMenu.Where(t => t.Cod_Dia == item.Cod_Dia).Count(),
                    IdDia = item.Cod_Dia
                };
                lisCDias.Add(Cdias);
            }
            ViewBag.SeleDia = id;
            HomeView.Menu = new Menu();
            HomeView.Menus = menus;
            HomeView.Dias = listDias;
            HomeView.CantidadD = lisCDias;
            ViewBag.CMesas = CantidadMesas.CMesas(db);
            return View("Index",HomeView);
        }

        public ActionResult Index()
        {
            var listDias = new List<Models.Dias>();
            var HomeView = new HomeView();
            using (var transacion = db.Database.BeginTransaction())
            {
                try
                {
                    string correo = WebConfigurationManager.AppSettings["mailAccount"];
                    var clientes = db.Clientes.ToList();
                    var cliente = clientes.Where(t => t.Correo_Cli == correo);
                    var listaDT = db.TipoDocumentoes.ToList();
                    listDias = db.Dias.ToList();
                    if (listDias.Count() == 0 && cliente.Count() == 0 && listaDT.Count() == 0)
                    {
                        Cliente.idCliente(WebConfigurationManager.AppSettings["mailAccount"], "NRestaurante", "PRestaurante", "3177965608", db);
                        ClasesUtil.Dias.listaDias(db);
                        TipoDocumentos.ListaTD(db);
                        listDias = db.Dias.ToList();
                    }
                    var menus = new List<Menu>();
                    var lisCDias = new List<CDias>();
                    var Dia = DateTime.Now.DayOfWeek;
                    switch (Dia)
                    {
                        case DayOfWeek.Sunday:
                            var Sunday = db.DayMenu.Where(dm => dm.Cod_Dia == 7).ToList();
                            foreach (var item in Sunday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                           
                            break;
                        case DayOfWeek.Monday:
                            var Monday = db.DayMenu.Where(dm => dm.Cod_Dia == 1).ToList();
                            foreach (var item in Monday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                            break;
                        case DayOfWeek.Tuesday:
                            var Tuesday = db.DayMenu.Where(dm => dm.Cod_Dia == 2).ToList();
                            foreach (var item in Tuesday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                            break;
                        case DayOfWeek.Wednesday:
                            var Wednesday = db.DayMenu.Where(dm => dm.Cod_Dia == 3).ToList();
                            ViewBag.CWednesday = db.DayMenu.Where(dm => dm.Cod_Dia == 3).Count();
                            ViewBag.Wednesday = "Miercoles";
                            foreach (var item in Wednesday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                            break;
                        case DayOfWeek.Thursday:
                            var Thursday = db.DayMenu.Where(dm => dm.Cod_Dia == 4).ToList();
                            foreach (var item in Thursday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                            break;
                        case DayOfWeek.Friday:
                            var Friday = db.DayMenu.Where(dm => dm.Cod_Dia == 5).ToList();
                            foreach (var item in Friday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                            break;
                        case DayOfWeek.Saturday:
                            var Saturday = db.DayMenu.Where(dm => dm.Cod_Dia == 6).ToList();
                            foreach (var item in Saturday)
                            {
                                var Menus = db.Menus.Find(item.Cod_Menu);
                                menus.Add(Menus);
                            }
                            ViewBag.CMesas = CantidadMesas.CMesas(db);
                            break;
                        default:
                            break;
                    }
                    var listaD = db.Dias.ToList();
                    foreach (var item in listaD)
                    {
                        var Cdias = new CDias
                        {
                            Cantidad = db.DayMenu.Where(t => t.Cod_Dia == item.Cod_Dia).Count(),
                            IdDia = item.Cod_Dia
                        };
                        lisCDias.Add(Cdias);
                    }
                    HomeView.Menu = new Menu();
                    HomeView.Menus = menus;
                    HomeView.Dias = listDias;
                    HomeView.CantidadD = lisCDias;
                    transacion.Commit();                    
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                    transacion.Rollback();
                }
            }
            return View(HomeView);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            var ListC = db.Restaurantes.ToList();
            var AboutView = new AboutView();
            AboutView.Contatenos = new Contatenos();
            AboutView.Correo = ListC.FirstOrDefault().Correo;
            AboutView.Direccion = ListC.FirstOrDefault().Direccion;
            AboutView.Telefono = ListC.FirstOrDefault().Telefono;
            AboutView.Ubicacion = ListC.FirstOrDefault().Ubicacion;
            ViewBag.Message = "informacion del restaurante";
            return View(AboutView);
        }
    }
}