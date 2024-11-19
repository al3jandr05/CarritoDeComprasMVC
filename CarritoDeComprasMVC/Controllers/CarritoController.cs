using CarritoDeComprasMVC.Data;
using CarritoDeComprasMVC.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoDeComprasMVC.Controllers
{
    public class CarritoController : Controller
    {
        private readonly MyDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoController(MyDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoItems.Any())
            {
                return View("CarritoVacio");
            }

            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            if (quantity <= 0) return RedirectToAction("Index");

            var carritoItem = await _context.CarritoItems
                .Include(ci => ci.Producto)
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (carritoItem != null)
            {
                carritoItem.Cantidad = quantity;
                carritoItem.PrecioTotal = carritoItem.Producto.Precio * quantity;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EmptyCart()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito != null && carrito.CarritoItems.Any())
            {
                _context.CarritoItems.RemoveRange(carrito.CarritoItems);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var carritoItem = await _context.CarritoItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (carritoItem != null)
            {
                _context.CarritoItems.Remove(carritoItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            int cartItemCount = carrito?.CarritoItems.Sum(ci => ci.Cantidad) ?? 0;
            return Ok(new { cartItemCount });
        }
    }
}
