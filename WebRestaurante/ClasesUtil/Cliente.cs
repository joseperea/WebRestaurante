using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;
using WebRestaurante.ModelsViews;

namespace WebRestaurante.ClasesUtil
{
    public class Cliente
    {
        public static Guid idCliente(string correo, string nombre, string apellido, string telefono, WebRestauranteContext db)
        {
            var usuario = db.Clientes.Where(c => c.Correo_Cli == correo).FirstOrDefault();
            if (usuario == null)
            {
                var cliente = new Clientes
                {
                    Nombres_Cli = nombre,
                    Apellidos_Cli = apellido,
                    Correo_Cli = correo,
                    Telefono_Cli = telefono,
                    Fecha_Cli = DateTime.Now.Date,
                    UserName = correo

                };
                db.Clientes.Add(cliente);
                db.SaveChanges();
            }
            Guid idCliente = db.Clientes.Where(c => (c.Correo_Cli == correo)).FirstOrDefault().Cod_Cli;
            return idCliente;
        }

    }
}