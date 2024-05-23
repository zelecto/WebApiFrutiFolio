using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;

namespace WebApiFrutiFolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiendaVirtualsController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public TiendaVirtualsController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/TiendaVirtuals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TiendaVirtual>>> GetTiendasVirtuales()
        {
            return await _context.TiendasVirtuales.ToListAsync();
        }

        // GET: api/TiendaVirtuals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TiendaVirtual>> GetTiendaVirtual(int id)
        {
            var tiendaVirtual = await _context.TiendasVirtuales.FindAsync(id);

            if (tiendaVirtual == null)
            {
                return NotFound();
            }

            return tiendaVirtual;
        }

        // PUT: api/TiendaVirtuals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTiendaVirtual(int id, TiendaVirtual tiendaVirtual)
        {
            if (id != tiendaVirtual.Id)
            {
                return BadRequest();
            }

            _context.Entry(tiendaVirtual).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TiendaVirtualExists(id))
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

        // POST: api/TiendaVirtuals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TiendaVirtual>> PostTiendaVirtual(TiendaVirtual tiendaVirtual)
        {
            _context.TiendasVirtuales.Add(tiendaVirtual);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTiendaVirtual", new { id = tiendaVirtual.Id }, tiendaVirtual);
        }

        // DELETE: api/TiendaVirtuals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTiendaVirtual(int id)
        {
            var tiendaVirtual = await _context.TiendasVirtuales.FindAsync(id);
            if (tiendaVirtual == null)
            {
                return NotFound();
            }

            _context.TiendasVirtuales.Remove(tiendaVirtual);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/TiendaVirtuals/by-username/{nombreUsuario}
        [HttpGet("username/{nombreUsuario}")]
        public async Task<ActionResult<TiendaVirtual>> GetTiendaVirtualByNombreUsuario(string nombreUsuario)
        {
            var tiendaVirtual = await _context.TiendasVirtuales
                                              .FirstOrDefaultAsync(t => t.Username == nombreUsuario);

            if (tiendaVirtual == null)
            {
                return NotFound();
            }

            return tiendaVirtual;
        }

        [HttpGet("by-username/{nombreUsuario}/pedidos")]
        public async Task<ActionResult<TiendaVirtualPedidoSummary>> GetTiendaYPedidosPorUsuarioYFecha(
    string nombreUsuario,
    [FromQuery] string fecha,
    [FromQuery] string estado = "completado")
        {
            // Buscar la tienda por nombre de usuario
            var tiendaVirtual = await _context.TiendasVirtuales
                                              .FirstOrDefaultAsync(t => t.Username == nombreUsuario);

            if (tiendaVirtual == null)
            {
                return NotFound("Tienda no encontrada");
            }

            // Validar y convertir la fecha
            if (!DateOnly.TryParse(fecha, out DateOnly fechaParsed))
            {
                return BadRequest("Formato de fecha inválido");
            }

            // Buscar los pedidos según el estado y la fecha especificada
            var pedidos = await _context.Pedidos
                                        .Include(p => p.Factura)
                                        .Where(p => p.Id_Tienda == tiendaVirtual.Id &&
                                                    p.Factura.Fecha == fechaParsed &&
                                                    p.Estado == estado)
                                        .ToListAsync();

            // Calcular el total de pedidos y la suma de cobros
            var totalPedidos = pedidos.Count;
            var sumaTotalCobros = pedidos.Sum(p => p.Factura.Preciototal);

            var resultado = new TiendaVirtualPedidoSummary
            {
                Tienda = tiendaVirtual,
                TotalPedidos = totalPedidos,
                SumaTotalCobros = sumaTotalCobros
            };

            return Ok(resultado);
        }





        private bool TiendaVirtualExists(int id)
        {
            return _context.TiendasVirtuales.Any(e => e.Id == id);
        }
    }
}
