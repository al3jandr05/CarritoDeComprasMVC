using CarritoDeComprasMVC.Data;
using CarritoDeComprasMVC.Models;
using CarritoDeComprasMVC.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CarritoDeComprasMVC.Controllers
{
    public class ProductoController : Controller
    {
        private readonly MyDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductoController(MyDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos.ToListAsync();
            ViewBag.CartCount = await GetCartItemCount();
            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Cantidad no válida");

            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = userId, CarritoItems = new List<CarritoItem>() };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            var carritoItem = carrito.CarritoItems.FirstOrDefault(ci => ci.ProductoId == productId);

            if (carritoItem != null)
            {
                carritoItem.Cantidad += quantity;
            }
            else
            {
                var producto = await _context.Productos.FindAsync(productId);
                if (producto == null) return NotFound();

                carritoItem = new CarritoItem
                {
                    ProductoId = productId,
                    Cantidad = quantity,
                    CarritoId = carrito.Id,
                    PrecioTotal = producto.Precio * quantity
                };
                carrito.CarritoItems.Add(carritoItem);
            }

            await _context.SaveChangesAsync();

            var cartItemCount = carrito.CarritoItems.Sum(ci => ci.Cantidad);

            // Solo devolvemos el conteo actualizado sin redirigir ni mostrar JSON en pantalla
            return Json(new { cartItemCount });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var producto = new Producto
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Precio = model.Precio,
                    Stock = model.Stock,
                    ImagenUrl = model.ImagenUrl
                };

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            var model = new ProductoEditViewModel
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                ImagenUrl = producto.ImagenUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null) return NotFound();

                producto.Nombre = model.Nombre;
                producto.Descripcion = model.Descripcion;
                producto.Precio = model.Precio;
                producto.Stock = model.Stock;
                producto.ImagenUrl = model.ImagenUrl;

                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        private async Task<int> GetCartItemCount()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            return carrito?.CarritoItems.Sum(ci => ci.Cantidad) ?? 0;
        }
    }
}
