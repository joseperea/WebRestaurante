using System.Collections.Generic;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class HomeView
    {
        public Menu Menu { get; set; }
        public List<Menu> Menus { get; set; }
        public List<Dias> Dias { get; set; }
        public List<CDias> CantidadD { get; set; }
    }

    public class CDias
    {
        public int IdDia { get; set; }
        public int Cantidad { get; set; }
    }
}