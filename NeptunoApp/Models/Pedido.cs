namespace NeptunoApp.Models
{
    public class Pedido
    {
        public int PedidoID { get; set; }
        public int? ClienteID { get; set; }
        public int? EmpleadoID { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaRequerida { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public int? TransportistaID { get; set; }
        public string Destinatario { get; set; } = string.Empty;
        public string CiudadDestino { get; set; } = string.Empty;
        public string PaisDestino { get; set; } = string.Empty;

        // Propiedades de navegación
        public string Empresa { get; set; } = string.Empty;
        public string EmpleadoNombre { get; set; } = string.Empty;
        public string Transportista { get; set; } = string.Empty;
    }
}