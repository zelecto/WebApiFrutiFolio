using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class PedidosController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public PedidosController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/Pedidoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return await _context.Pedidos.ToListAsync();
        }

        // GET: api/Pedidoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

        // PUT: api/Pedidoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
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

        // POST: api/Pedidoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedidoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }


        // GET: api/Pedidos/FullDetails/tienda/{tiendaId}
        [HttpGet("FullDetails/tienda/{tiendaId}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosWithDetails(int tiendaId, [FromQuery] string? estado = null)
        {
            var query = _context.Pedidos
                                .Include(p => p.Factura)
                                .ThenInclude(f => f.Cliente)
                                .Include(p => p.TiendaVirtual)
                                .Include(p => p.ClienteUsuario)
                                .Where(p => p.Id_Tienda == tiendaId)
                                .Include(p=>p.ClienteUsuario)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            var pedidos = await query.ToListAsync();

            if (pedidos == null || pedidos.Count == 0)
            {
                return NotFound();
            }

            return pedidos;
        }

        [HttpPatch("{id}/ActualizarEstadoYPrecioEnvio")]
        public async Task<ActionResult<Pedido>> PatchActualizarEstadoYPrecioEnvio(int id, [FromBody] ActualizarEstadoYPrecioEnvioRequest request)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound();
            }

            pedido.Estado = request.Estado;
            pedido.PrecioTransporte = request.PrecioEnvio;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(pedido);
        }

        public class ActualizarEstadoYPrecioEnvioRequest
        {
            public string Estado { get; set; }
            public decimal PrecioEnvio { get; set; }
        }

        // GET: api/Pedidos/CantidadPorMes
        [HttpGet("CantidadPorMes")]
        public async Task<ActionResult<IEnumerable<object>>> GetCantidadPedidosPorMes(
            [FromQuery] DateOnly fechaInicio,
            [FromQuery] DateOnly fechaFin,
            [FromQuery] string username)
        {
            try
            {
                // Obtener la tienda asociada al username
                var tienda = await _context.TiendasVirtuales
                                           .FirstOrDefaultAsync(t => t.Username == username);

                if (tienda == null)
                {
                    return NotFound($"No se encontró ninguna tienda asociada al usuario '{username}'.");
                }

                // Filtrar pedidos por rango de fechas, ID de la tienda y estado "Entregado"
                var pedidos = await _context.Pedidos
                    .Include(p => p.Factura)
                    .Where(p => p.Factura.Fecha >= fechaInicio
                             && p.Factura.Fecha <= fechaFin
                             && p.Id_Tienda == tienda.Id
                             && p.Estado == "Entregado")  // Agregar filtro por estado "Entregado"
                    .ToListAsync();

                // Agrupar por mes y contar los pedidos
                var cantidadPorMes = pedidos
                    .GroupBy(p => new { p.Factura.Fecha.Year, p.Factura.Fecha.Month })
                    .Select(g => new
                    {
                        Mes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM", new CultureInfo("es-ES")),
                        CantidadPedidos = g.Count()
                    })
                    .ToList();

                return Ok(cantidadPorMes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se produjo un error en el servidor: {ex.Message}");
            }
        }

        // Nuevo endpoint para obtener pedidos por clientusername y estado (opcional)
        // Nuevo endpoint para obtener pedidos por clientusername y estado (opcional), incluyendo datos de la factura
        [HttpGet("cliente/{clientusername}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosByClientUsernameAndEstado(string clientusername, [FromQuery] string? estado = null)
        {
            var query = _context.Pedidos
                                .Include(p => p.Factura) // Incluye los datos de la factura
                                .Include(p=>p.Factura.Cliente)
                                .Include(p => p.ClienteUsuario)
                                .Where(p => p.Username_Cliente == clientusername)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            var pedidos = await query.ToListAsync();

            if (pedidos == null || pedidos.Count == 0)
            {
                return NotFound();
            }

            return pedidos;
        }







    }
}
