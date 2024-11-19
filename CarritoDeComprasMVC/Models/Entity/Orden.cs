using Microsoft.AspNetCore.Identity;

namespace CarritoDeComprasMVC.Models.Entity
{
    public class Orden
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioId { get; set; }
        public decimal Total { get; set; }
        public List<OrdenItem> Items { get; set; }
        public IdentityUser Usuario { get; set; }

    }
}
