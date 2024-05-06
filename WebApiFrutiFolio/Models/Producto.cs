using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models;

public partial class Producto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = "Sin descripcion";

    public int Stock { get; set; }

    public decimal Price { get; set; }

    public string? Img { get; set; }

    public bool Activo { get; set; }

    public string Username { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<DetallesProductosVendido>? DetallesProductosVendidos { get; set; }
    [JsonIgnore]
    public virtual Usuario? UsuarioUsernameNavigation { get; set; } 
}
