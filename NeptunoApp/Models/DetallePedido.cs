namespace NeptunoApp.Models
{
    public class DetallePedido
    {
        public int PedidoID { get; set; }
        public int ProductoID { get; set; }
        public decimal PrecioUnidad { get; set; }
        public short Cantidad { get; set; }
        public decimal Descuento { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}