using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRestaurante.Models;
using System.ComponentModel.DataAnnotations;

namespace WebRestaurante.ModelsViews
{
    public class AddMenu
    {
        public List<Menu> Menus { get; set; }
        public List<MenuList> checkbox { get; set; }
        public List<TipoMenu> TipoMenus { get; set; }
        public Mesas Mesa { get; set; }
        public int IdCliente { get; set; }
        public int IdMenu { get; set; }
        [Display(Name ="Cantidad Persona")]
        public int CPersona { get; set; }
        public int IdMesasO { get; set; }
    }

    public class MenuList
    {
        public int IdMenus { get; set; }
        public int TMenu { get; set; }
        public bool checkbox { get; set; }
        public int cantidad { get; set; }
    }

    public class AddR
    {
        public int IdM { get; set; }
        public int IdC { get; set; }
        public int IdMesaO { get; set; }
        public string CPersona { get; set; }
        public string add { get; set; }
        public string NCF { get; set; }
        public int? TMenu { get; set; }
    }
}