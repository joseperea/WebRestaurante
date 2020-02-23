using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebRestaurante.Models;
using PagedList;
using WebRestaurante.ModelsViews;
using System.Web;
using System.IO;

namespace WebRestaurante.Controllers
{
    public class MesasController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();

        // GET: Mesas
        public ActionResult Index(int? page)
        {
            page = (page ?? 1);
            var mesas = db.Mesas.
                OrderBy(m => m.Cod_Mesa);
            return View(mesas.ToPagedList((int)page, 5));
        }        

        // GET: Mesas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mesas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mesas Mesas)
        {
            if (ModelState.IsValid)
            {
                using (var transacion = db.Database.BeginTransaction())
                {
                    try
                    {
                        var mesas = db.Mesas.ToList();
                        if (mesas.Count != 0)
                        {
                            foreach (var item in mesas)
                            {
                                var mesa = db.Mesas.Find(item.Cod_Mesa);
                                db.Mesas.Remove(mesa);
                                db.SaveChanges();
                            }
                        }
                        int numero = Convert.ToInt32(Mesas.Numero_Mesa);
                        for (int i = 1; i <= numero; i++)
                        {
                            var mesa = new Mesas
                            {
                                Numero_Mesa = ("M " + i),
                                Estado_Mesa = true
                            };
                            db.Mesas.Add(mesa);
                            db.SaveChanges();
                        }
                        transacion.Commit();
                    }
                    catch (Exception ex)
                    {
                        
                        ModelState.AddModelError(string.Empty, ex.ToString());
                        ViewBag.Error = true;
                        transacion.Rollback();
                        return View(Mesas);
                    }
                }
                return RedirectToAction("Index");
            }            
            return View(Mesas);
        }

        // GET: Mesas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mesas = db.Mesas.Find(id);
            if (mesas == null)
            {
                return HttpNotFound();
            }
           return View(mesas);
        }

        // POST: Mesas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mesas Mesas)
        {
            if (ModelState.IsValid)
            {
                var mesas = new Mesas
                {
                    Numero_Mesa = Mesas.Numero_Mesa,
                    Estado_Mesa = Mesas.Estado_Mesa,
                    Cod_Mesa = Mesas.Cod_Mesa
                };
                db.Entry(mesas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }            
            return View(Mesas);
        }

        // GET: Mesas/Delete/5
        public ActionResult Delete(int? id, bool? D)
        {
            try
            {
                if (id == null || D == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var mesa = db.Mesas.Find(id);
                if (mesa == null)
                {
                    return HttpNotFound();
                }
                else if (D != null)
                {
                    mesa.Estado_Mesa = D.Value;
                    db.Entry(mesa).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    mesa.Eliminado_Mesa = false;
                    db.Entry(mesa).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (D != null)
                {
                    ModelState.AddModelError(null, "Error al desactivar, " + ex.Message);
                }
                else if (id != null)
                {
                    ModelState.AddModelError(null, "Error al Eliminar, " + ex.Message);
                }
                else
                {
                    ModelState.AddModelError(null, ex.Message);
                }
                ViewBag.Error = true;
                return RedirectToAction("Index");
            }
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var mesas = db.Mesas.Find(id);
            mesas.Estado_Mesa = false;
            db.Entry(mesas).State = EntityState.Modified;
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
