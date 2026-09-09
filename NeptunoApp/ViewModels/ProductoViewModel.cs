using NeptunoApp.Models;
using NeptunoApp.Data;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace NeptunoApp.ViewModels
{
    public class ProductoViewModel : ViewModelBase
    {
        public ObservableCollection<Producto> Productos { get; set; }
        public ObservableCollection<Categoria> Categorias { get; set; }
        public ObservableCollection<Proveedor> Proveedores { get; set; }

        private Producto _productoSeleccionado;
        public Producto ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set => SetProperty(ref _productoSeleccionado, value);
        }

        private string _tituloFormulario = "Nuevo Producto";
        public string TituloFormulario
        {
            get => _tituloFormulario;
            set => SetProperty(ref _tituloFormulario, value);
        }

        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }
        public RelayCommand LimpiarCommand { get; }

        public ProductoViewModel()
        {
            Productos = new ObservableCollection<Producto>();
            Categorias = new ObservableCollection<Categoria>();
            Proveedores = new ObservableCollection<Proveedor>();

            GuardarCommand = new RelayCommand(GuardarProducto);
            EliminarCommand = new RelayCommand(EliminarProducto);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);

            CargarDatos();
        }

        private void CargarDatos()
        {
            CargarProductos();
            CargarCombos();
        }

        private void CargarProductos()
        {
            Productos.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_Productos_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Productos.Add(new Producto
                {
                    ProductoID = (int)row["ProductoID"],
                    NombreProducto = row["NombreProducto"].ToString(),
                    ProveedorID = row["ProveedorID"] as int?,
                    CategoriaID = row["CategoriaID"] as int?,
                    CantidadPorUnidad = row["CantidadPorUnidad"].ToString(),
                    PrecioUnidad = (decimal)row["PrecioUnidad"],
                    UnidadesEnExistencia = (short)row["UnidadesEnExistencia"],
                    NombreCategoria = row["NombreCategoria"].ToString(),
                    CompaniaNombre = row["CompaniaNombre"].ToString()
                });
            }
        }

        private void CargarCombos()
        {
            Categorias.Clear();
            var dtCat = DatabaseHelper.ExecuteQuery("SP_Categorias_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dtCat.Rows)
                Categorias.Add(new Categoria
                {
                    CategoriaID = (int)row["CategoriaID"],
                    NombreCategoria = row["NombreCategoria"].ToString()
                });

            Proveedores.Clear();
            var dtProv = DatabaseHelper.ExecuteQuery("SP_Proveedores_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dtProv.Rows)
                Proveedores.Add(new Proveedor
                {
                    ProveedorID = (int)row["ProveedorID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString()
                });
        }

        private void GuardarProducto(object? parameter)
        {
            try
            {
                if (ProductoSeleccionado == null) return;

                string opcion = ProductoSeleccionado.ProductoID == 0 ? "C" : "U";

                DatabaseHelper.ExecuteNonQuery("SP_Productos_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", opcion),
                    new SqlParameter("@ProductoID", ProductoSeleccionado.ProductoID),
                    new SqlParameter("@NombreProducto", ProductoSeleccionado.NombreProducto),
                    new SqlParameter("@ProveedorID", ProductoSeleccionado.ProveedorID ?? (object)DBNull.Value),
                    new SqlParameter("@CategoriaID", ProductoSeleccionado.CategoriaID ?? (object)DBNull.Value),
                    new SqlParameter("@CantidadPorUnidad", ProductoSeleccionado.CantidadPorUnidad ?? (object)DBNull.Value),
                    new SqlParameter("@PrecioUnidad", ProductoSeleccionado.PrecioUnidad),
                    new SqlParameter("@UnidadesEnExistencia", ProductoSeleccionado.UnidadesEnExistencia),
                    new SqlParameter("@UnidadesEnPedido", ProductoSeleccionado.UnidadesEnPedido),
                    new SqlParameter("@NivelDeReorden", ProductoSeleccionado.NivelDeReorden),
                    new SqlParameter("@Descontinuado", ProductoSeleccionado.Descontinuado)
                });

                MessageBox.Show("Producto guardado correctamente", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CargarProductos();
                LimpiarFormulario(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProducto(object? parameter)
        {
            if (ProductoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto para eliminar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Eliminar el producto '{ProductoSeleccionado.NombreProducto}'?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("SP_Productos_Crud", new SqlParameter[]
                    {
                        new SqlParameter("@Opcion", "D"),
                        new SqlParameter("@ProductoID", ProductoSeleccionado.ProductoID)
                    });

                    MessageBox.Show("Producto eliminado correctamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CargarProductos();
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
            ProductoSeleccionado = new Producto();
            TituloFormulario = "Nuevo Producto";
        }

        public void EditarProducto(Producto producto)
        {
            ProductoSeleccionado = new Producto
            {
                ProductoID = producto.ProductoID,
                NombreProducto = producto.NombreProducto,
                ProveedorID = producto.ProveedorID,
                CategoriaID = producto.CategoriaID,
                CantidadPorUnidad = producto.CantidadPorUnidad,
                PrecioUnidad = producto.PrecioUnidad,
                UnidadesEnExistencia = producto.UnidadesEnExistencia,
                UnidadesEnPedido = producto.UnidadesEnPedido,
                NivelDeReorden = producto.NivelDeReorden,
                Descontinuado = producto.Descontinuado
            };
            TituloFormulario = "Editar Producto";
        }
    }
}