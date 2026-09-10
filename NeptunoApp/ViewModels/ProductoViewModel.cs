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
                    CantidadPorUnidad = row["CantidadPorUnidad"]?.ToString() ?? string.Empty,
                    PrecioUnidad = (decimal)row["PrecioUnidad"],
                    UnidadesEnExistencia = (short)row["UnidadesEnExistencia"],
                    UnidadesEnPedido = (short)row["UnidadesEnPedido"],
                    NivelDeReorden = (short)row["NivelDeReorden"],
                    Descontinuado = (bool)row["Descontinuado"],
                    NombreCategoria = row["NombreCategoria"]?.ToString() ?? string.Empty,
                    CompaniaNombre = row["CompaniaNombre"]?.ToString() ?? string.Empty
                });
            }
        }

        private void CargarCombos()
        {
            Categorias.Clear();
            var dtCat = DatabaseHelper.ExecuteQuery("SP_Categorias_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dtCat.Rows)
            {
                Categorias.Add(new Categoria
                {
                    CategoriaID = (int)row["CategoriaID"],
                    NombreCategoria = row["NombreCategoria"].ToString()
                });
            }

            Proveedores.Clear();
            var dtProv = DatabaseHelper.ExecuteQuery("SP_Proveedores_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dtProv.Rows)
            {
                Proveedores.Add(new Proveedor
                {
                    ProveedorID = (int)row["ProveedorID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString()
                });
            }
        }

        private void GuardarProducto(object? parameter)
        {
            try
            {
                if (ProductoSeleccionado == null)
                {
                    MessageBox.Show("No hay un producto seleccionado", "Advertencia",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ProductoSeleccionado.NombreProducto))
                {
                    MessageBox.Show("El nombre del producto es obligatorio", "Error de validación",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ProductoSeleccionado.PrecioUnidad <= 0)
                {
                    MessageBox.Show("El precio debe ser mayor a 0. Verifique el campo 'Precio Unidad'",
                        "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ProductoSeleccionado.UnidadesEnExistencia <= 0)
                {
                    MessageBox.Show("El stock es obligatorio y debe ser mayor a 0",
                        "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ProductoSeleccionado.NivelDeReorden < 0)
                {
                    MessageBox.Show("El nivel de reorden no puede ser negativo",
                        "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ProductoSeleccionado.CategoriaID == null || ProductoSeleccionado.CategoriaID == 0)
                {
                    MessageBox.Show("Debe seleccionar una categoría de la lista",
                        "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ProductoSeleccionado.ProveedorID == null || ProductoSeleccionado.ProveedorID == 0)
                {
                    MessageBox.Show("Debe seleccionar un proveedor de la lista",
                        "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string opcion = ProductoSeleccionado.ProductoID == 0 ? "C" : "U";

                DatabaseHelper.ExecuteNonQuery("SP_Productos_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", opcion),
                    new SqlParameter("@ProductoID", ProductoSeleccionado.ProductoID),
                    new SqlParameter("@NombreProducto", ProductoSeleccionado.NombreProducto),
                    new SqlParameter("@ProveedorID", ProductoSeleccionado.ProveedorID ?? (object)DBNull.Value),
                    new SqlParameter("@CategoriaID", ProductoSeleccionado.CategoriaID ?? (object)DBNull.Value),
                    new SqlParameter("@CantidadPorUnidad", string.IsNullOrWhiteSpace(ProductoSeleccionado.CantidadPorUnidad) ? (object)DBNull.Value : ProductoSeleccionado.CantidadPorUnidad),
                    new SqlParameter("@PrecioUnidad", ProductoSeleccionado.PrecioUnidad),
                    new SqlParameter("@UnidadesEnExistencia", ProductoSeleccionado.UnidadesEnExistencia),
                    new SqlParameter("@UnidadesEnPedido", ProductoSeleccionado.UnidadesEnPedido),
                    new SqlParameter("@NivelDeReorden", ProductoSeleccionado.NivelDeReorden),
                    new SqlParameter("@Descontinuado", ProductoSeleccionado.Descontinuado)
                });

                string mensaje = opcion == "C" ? "Producto creado correctamente" : "Producto actualizado correctamente";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                CargarProductos();
                LimpiarFormulario(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProducto(object? parameter)
        {
            if (ProductoSeleccionado == null || ProductoSeleccionado.ProductoID == 0)
            {
                MessageBox.Show("Seleccione un producto de la tabla para eliminar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Está seguro de eliminar el producto '{ProductoSeleccionado.NombreProducto}'?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
            ProductoSeleccionado = new Producto
            {
                ProductoID = 0,
                NombreProducto = string.Empty,
                CantidadPorUnidad = string.Empty,
                PrecioUnidad = 0,
                UnidadesEnExistencia = 0,
                UnidadesEnPedido = 0,
                NivelDeReorden = 0,
                Descontinuado = false,
                CategoriaID = null,
                ProveedorID = null
            };
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