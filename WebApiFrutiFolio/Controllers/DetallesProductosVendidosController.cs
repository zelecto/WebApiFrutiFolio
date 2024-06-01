using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;

namespace WebApiFrutiFolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        // GET: api/TiendaVirtuals/TopVendidosByCity/{ciudad}
        [HttpGet("TopVendidosByCity/{ciudad}")]
        public async Task<ActionResult<object>> GetTiendasAndTopVendidosByCiudad(string ciudad)
        {
            var tiendas = await _context.TiendasVirtuales
                                        .Where(t => t.Ciudad == ciudad)
                                        .ToListAsync();

            if (tiendas == null || !tiendas.Any())
            {
                return NotFound("No se encontraron tiendas en la ciudad especificada.");
            }

            // Obtener el mes y año actual
            var fechaActual = DateTime.Now;
            int mesActual = fechaActual.Month;
            int añoActual = fechaActual.Year;

            // Obtener el ID de todas las tiendas en la ciudad
            var tiendaIds = tiendas.Select(t => t.Id).ToList();

            // Consultar los 5 productos más vendidos en el mes actual para todas las facturas asociadas a las tiendas en la ciudad
            var topVendidos = await _context.DetallesProductosVendidos
                .Where(dp => dp.IdfacturaNavigation != null &&
                             dp.IdfacturaNavigation.Pedidos.Any(p => tiendaIds.Contains(p.Id_Tienda)) &&
                             dp.IdfacturaNavigation.Fecha.Month == mesActual &&
                             dp.IdfacturaNavigation.Fecha.Year == añoActual)
                .GroupBy(dp => dp.Idproducto)
                .Select(group => new
                {
                    ProductoId = group.Key,
                    TotalVendido = group.Sum(dp => dp.Cantidadvendida),
                    Imagen = group.FirstOrDefault().producto.Img // Obtener la imagen del producto
                })
                .OrderByDescending(g => g.TotalVendido)
                .Take(5) // Limitar a 5 productos más vendidos
                .ToListAsync();

            var resultado = new
            {
                Tiendas = tiendas,
                TopVendidos = topVendidos
            };

            return Ok(resultado);
        }

        [HttpGet("VentasPorProductoEnRangoFechas")]
        public async Task<ActionResult<IEnumerable<object>>> GetVentasPorProductoEnRangoFechas(DateOnly fechaInicio, DateOnly fechaFin, string username)
        {
            if (fechaInicio > fechaFin)
            {
                return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin.");
            }

            var query = _context.DetallesProductosVendidos
                .Where(dp => dp.IdfacturaNavigation != null && dp.IdfacturaNavigation.Fecha >= fechaInicio && dp.IdfacturaNavigation.Fecha <= fechaFin)
                .Include(dp => dp.producto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(dp => dp.IdfacturaNavigation.UsuarioUsername == username);
            }

            var ventasPorProducto = await query
                .GroupBy(dp => dp.producto.Name)
                .Select(group => new
                {
                    NombreProducto = group.Key,
                    CantidadVendida = group.Sum(dp => dp.Cantidadvendida),
                    IngresoTotal = group.Sum(dp => dp.Cantidadvendida * dp.producto.Price) // Calcular el ingreso total por fruta
                })
                .OrderByDescending(result => result.CantidadVendida) // Ordenar por CantidadVendida descendente
                .ToListAsync();

            if (!ventasPorProducto.Any())
            {
                return NotFound("No se encontraron ventas en el rango de fechas especificado.");
            }

            return Ok(ventasPorProducto);
        }




    }
}
