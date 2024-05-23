using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models;

public partial class Factura
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal Preciototal { get; set; }
    [JsonIgnore]
    public int ClienteCedula { get; set; }
    public string UsuarioUsername { get; set; } = null!;
    public virtual Cliente? Cliente { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<DetallesProductosVendido>? DetallesProductosVendidos { get; set; } = new List<DetallesProductosVendido>();
    [JsonIgnore]
    public virtual Usuario? UsuarioUsernameNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Pedido>? Pedidos { get; set; } // Ahora es opcional
}
