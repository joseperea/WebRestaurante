using System.Collections.Generic;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class MenuView
    {
        public TipoMenu TipoMenu { get; set; }

        public Menu Menu { get; set; }

        public List<Dias> Dias { get; set; }

        public List<TipoMenu> LTipoMenu { get; set; }

        public int Editar { get; set; }
    }
}