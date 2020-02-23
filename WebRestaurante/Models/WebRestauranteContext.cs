using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WebRestaurante.Models
{
    public class WebRestauranteContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public WebRestauranteContext() : base("name=WebRestauranteContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<TipoDocumento> TipoDocumentoes { get; set; }
        public DbSet<DetalleMesasCliente> DetalleMesasCliente { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<TipoMenu> TipoMenus { get; set; }
        public DbSet<Mesas> Mesas { get; set; }
        public DbSet<Dias> Dias { get; set; }
        public DbSet<DayMenu> DayMenu { get; set; }
        public DbSet<MesasOcupadas> MesasOcupadas { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<DetalleDocumento> DetalleDocumento { get; set; }

    }
}
