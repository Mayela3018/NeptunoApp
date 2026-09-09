using NeptunoApp.Models;
using NeptunoApp.Data;
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

        private string _filtroNombre = string.Empty;
        public string FiltroNombre
        {
            get => _filtroNombre;
            set
            {
                if (SetProperty(ref _filtroNombre, value))
                    BuscarProveedores();
            }
        }

        private string _filtroCiudad = string.Empty;
        public string FiltroCiudad
        {
            get => _filtroCiudad;
            set
            {
                if (SetProperty(ref _filtroCiudad, value))
                    BuscarProveedores();
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
            Proveedores.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_Proveedores_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Proveedores.Add(new Proveedor
                {
                    ProveedorID = (int)row["ProveedorID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString(),
                    NombreContacto = row["NombreContacto"].ToString(),
                    Ciudad = row["Ciudad"].ToString(),
                    Telefono = row["Telefono"].ToString()
                });
            }
        }

        private void BuscarProveedores()
        {
            Proveedores.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_Proveedores_Buscar",
                new SqlParameter[]
                {
                    new SqlParameter("@NombreContacto", string.IsNullOrEmpty(FiltroNombre) ? (object)DBNull.Value : FiltroNombre),
                    new SqlParameter("@Ciudad", string.IsNullOrEmpty(FiltroCiudad) ? (object)DBNull.Value : FiltroCiudad)
                });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Proveedores.Add(new Proveedor
                {
                    ProveedorID = (int)row["ProveedorID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString(),
                    NombreContacto = row["NombreContacto"].ToString(),
                    Ciudad = row["Ciudad"].ToString(),
                    Telefono = row["Telefono"].ToString()
                });
            }
        }

        private void GuardarProveedor(object? parameter)
        {
            try
            {
                if (ProveedorSeleccionado == null) return;

                string opcion = ProveedorSeleccionado.ProveedorID == 0 ? "C" : "U";

                DatabaseHelper.ExecuteNonQuery("SP_Proveedores_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", opcion),
                    new SqlParameter("@ProveedorID", ProveedorSeleccionado.ProveedorID),
                    new SqlParameter("@CompaniaNombre", ProveedorSeleccionado.CompaniaNombre),
                    new SqlParameter("@NombreContacto", ProveedorSeleccionado.NombreContacto),
                    new SqlParameter("@CargoContacto", ProveedorSeleccionado.CargoContacto ?? (object)DBNull.Value),
                    new SqlParameter("@Direccion", ProveedorSeleccionado.Direccion ?? (object)DBNull.Value),
                    new SqlParameter("@Ciudad", ProveedorSeleccionado.Ciudad ?? (object)DBNull.Value),
                    new SqlParameter("@CodigoPostal", ProveedorSeleccionado.CodigoPostal ?? (object)DBNull.Value),
                    new SqlParameter("@Pais", ProveedorSeleccionado.Pais ?? (object)DBNull.Value),
                    new SqlParameter("@Telefono", ProveedorSeleccionado.Telefono ?? (object)DBNull.Value),
                    new SqlParameter("@Fax", ProveedorSeleccionado.Fax ?? (object)DBNull.Value)
                });

                MessageBox.Show("Proveedor guardado correctamente", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CargarProveedores();
                LimpiarFormulario(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProveedor(object? parameter)
        {
            if (ProveedorSeleccionado == null)
            {
                MessageBox.Show("Seleccione un proveedor para eliminar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Eliminar el proveedor '{ProveedorSeleccionado.CompaniaNombre}'?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                    MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LimpiarFormulario(object? parameter)
        {
            ProveedorSeleccionado = new Proveedor();
        }
    }
}