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
                    Descripcion = row["Descripcion"].ToString()
                });
            }
        }

        private void GuardarCategoria(object? parameter)
        {
            if (CategoriaSeleccionada == null) return;

            string opcion = CategoriaSeleccionada.CategoriaID == 0 ? "C" : "U";

            DatabaseHelper.ExecuteNonQuery("SP_Categorias_Crud", new SqlParameter[]
            {
                new SqlParameter("@Opcion", opcion),
                new SqlParameter("@CategoriaID", CategoriaSeleccionada.CategoriaID),
                new SqlParameter("@NombreCategoria", CategoriaSeleccionada.NombreCategoria),
                new SqlParameter("@Descripcion", CategoriaSeleccionada.Descripcion)
            });

            MessageBox.Show("Categoría guardada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            CargarCategorias();
            LimpiarFormulario(null);
        }

        private void EliminarCategoria(object? parameter)
        {
            if (CategoriaSeleccionada == null) return;

            var res = MessageBox.Show("¿Eliminar categoría?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery("SP_Categorias_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", "D"),
                    new SqlParameter("@CategoriaID", CategoriaSeleccionada.CategoriaID)
                });
                CargarCategorias();
            }
        }

        private void LimpiarFormulario(object? parameter)
        {
            CategoriaSeleccionada = new Categoria();
        }
    }
}