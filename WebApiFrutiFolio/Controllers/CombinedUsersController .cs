using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;
//JWT
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using System.Text;
using Azure.Core;

namespace WebApiFrutiFolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombinedUsersController : ControllerBase
    {
        private readonly FruityFolioContext _context;
        private readonly string secretKey;

        public CombinedUsersController(FruityFolioContext context, IConfiguration configuration)
        {
            _context = context;
            secretKey = configuration.GetSection("settings").GetSection("secretKey").ToString();
        }

        // GET: api/CombinedUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCombinedUser(string id, [FromQuery] string contraseña)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(contraseña))
            {
                return BadRequest("El id y la contraseña son requeridos.");
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                if (contraseña == usuario.Password)
                {
                    string token = GenerarToken(usuario.Username);
                    return new { Tipo = "Usuario", cuenta = usuario, Token=token };
                }
                else
                {
                    return Unauthorized("Contraseña incorrecta.");
                }
            }

            var clienteUsuario = await _context.ClienteUsuarios.FindAsync(id);
            if (clienteUsuario != null)
            {
                if (contraseña == usuario.Password)
                {
                    return new { Tipo = "ClienteUsuario", cuenta = clienteUsuario };
                }
                else
                {
                    return Unauthorized("Contraseña incorrecta.");
                }
                
            }

            return NotFound();
        }

        private string GenerarToken(string username)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokencreado = tokenHandler.WriteToken(tokenConfig);
            return tokencreado;
        }

    }
}
