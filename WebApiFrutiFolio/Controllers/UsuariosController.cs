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
    public class UsuariosController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public UsuariosController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(string id, Usuario usuario)
        {
            if (id != usuario.Username)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Validar Cedula
            if (string.IsNullOrEmpty(usuario.Cedula))
            {
                return BadRequest("La cédula del usuario es obligatoria.");
            }
            else if (usuario.Cedula.Length != 10)
            {
                return BadRequest("La cédula del usuario debe tener exactamente 10 caracteres.");
            }

            // Validar Nombre
            if (string.IsNullOrEmpty(usuario.Nombre))
            {
                return BadRequest("El nombre del usuario es obligatorio.");
            }
            else if (usuario.Nombre.Length < 1 || usuario.Nombre.Length > 50)
            {
                return BadRequest("El nombre del usuario debe tener entre 1 y 50 caracteres.");
            }

            // Validar Correo
            if (!string.IsNullOrEmpty(usuario.Correo))
            {
                if (usuario.Correo.Length > 100)
                {
                    return BadRequest("El correo del usuario no puede tener más de 100 caracteres.");
                }
                else if (!IsValidEmail(usuario.Correo))
                {
                    return BadRequest("El formato del correo del usuario no es válido.");
                }
            }

            // Validar Nombre Usuario
            if (string.IsNullOrEmpty(usuario.Username))
            {
                return BadRequest("El user name es obligatorio.");
            }
            else if (usuario.Username.Length < 1 || usuario.Username.Length > 50)
            {
                return BadRequest("El user name debe tener entre 1 y 50 caracteres.");
            }

            // Validar Contraseña
            if (string.IsNullOrEmpty(usuario.Password))
            {
                return BadRequest("La contraseña del usuario es obligatoria.");
            }
            else if (usuario.Password.Length < 8 || usuario.Password.Length > 50)
            {
                return BadRequest("La contraseña del usuario debe tener entre 8 y 50 caracteres.");
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Username }, usuario);
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



        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Username == id);
        }
    }
}
