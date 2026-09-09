namespace NeptunoApp.Models
{
    public class Proveedor
    {
        public int ProveedorID { get; set; }
        public string CompaniaNombre { get; set; } = string.Empty;
        public string NombreContacto { get; set; } = string.Empty;
        public string CargoContacto { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
    }
}