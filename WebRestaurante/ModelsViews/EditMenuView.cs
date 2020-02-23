using System.Collections.Generic;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class EditMenuView
    {
        public Menu Menu { get; set; }

        public Dias Dia { get; set; }

        public List<Dias> Dias { get; set; }
    }
}