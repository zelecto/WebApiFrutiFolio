using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models
{
    public partial class TiendaVirtual
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
        public string Direccion { get; set; } = null!;

        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Pedido>? Pedidos { get; set; } = new List<Pedido>();
        [JsonIgnore]
        public virtual Ciudad? CiudadNavigation { get; set; } = default!;
    }
}

