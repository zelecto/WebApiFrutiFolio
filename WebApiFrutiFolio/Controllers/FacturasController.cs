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
    [Authorize]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public FacturasController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            // Incluir los datos del cliente asociado a cada factura
            var facturas = await _context.Facturas
                                        .Include(f => f.Cliente) // Cambiar el nombre de la propiedad de navegación
                                        .ToListAsync();

            return facturas;
        }


        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound();
            }

            return factura;
        }

        // PUT: api/Facturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.Id)
            {
                return BadRequest();
            }

            // Verificar si se proporcionó el cliente en la solicitud
            if (factura.Cliente != null)
            {
                // Verificar si el cliente ya existe
                var existingCliente = await _context.Clientes.FirstOrDefaultAsync(c =>
                    c.Cedula == factura.Cliente.Cedula &&
                    c.Nombre == factura.Cliente.Nombre &&
                    c.Correo == factura.Cliente.Correo);

                if (existingCliente != null)
                {
                    // Asignar el cliente existente a la factura
                    factura.ClienteCedula = existingCliente.Cedula;
                }
                else
                {
                    // Si el cliente no existe, agregarlo al contexto (si no está adjunto)
                    var entry = _context.Entry(factura.Cliente);
                    if (entry.State == EntityState.Detached)
                    {
                        _context.Clientes.Add(factura.Cliente);
                    }
                }
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
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


        // POST: api/Facturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            // Verificar si el cliente ya existe en la base de datos
            var existingCliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Cedula == factura.Cliente.Cedula);

            if (existingCliente != null)
            {
                // Asignar el cliente existente a la factura
                factura.Cliente = existingCliente;
            }
            else
            {
                // Si el cliente no existe, agregarlo al contexto (si no está adjunto)
                var entry = _context.Entry(factura.Cliente);
                if (entry.State == EntityState.Detached)
                {
                    _context.Clientes.Add(factura.Cliente);
                }
            }

            // Agregar la factura al contexto y guardar los cambios en la base de datos
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFactura", new { id = factura.Id }, factura);
        }


        // Método para validar formato de correo electrónico
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // GET: api/Facturas/ByUser/{username}
        [HttpGet("ByUser/{username}")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasByUser(string username, [FromQuery] DateOnly? fechaInicio, [FromQuery] DateOnly? fechaFinal)
        {
            // Buscar el usuario en la base de datos por su nombre
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound($"No se encontró ningún usuario con el nombre '{username}'.");
            }

            // Crear la consulta para obtener las facturas del usuario
            var query = _context.Facturas
                                .Include(f => f.Cliente)
                                .Where(f => f.UsuarioUsername == username)
                                .AsQueryable();

            // Aplicar el filtro de rango de fechas si se proporcionaron
            if (fechaInicio.HasValue)
            {
                query = query.Where(f => f.Fecha >= fechaInicio.Value);
            }

            if (fechaFinal.HasValue)
            {
                query = query.Where(f => f.Fecha <= fechaFinal.Value);
            }

            // Ejecutar la consulta
            var facturas = await query.ToListAsync();

            if (facturas.Count == 0)
            {
                return NotFound($"No se encontraron facturas para el usuario '{username}' en el rango de fechas proporcionado.");
            }

            return facturas;
        }





        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("IngresosPorDia")]
        public async Task<ActionResult<IEnumerable<object>>> GetIngresosPorDia(
    [FromQuery] DateOnly fechaInicio,
    [FromQuery] DateOnly fechaFin,
    [FromQuery] string username)
        {
            // Verificar si el nombre de usuario es válido
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound($"No se encontró ningún usuario con el nombre '{username}'.");
            }

            // Filtrar facturas por rango de fechas y nombre de usuario
            var query = _context.Facturas
                                .Where(f => f.Fecha >= fechaInicio && f.Fecha <= fechaFin && f.UsuarioUsername == username);

            // Obtener las facturas y calcular el total de ingresos por día
            var facturas = await query.ToListAsync();

            var ingresosPorDia = facturas
                .GroupBy(f => f.Fecha)
                .Select(g => new
                {
                    Dia = g.Key.Day, // Formato de fecha
                    TotalIngresos = g.Sum(f => f.Preciototal)
                })
                .ToList();

            return Ok(ingresosPorDia);
        }




        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }
    }
}
