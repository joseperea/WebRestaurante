using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;
using PagedList;

namespace WebRestaurante.Controllers
{
    public class ClientesController : Controller
    {
        private WebRestauranteContext db = new WebRestauranteContext();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public ClientesController()
        {

        }
        public ClientesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Clientes
        public ActionResult Index(int? page)
        {
            page = (page ?? 1);
            var clientes = db.Clientes.
                OrderBy(c => c.Apellidos_Cli).
                ThenBy(c => c.Nombres_Cli);
            return View(clientes.ToPagedList((int)page, 5));
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            var registroView = new RegistroView();
            registroView.Clientes = new Clientes();
            registroView.User = new RegisterViewModel();
            var documentos = db.TipoDocumentoes.ToList();
            documentos.Add(new TipoDocumento { Cod_TDoc=0, Nombre_TDoc= "{ Selecione Documento}" });            
            ViewBag.Cod_TDoc = new SelectList(documentos.OrderBy(d => d.Nombre_TDoc).ToList(), "Cod_TDoc", "Nombre_TDoc");
            return View(registroView);
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegistroView RegistroView)
        {        
            var documentos = db.TipoDocumentoes.ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = RegistroView.User.Email, Email = RegistroView.User.Email };
                var result = await UserManager.CreateAsync(user, RegistroView.User.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                ModelState.AddModelError(string.Empty, result.ToString());
                var municipio = Request["Cod_Muni"];
                var documeto = Request["Cod_TDoc"];
                var clientes = new Clientes
                {
                    Nombres_Cli = RegistroView.Clientes.Nombres_Cli,
                    Apellidos_Cli = RegistroView.Clientes.Apellidos_Cli,
                    Correo_Cli = RegistroView.User.Email,
                    Telefono_Cli = RegistroView.Clientes.Telefono_Cli,
                    Fecha_Cli = DateTime.Now.Date,
                    UserName= RegistroView.User.Email,

                };
                db.Clientes.Add(clientes);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.ToString());                    
                    documentos.Add(new TipoDocumento { Cod_TDoc = 0, Nombre_TDoc = "{ Selecione Documento}" });              
                    return View(RegistroView);
                }
                return RedirectToAction("Index", "Home");
            }            
            documentos.Add(new TipoDocumento { Cod_TDoc = 0, Nombre_TDoc = "{ Selecione Documento}" });
            return View(RegistroView);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }            
            return View(clientes);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Clientes clientes = db.Clientes.Find(id);
            db.Clientes.Remove(clientes);
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
