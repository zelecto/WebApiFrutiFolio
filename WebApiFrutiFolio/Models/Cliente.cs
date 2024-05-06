using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models;

public partial class Cliente
{
    public int Cedula { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }

    [JsonIgnore]
    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
