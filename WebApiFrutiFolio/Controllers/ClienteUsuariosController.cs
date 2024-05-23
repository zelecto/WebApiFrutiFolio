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
    public class ClienteUsuariosController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public ClienteUsuariosController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/ClienteUsuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteUsuario>>> GetClienteUsuarios()
        {
            return await _context.ClienteUsuarios.ToListAsync();
        }

        // GET: api/ClienteUsuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteUsuario>> GetClienteUsuario(string id)
        {
            var clienteUsuario = await _context.ClienteUsuarios.FindAsync(id);

            if (clienteUsuario == null)
            {
                return NotFound();
            }

            return clienteUsuario;
        }

        // PUT: api/ClienteUsuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClienteUsuario(string id, ClienteUsuario clienteUsuario)
        {
            if (id != clienteUsuario.Username)
            {
                return BadRequest();
            }

            _context.Entry(clienteUsuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteUsuarioExists(id))
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

        // POST: api/ClienteUsuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClienteUsuario>> PostClienteUsuario(ClienteUsuario clienteUsuario)
        {
            _context.ClienteUsuarios.Add(clienteUsuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ClienteUsuarioExists(clienteUsuario.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetClienteUsuario", new { id = clienteUsuario.Username }, clienteUsuario);
        }

        // DELETE: api/ClienteUsuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClienteUsuario(string id)
        {
            var clienteUsuario = await _context.ClienteUsuarios.FindAsync(id);
            if (clienteUsuario == null)
            {
                return NotFound();
            }

            _context.ClienteUsuarios.Remove(clienteUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteUsuarioExists(string id)
        {
            return _context.ClienteUsuarios.Any(e => e.Username == id);
        }
    }
}
