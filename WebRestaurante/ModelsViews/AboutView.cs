using System.ComponentModel.DataAnnotations;
using WebRestaurante.Models;

namespace WebRestaurante.ModelsViews
{
    public class AboutView
    {
        public Contatenos Contatenos { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }

        public string Direccion { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Display(Name = "Mapa ubicación")]
        public string Ubicacion { get; set; }
    }
}