using System.ComponentModel.DataAnnotations;

namespace NeptunoApp.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(60, ErrorMessage = "El nombre no puede exceder 60 caracteres")]
        public string NombreProducto { get; set; } = string.Empty;

        public int? ProveedorID { get; set; }
        public int? CategoriaID { get; set; }
        public string CantidadPorUnidad { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnidad { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public short UnidadesEnExistencia { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Valor inválido")]
        public short UnidadesEnPedido { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Valor inválido")]
        public short NivelDeReorden { get; set; }

        public bool Descontinuado { get; set; }

        // Propiedades de navegación
        public string NombreCategoria { get; set; } = string.Empty;
        public string CompaniaNombre { get; set; } = string.Empty;
    }
}