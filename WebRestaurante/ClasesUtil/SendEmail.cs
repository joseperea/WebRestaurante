using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using WebRestaurante.Models;

namespace WebRestaurante.ClasesUtil
{
    public class SendEmail : IDisposable
    {
        private static ApplicationDbContext DbContext = new ApplicationDbContext();
        public static async Task sendMail(string to, string subject, string body)
        {

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(WebConfigurationManager.AppSettings["mailAccount"]);
            msg.To.Add(new MailAddress(to));
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var Credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["mailAccount"],
                    Password = WebConfigurationManager.AppSettings["mailPassword"]
                };
                smtp.Credentials = Credential;
                smtp.Host = WebConfigurationManager.AppSettings["SmtpName"];
                smtp.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["SmtpPort"]);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(msg);
            }

        }

        public static async Task passawordRecovery(string email, WebRestauranteContext db)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DbContext));
            var UserASP = UserManager.FindByEmail(email);
            if (UserASP == null)
            {
                return;
            }
            var user = db.Clientes.Where(c => c.UserName == email).FirstOrDefault();

            if (user == null)
            {
                return;
            }
            var NRandon = new Random();
            var newPassaword = string.Format("{0}{2:04}{1}"
                , user.Apellidos_Cli.Trim().ToUpper().Substring(0,1)
                , user.Nombres_Cli.Trim().ToUpper().Substring(0,1)
                ,NRandon.Next(9999));
            UserManager.RemovePassword(UserASP.Id);
            UserManager.AddPassword(UserASP.Id, newPassaword);

            var subject = "Recordar Contraseña";
            var body = string.Format(@"<h1>Cliente "+user.Nombres_Cli+
                "</h1> <p>Su nueva contrseña es:<strong>{0}</strong></p><p>Por favor ingrese con esta nueva contraseña, y la cambia apenas inicie</p>", newPassaword);
            await sendMail(email, subject, body);
        }

        public static async Task ConfirmarReserva(int idCliente, string NReserva, WebRestauranteContext db)
        {
            int CDocumento = 0, CPersonas = 0, M = 0;
            var DMesasCliente = db.DetalleMesasCliente.Where(r => (r.Cod_Cli == idCliente && r.NConfirmacion_DMC == NReserva)).ToList();
            var Cliente = db.Clientes.Where(c => c.Cod_Cli == idCliente).ToList() ;
            string fecha = "", horai = "", horas = "";
            foreach (var item in DMesasCliente)
            {
                fecha = item.MesasOcupadas.Fecha_MesasO.Date.ToString();
                horai = item.MesasOcupadas.HoraIngreso_MesasO.TimeOfDay.ToString();
                horas = item.MesasOcupadas.HoraSalida_MesasO.TimeOfDay.ToString();
                CPersonas = item.MesasOcupadas.CPersonas_Mesas;
                if (item.Cod_MesasO != M)
                {
                    CDocumento++;
                    M = item.Cod_MesasO;
                }                
            }
            if (Cliente == null)
            {
                return;
            }
            string Nombres = Cliente.FirstOrDefault().Nombres_Cli + " " + Cliente.FirstOrDefault().Apellidos_Cli;
            var subject = "Confirmacion de Reserva";
            var body = string.Format(@"<h1><strong>Señor, " + Nombres +
                "</strong></h1> <p><h2>Ha generado una Reservar</h2></p><p><h3>Detalles de la Reservar</h3></p>" +
                "<p># Mesas: " + CDocumento + " </p> <p>Cantidad de Personas: " + CPersonas + " </p> <p>Fecha: " + fecha + "</p> <p>Hora de Ingreso: " + horai + "</p> <p>Tiempo de espera: " + horas + "</p>" +
                "<p>Link de confirmacion de Reserva: <a href='http://localhost:55157/DetalleReserva/DetalleReserva?Idr=" + NReserva + "&Idc=" + idCliente + "'>http://localhost:55157/DetalleReserva/DetalleReserva?Idr=" + NReserva + "&Idc=" + idCliente + "</a><p>");
            await sendMail(Cliente.FirstOrDefault().Correo_Cli, subject, body);
        }


        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}