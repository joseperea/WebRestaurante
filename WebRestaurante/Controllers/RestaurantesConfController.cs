using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebRestaurante.Models;

namespace WebRestaurante.Controllers
{
    public class RestaurantesConfController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();

        public ActionResult Index()
        {
            var Restaurante = db.Restaurantes.ToList();
            if (Restaurante.Count > 0)
            {
                Guid id = Restaurante.Max(r => r.Id_Res);
                return RedirectToAction(string.Format("Edit/{0}", id));
            }
            return RedirectToAction("Create", "RestaurantesConf") ;
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantesConf/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Restaurante restaurante)
        {
            using (var transacion = db.Database.BeginTransaction()) 
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Restaurantes.Add(restaurante);
                        db.SaveChanges();
                        if (db.TipoMenus.Count() == 0)
                        {
                            var TMenu = new TipoMenu
                            {
                                Nombre_TMenu = "Tipo Menu",
                                Estado_TMenu = false
                            };
                            db.TipoMenus.Add(TMenu);
                            db.SaveChanges();
                        }
                        if (db.Menus.Count() == 0)
                        {
                            var Menu = new Menu
                            {
                                Cod_TMenu = 1,
                                Descripcion_Menu = "xxxx xxxxx xxxxx",
                                Estado_Menu = false,
                                Nombre_Menu = "xxxxxxxxxx",
                                Valor_Menu = 0
                            };
                            db.Menus.Add(Menu);
                            db.SaveChanges();
                        }
                        transacion.Commit();
                        return RedirectToAction("Create", "Mesas");
                    }
                }
                catch (Exception ex)
                {
                    transacion.Rollback();
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(restaurante);
                }
            }

            return View(restaurante);
        }

        // GET: RestaurantesConf/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurante restaurante = db.Restaurantes.Find(id);
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // POST: RestaurantesConf/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Res,Nombre,Mision,Vision,Objetivos,Telefono,Direccion,Correo")] Restaurante restaurante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restaurante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(restaurante);
        }

        // GET: RestaurantesConf/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurante restaurante = db.Restaurantes.Find(id);
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // POST: RestaurantesConf/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Restaurante restaurante = db.Restaurantes.Find(id);
            db.Restaurantes.Remove(restaurante);
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
