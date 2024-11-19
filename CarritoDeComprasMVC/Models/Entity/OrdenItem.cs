namespace CarritoDeComprasMVC.Models.Entity
{
    public class OrdenItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int OrdenId { get; set; } 
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public Producto Producto { get; set; }
        public Orden Orden { get; set; } 
    }

}
