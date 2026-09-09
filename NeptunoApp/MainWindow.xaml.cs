using NeptunoApp.ViewModels;
using NeptunoApp.Views;
using System.Windows;
using System.Windows.Controls;

namespace NeptunoApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Cargar vista por defecto
            CargarVista(new VistaProductos(), new ProductoViewModel());
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
            => CargarVista(new VistaProductos(), new ProductoViewModel());

        private void BtnCategorias_Click(object sender, RoutedEventArgs e)
            => CargarVista(new VistaCategorias(), new CategoriaViewModel());

        private void BtnProveedores_Click(object sender, RoutedEventArgs e)
            => CargarVista(new VistaProveedores(), new ProveedorViewModel());

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
            => CargarVista(new VistaPedidos(), new PedidoViewModel());

        private void CargarVista(UserControl vista, object viewModel)
        {
            MainContent.Content = vista;
            MainContent.DataContext = viewModel;
        }
    }
}