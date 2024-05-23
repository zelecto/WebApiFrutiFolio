using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models
{
    public partial class ClienteUsuario
    {
        public string Username { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
        public string DireccionResidencia { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}

