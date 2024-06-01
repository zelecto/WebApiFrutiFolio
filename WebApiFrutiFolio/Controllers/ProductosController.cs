using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;

namespace WebApiFrutiFolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public ProductosController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/Productos/Consultar todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        [HttpGet("/api/productos/Usuarios/{username}")]
        public async Task<ActionResult<List<Producto>>> GetProductosByOwner(string username)
        {
            var productos = await _context.Productos
                                        .Where(p => p.Username == username && p.Activo == true)
                                        .ToListAsync();

            if (productos == null || productos.Count == 0)
            {
                return NotFound(); // O devuelve un código de estado HTTP 404 si no se encuentran productos activos
            }

            return productos;
        }



        // GET: api/Productoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT: api/Productoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Productoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            // Validar nombre no nulo ni vacío
            if (string.IsNullOrEmpty(producto.Name) || string.IsNullOrWhiteSpace(producto.Name))
            {
                return BadRequest("El nombre del producto es obligatorio y no puede estar vacío.");
            }else if(producto.Name.Trim().Length > 50)
            {
                return BadRequest("El nombre del producto no puede superar los 50 caracteres");
            }

            // Validar descripción no vacía si está presente y menor a 150 caracteres
            if (!string.IsNullOrEmpty(producto.Description) && producto.Description.Trim().Length > 150)
            {
                return BadRequest("La descripción del producto no puede exceder los 150 caracteres.");
            }
            

            // Validar que ningún dato numérico sea negativo
            if (producto.Stock < 0 || producto.Price < 0)
            {
                return BadRequest("El stock y el precio del producto no pueden ser valores negativos.");
            }

            // Validar que el stock tenga hasta 6 decimales
            if (producto.Stock % 1 != 0 || producto.Stock.ToString().Split('.').LastOrDefault()?.Length > 6)
            {
                return BadRequest("El stock del producto debe ser un número entero con hasta 6 decimales.");
            }else if (producto.Stock<1)
            {
                return BadRequest("El stock debe ser mayor a 0");
            }

            // Validar que el precio tenga hasta 8 decimales
            if (producto.Price.ToString().Split('.').LastOrDefault()?.Length > 8)
            {
                return BadRequest("El precio del producto no puede tener más de 8 decimales.");
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        }



        // DELETE: api/Productoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Nuevo endpoint para consultar productos de un usuario filtrados por stock
        [HttpGet("/api/productos/Usuarios/{username}/FiltrarPorStock")]
        public async Task<ActionResult<List<Producto>>> GetProductosByOwnerAndStock(string username, [FromQuery] bool stockMayorAZero)
        {
            var productos = await _context.Productos
                                          .Where(p => p.Username == username && p.Activo == true)
                                          .Where(p => stockMayorAZero ? p.Stock > 0 : p.Stock == 0)
                                          .ToListAsync();

            if (productos == null || productos.Count == 0)
            {
                return NotFound(); // O devuelve un código de estado HTTP 404 si no se encuentran productos que cumplan los criterios
            }

            return productos;
        }


        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
