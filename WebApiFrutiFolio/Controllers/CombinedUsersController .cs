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
    public class CombinedUsersController : ControllerBase
    {
        private readonly FruityFolioContext _context;

        public CombinedUsersController(FruityFolioContext context)
        {
            _context = context;
        }

        // GET: api/CombinedUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCombinedUser(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                return new { Tipo = "Usuario", cuenta = usuario };
            }

            var clienteUsuario = await _context.ClienteUsuarios.FindAsync(id);
            if (clienteUsuario != null)
            {
                return new { Tipo = "ClienteUsuario", cuenta = clienteUsuario };
            }

            return NotFound();
        }
    }
}
