using NeptunoApp.Models;
using NeptunoApp.Data;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace NeptunoApp.ViewModels
{
    public class ProveedorViewModel : ViewModelBase
    {
        public ObservableCollection<Proveedor> Proveedores { get; set; }

        private Proveedor _proveedorSeleccionado;
        public Proveedor ProveedorSeleccionado
        {
            get => _proveedorSeleccionado;
            set => SetProperty(ref _proveedorSeleccionado, value);
        }

        // Propiedades para el filtro de búsqueda (Requisito 11.a)
        private string _filtroNombre = string.Empty;
        public string FiltroNombre
        {
            get => _filtroNombre;
            set
            {
                if (SetProperty(ref _filtroNombre, value))
                {
                    BuscarProveedores(); // Busca automáticamente al escribir
                }
            }
        }

        private string _filtroCiudad = string.Empty;
        public string FiltroCiudad
        {
            get => _filtroCiudad;
            set
            {
                if (SetProperty(ref _filtroCiudad, value))
                {
                    BuscarProveedores(); // Busca automáticamente al escribir
                }
            }
        }

        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }
        public RelayCommand LimpiarCommand { get; }

        public ProveedorViewModel()
        {
            Proveedores = new ObservableCollection<Proveedor>();

            GuardarCommand = new RelayCommand(GuardarProveedor);
            EliminarCommand = new RelayCommand(EliminarProveedor);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);

            CargarProveedores();
        }

        private void CargarProveedores()
        {
            // Limpia los filtros al cargar todos
            _filtroNombre = string.Empty;
            _filtroCiudad = string.Empty;
            OnPropertyChanged(nameof(FiltroNombre));
            OnPropertyChanged(nameof(FiltroCiudad));

            Proveedores.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_Proveedores_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Proveedores.Add(new Proveedor
                {
                    ProveedorID = (int)row["ProveedorID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString(),
                    NombreContacto = row["NombreContacto"]?.ToString() ?? string.Empty,
                    CargoContacto = row["CargoContacto"]?.ToString() ?? string.Empty,
                    Direccion = row["Direccion"]?.ToString() ?? string.Empty,
                    Ciudad = row["Ciudad"]?.ToString() ?? string.Empty,
                    CodigoPostal = row["CodigoPostal"]?.ToString() ?? string.Empty,
                    Pais = row["Pais"]?.ToString() ?? string.Empty,
                    Telefono = row["Telefono"]?.ToString() ?? string.Empty,
                    Fax = row["Fax"]?.ToString() ?? string.Empty
                });
            }
        }

        // Requisito 11.a: Búsqueda de proveedores usando filtros
        private void BuscarProveedores()
        {
            Proveedores.Clear();

            // Preparamos los parámetros: si están vacíos, enviamos DBNull para que el SP ignore el filtro
            object nombreParam = string.IsNullOrWhiteSpace(FiltroNombre) ? (object)DBNull.Value : FiltroNombre;
            object ciudadParam = string.IsNullOrWhiteSpace(FiltroCiudad) ? (object)DBNull.Value : FiltroCiudad;

            var dt = DatabaseHelper.ExecuteQuery("SP_Proveedores_Buscar",
                new SqlParameter[]
                {
                    new SqlParameter("@NombreContacto", nombreParam),
                    new SqlParameter("@Ciudad", ciudadParam)
                });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Proveedores.Add(new Proveedor
                {
                    ProveedorID = (int)row["ProveedorID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString(),
                    NombreContacto = row["NombreContacto"]?.ToString() ?? string.Empty,
                    Ciudad = row["Ciudad"]?.ToString() ?? string.Empty,
                    Telefono = row["Telefono"]?.ToString() ?? string.Empty
                });
            }
        }

        private void GuardarProveedor(object? parameter)
        {
            // 1. Validar que haya un objeto seleccionado
            if (ProveedorSeleccionado == null)
            {
                MessageBox.Show("No hay un proveedor seleccionado", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validar campo obligatorio
            if (string.IsNullOrWhiteSpace(ProveedorSeleccionado.CompaniaNombre))
            {
                MessageBox.Show("El nombre de la compañía es obligatorio", "Error de validación",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 3. Determinar si es Crear o Actualizar
                string opcion = ProveedorSeleccionado.ProveedorID == 0 ? "C" : "U";

                // 4. Ejecutar el Stored Procedure con manejo seguro de nulos
                DatabaseHelper.ExecuteNonQuery("SP_Proveedores_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", opcion),
                    new SqlParameter("@ProveedorID", ProveedorSeleccionado.ProveedorID),
                    new SqlParameter("@CompaniaNombre", ProveedorSeleccionado.CompaniaNombre),
                    new SqlParameter("@NombreContacto", string.IsNullOrWhiteSpace(ProveedorSeleccionado.NombreContacto) ? (object)DBNull.Value : ProveedorSeleccionado.NombreContacto),
                    new SqlParameter("@CargoContacto", string.IsNullOrWhiteSpace(ProveedorSeleccionado.CargoContacto) ? (object)DBNull.Value : ProveedorSeleccionado.CargoContacto),
                    new SqlParameter("@Direccion", string.IsNullOrWhiteSpace(ProveedorSeleccionado.Direccion) ? (object)DBNull.Value : ProveedorSeleccionado.Direccion),
                    new SqlParameter("@Ciudad", string.IsNullOrWhiteSpace(ProveedorSeleccionado.Ciudad) ? (object)DBNull.Value : ProveedorSeleccionado.Ciudad),
                    new SqlParameter("@CodigoPostal", string.IsNullOrWhiteSpace(ProveedorSeleccionado.CodigoPostal) ? (object)DBNull.Value : ProveedorSeleccionado.CodigoPostal),
                    new SqlParameter("@Pais", string.IsNullOrWhiteSpace(ProveedorSeleccionado.Pais) ? (object)DBNull.Value : ProveedorSeleccionado.Pais),
                    new SqlParameter("@Telefono", string.IsNullOrWhiteSpace(ProveedorSeleccionado.Telefono) ? (object)DBNull.Value : ProveedorSeleccionado.Telefono),
                    new SqlParameter("@Fax", string.IsNullOrWhiteSpace(ProveedorSeleccionado.Fax) ? (object)DBNull.Value : ProveedorSeleccionado.Fax)
                });

                string mensaje = opcion == "C" ? "Proveedor creado correctamente" : "Proveedor actualizado correctamente";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                CargarProveedores(); // Recarga la lista completa
                LimpiarFormulario(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el proveedor: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProveedor(object? parameter)
        {
            if (ProveedorSeleccionado == null || ProveedorSeleccionado.ProveedorID == 0)
            {
                MessageBox.Show("Seleccione un proveedor de la tabla para eliminar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Está seguro de eliminar al proveedor '{ProveedorSeleccionado.CompaniaNombre}'?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("SP_Proveedores_Crud", new SqlParameter[]
                    {
                        new SqlParameter("@Opcion", "D"),
                        new SqlParameter("@ProveedorID", ProveedorSeleccionado.ProveedorID)
                    });

                    MessageBox.Show("Proveedor eliminado correctamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CargarProveedores();
                    LimpiarFormulario(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}. Es posible que esté asociado a productos.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LimpiarFormulario(object? parameter)
        {
            // Inicialización segura para evitar nulos en la interfaz
            ProveedorSeleccionado = new Proveedor
            {
                ProveedorID = 0,
                CompaniaNombre = string.Empty,
                NombreContacto = string.Empty,
                CargoContacto = string.Empty,
                Direccion = string.Empty,
                Ciudad = string.Empty,
                CodigoPostal = string.Empty,
                Pais = string.Empty,
                Telefono = string.Empty,
                Fax = string.Empty
            };
        }
    }
}