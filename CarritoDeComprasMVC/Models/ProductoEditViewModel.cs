﻿namespace CarritoDeComprasMVC.Models
{
    public class ProductoEditViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }
}
