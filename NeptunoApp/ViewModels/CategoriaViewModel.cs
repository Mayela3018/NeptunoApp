using System;
using NeptunoApp.Models;
using NeptunoApp.Data;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace NeptunoApp.ViewModels
{
    public class CategoriaViewModel : ViewModelBase
    {
        public ObservableCollection<Categoria> Categorias { get; set; }

        private Categoria _categoriaSeleccionada;
        public Categoria CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set => SetProperty(ref _categoriaSeleccionada, value);
        }

        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }
        public RelayCommand LimpiarCommand { get; }

        public CategoriaViewModel()
        {
            Categorias = new ObservableCollection<Categoria>();
            GuardarCommand = new RelayCommand(GuardarCategoria);
            EliminarCommand = new RelayCommand(EliminarCategoria);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            Categorias.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_Categorias_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Categorias.Add(new Categoria
                {
                    CategoriaID = (int)row["CategoriaID"],
                    NombreCategoria = row["NombreCategoria"].ToString(),
                    Descripcion = row["Descripcion"]?.ToString() ?? string.Empty
                });
            }
        }

        private void GuardarCategoria(object? parameter)
        {
            // 1. Validar que haya un objeto seleccionado
            if (CategoriaSeleccionada == null)
            {
                MessageBox.Show("No hay una categoría seleccionada", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validar que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(CategoriaSeleccionada.NombreCategoria))
            {
                MessageBox.Show("El nombre de la categoría es obligatorio", "Error de validación",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 3. Determinar si es Crear o Actualizar
                string opcion = CategoriaSeleccionada.CategoriaID == 0 ? "C" : "U";

                // 4. Ejecutar el Stored Procedure
                DatabaseHelper.ExecuteNonQuery("SP_Categorias_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", opcion),
                    new SqlParameter("@CategoriaID", CategoriaSeleccionada.CategoriaID),
                    new SqlParameter("@NombreCategoria", CategoriaSeleccionada.NombreCategoria),
                    // Manejo seguro de nulos para la descripción
                    new SqlParameter("@Descripcion", string.IsNullOrWhiteSpace(CategoriaSeleccionada.Descripcion) ? (object)DBNull.Value : CategoriaSeleccionada.Descripcion)
                });

                string mensaje = opcion == "C" ? "Categoría creada correctamente" : "Categoría actualizada correctamente";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                CargarCategorias();
                LimpiarFormulario(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la categoría: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarCategoria(object? parameter)
        {
            if (CategoriaSeleccionada == null || CategoriaSeleccionada.CategoriaID == 0)
            {
                MessageBox.Show("Seleccione una categoría de la tabla para eliminar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = MessageBox.Show($"¿Está seguro de eliminar la categoría '{CategoriaSeleccionada.NombreCategoria}'?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("SP_Categorias_Crud", new SqlParameter[]
                    {
                        new SqlParameter("@Opcion", "D"),
                        new SqlParameter("@CategoriaID", CategoriaSeleccionada.CategoriaID)
                    });

                    MessageBox.Show("Categoría eliminada correctamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CargarCategorias();
                    LimpiarFormulario(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}. Es posible que esté asociada a productos.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LimpiarFormulario(object? parameter)
        {
            CategoriaSeleccionada = new Categoria
            {
                CategoriaID = 0,
                NombreCategoria = string.Empty,
                Descripcion = string.Empty
            };
        }
    }
}