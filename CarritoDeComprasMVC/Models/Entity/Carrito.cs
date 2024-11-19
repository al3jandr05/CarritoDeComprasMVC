using Microsoft.AspNetCore.Identity;

namespace CarritoDeComprasMVC.Models.Entity
{
    public class Carrito
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public IdentityUser Usuario { get; set; }

        public List<CarritoItem> CarritoItems { get; set; }
    }
}
