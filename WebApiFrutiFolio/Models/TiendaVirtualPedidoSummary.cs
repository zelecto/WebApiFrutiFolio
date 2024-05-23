namespace WebApiFrutiFolio.Models
{
    public class TiendaVirtualPedidoSummary
    {
        public TiendaVirtual Tienda { get; set; }
        public int TotalPedidos { get; set; }
        public decimal SumaTotalCobros { get; set; }
    }
}
