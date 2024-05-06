using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApiFrutiFolio.Models;

public partial class DetallesProductosVendido
{
    public int Id { get; set; }

    public int Cantidadvendida { get; set; }

    public decimal Subprecio { get; set; }

    public int Idfactura { get; set; }
    
    public int Idproducto { get; set; }
    [JsonIgnore]
    public virtual Factura? IdfacturaNavigation { get; set; } = null!;
    
    public virtual Producto? producto { get; set; } = null!;
}
