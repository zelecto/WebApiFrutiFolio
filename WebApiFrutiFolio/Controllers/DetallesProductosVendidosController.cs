using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;

namespace WebApiFrutiFolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesProductosVendidosController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public DetallesProductosVendidosController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/DetallesProductosVendidoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallesProductosVendido>>> GetDetallesProductosVendidos()
        {
            return await _context.DetallesProductosVendidos.ToListAsync();
        }

        // GET: api/DetallesProductosVendidoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetallesProductosVendido>> GetDetallesProductosVendido(int id)
        {
            var detallesProductosVendido = await _context.DetallesProductosVendidos.FindAsync(id);

            if (detallesProductosVendido == null)
            {
                return NotFound();
            }

            return detallesProductosVendido;
        }

        // PUT: api/DetallesProductosVendidoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetallesProductosVendido(int id, DetallesProductosVendido detallesProductosVendido)
        {
            if (id != detallesProductosVendido.Id)
            {
                return BadRequest();
            }

            _context.Entry(detallesProductosVendido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetallesProductosVendidoExists(id))
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

        // POST: api/DetallesProductosVendidoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DetallesProductosVendido>> PostDetallesProductosVendido(DetallesProductosVendido detallesProductosVendido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que la cantidad a vender esté dentro del rango permitido
            if (detallesProductosVendido.Cantidadvendida < 1 || detallesProductosVendido.Cantidadvendida > 999999)
            {
                return BadRequest("La cantidad a vender debe estar entre 1 y 999999.");
            }

            // Verificar si se proporcionó el producto en la solicitud
            if (detallesProductosVendido.producto != null)
            {
                // Verificar si el producto existe
                Producto producto = await _context.Productos.FindAsync(detallesProductosVendido.producto.Id);
                if (producto == null)
                {
                    return NotFound("El producto no existe.");
                }

                // Verificar si la cantidad a vender es mayor que el stock del producto
                if (detallesProductosVendido.Cantidadvendida > producto.Stock)
                {
                    return BadRequest("La cantidad a vender es mayor que el stock disponible.");
                }
            }

            _context.DetallesProductosVendidos.Add(detallesProductosVendido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetallesProductosVendido", new { id = detallesProductosVendido.Id }, detallesProductosVendido);
        }



        // DELETE: api/DetallesProductosVendidoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallesProductosVendido(int id)
        {
            var detallesProductosVendido = await _context.DetallesProductosVendidos.FindAsync(id);
            if (detallesProductosVendido == null)
            {
                return NotFound();
            }

            _context.DetallesProductosVendidos.Remove(detallesProductosVendido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetallesProductosVendidoExists(int id)
        {
            return _context.DetallesProductosVendidos.Any(e => e.Id == id);
        }

        // GET: api/DetallesProductosVendidoes/ByFactura/{facturaId}
        [HttpGet("ByFactura/{facturaId}")]
        public async Task<ActionResult<IEnumerable<DetallesProductosVendido>>> GetDetallesProductosVendidosByFactura(int facturaId)
        {
            // Verificar si existe la factura
            var factura = await _context.Facturas.FindAsync(facturaId);
            if (factura == null)
            {
                return NotFound($"No se encontró ninguna factura con el ID '{facturaId}'.");
            }

            // Obtener los detalles de productos vendidos asociados a la factura
            var detallesProductosVendidos = await _context.DetallesProductosVendidos
                .Where(d => d.Idfactura == facturaId)
                .Include(d => d.producto) // Incluir la información del producto asociado
                .ToListAsync();

            if (detallesProductosVendidos.Count == 0)
            {
                return NotFound($"No se encontraron detalles de productos vendidos para la factura con ID '{facturaId}'.");
            }

            return detallesProductosVendidos;
        }

    }
}
