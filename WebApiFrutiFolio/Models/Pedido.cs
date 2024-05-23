using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models
{
    public partial class Pedido
    {
        public int Id { get; set; }
        public string Estado { get; set; } = null!;
        
        public int Id_Factura { get; set; }
        
        public int Id_Tienda { get; set; }
        public decimal? PrecioTransporte { get; set; }
        
        public string? Username_Cliente { get; set; }

        
        public virtual Factura? Factura { get; set; } = null!;
        
        public virtual TiendaVirtual? TiendaVirtual { get; set; } = null!;
        
        public virtual ClienteUsuario? ClienteUsuario { get; set; } = null!;
    }
}
