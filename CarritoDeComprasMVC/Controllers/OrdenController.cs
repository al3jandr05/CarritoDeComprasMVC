using CarritoDeComprasMVC.Data;
using CarritoDeComprasMVC.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoDeComprasMVC.Controllers
{
    public class OrdenController : Controller
    {
        private readonly MyDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdenController(MyDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.Producto) 
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoItems.Any())
            {
                return RedirectToAction("Index", "Carrito");
            }

            var total = carrito.CarritoItems.Sum(item => item.Producto.Precio * item.Cantidad);

            var orden = new Orden
            {
                UsuarioId = userId,
                FechaCreacion = DateTime.Now,
                Total = total,
                Items = carrito.CarritoItems.Select(item => new OrdenItem
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto.Precio,
                    PrecioTotal = item.Producto.Precio * item.Cantidad,
                    Producto = item.Producto 
                }).ToList()
            };

            return View(orden);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmOrder()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoItems.Any())
            {
                return RedirectToAction("Index", "Carrito");
            }

            var total = carrito.CarritoItems.Sum(item => item.Producto.Precio * item.Cantidad);

            var orden = new Orden
            {
                UsuarioId = userId,
                FechaCreacion = DateTime.Now,
                Total = total,
                Items = carrito.CarritoItems.Select(item => new OrdenItem
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto.Precio,
                    PrecioTotal = item.Producto.Precio * item.Cantidad
                }).ToList()
            };

            _context.Ordenes.Add(orden);

            foreach (var item in carrito.CarritoItems)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto != null)
                {
                    producto.Stock -= item.Cantidad;
                }
            }

            _context.CarritoItems.RemoveRange(carrito.CarritoItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { orderId = orden.Id });
        }

        [HttpGet]
        public IActionResult OrderConfirmation(int orderId)
        {
            return View(orderId);
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Historial()
        {
            var userId = _userManager.GetUserId(User);
            var ordenes = await _context.Ordenes
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Producto) 
                .Where(o => o.UsuarioId == userId)
                .ToListAsync();

            return View(ordenes);
        }

    }
}
