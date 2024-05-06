using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models;

public partial class Usuario
{
    public string Username { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Password { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Factura>? Facturas { get; set; } = new List<Factura>();
    [JsonIgnore]
    public virtual ICollection<Producto>? Productos { get; set; } = new List<Producto>();
}
