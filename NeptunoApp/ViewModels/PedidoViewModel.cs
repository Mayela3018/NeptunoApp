using NeptunoApp.Models;
using NeptunoApp.Data;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace NeptunoApp.ViewModels
{
    public class PedidoViewModel : ViewModelBase
    {
        public ObservableCollection<Pedido> Pedidos { get; set; }
        public ObservableCollection<DetallePedido> DetallePedidos { get; set; }
        public ObservableCollection<Cliente> Clientes { get; set; }
        public ObservableCollection<Empleado> Empleados { get; set; }
        public ObservableCollection<Transportista> Transportistas { get; set; }

        private Pedido _pedidoSeleccionado;
        public Pedido PedidoSeleccionado
        {
            get => _pedidoSeleccionado;
            set => SetProperty(ref _pedidoSeleccionado, value);
        }

        private DateTime _fechaInicio = DateTime.Now.AddDays(-30);
        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set => SetProperty(ref _fechaInicio, value);
        }

        private DateTime _fechaFin = DateTime.Now;
        public DateTime FechaFin
        {
            get => _fechaFin;
            set => SetProperty(ref _fechaFin, value);
        }

        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }
        public RelayCommand LimpiarCommand { get; }
        public RelayCommand GenerarReporteCommand { get; }

        public PedidoViewModel()
        {
            Pedidos = new ObservableCollection<Pedido>();
            DetallePedidos = new ObservableCollection<DetallePedido>();
            Clientes = new ObservableCollection<Cliente>();
            Empleados = new ObservableCollection<Empleado>();
            Transportistas = new ObservableCollection<Transportista>();

            GuardarCommand = new RelayCommand(GuardarPedido);
            EliminarCommand = new RelayCommand(EliminarPedido);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            GenerarReporteCommand = new RelayCommand(GenerarReporte);

            CargarDatos();
        }

        private void CargarDatos()
        {
            CargarPedidos();
            CargarCombos();
        }

        private void CargarPedidos()
        {
            Pedidos.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_Pedidos_Crud",
                new SqlParameter[] { new SqlParameter("@Opcion", "R") });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Pedidos.Add(new Pedido
                {
                    PedidoID = (int)row["PedidoID"],
                    ClienteID = row["ClienteID"] as int?,
                    EmpleadoID = row["EmpleadoID"] as int?,
                    FechaPedido = (DateTime)row["FechaPedido"],
                    FechaRequerida = row["FechaRequerida"] as DateTime?,
                    FechaEnvio = row["FechaEnvio"] as DateTime?,
                    TransportistaID = row["TransportistaID"] as int?,
                    Destinatario = row["Destinatario"].ToString(),
                    CiudadDestino = row["CiudadDestino"].ToString(),
                    PaisDestino = row["PaisDestino"].ToString(),
                    Empresa = row["Empresa"].ToString(),
                    EmpleadoNombre = row["EmpleadoNombre"].ToString(),
                    Transportista = row["Transportista"].ToString()
                });
            }
        }

        private void CargarCombos()
        {
            // Cargar Clientes (Usando ExecuteSqlQuery para consultas directas)
            Clientes.Clear();
            var dtClientes = DatabaseHelper.ExecuteSqlQuery("SELECT * FROM Clientes ORDER BY Empresa");
            foreach (System.Data.DataRow row in dtClientes.Rows)
            {
                Clientes.Add(new Cliente
                {
                    ClienteID = (int)row["ClienteID"],
                    Empresa = row["Empresa"].ToString()
                });
            }

            // Cargar Empleados (Usando ExecuteSqlQuery para consultas directas)
            Empleados.Clear();
            var dtEmpleados = DatabaseHelper.ExecuteSqlQuery("SELECT EmpleadoID, Nombre + ' ' + Apellidos AS NombreCompleto FROM Empleados ORDER BY Nombre");
            foreach (System.Data.DataRow row in dtEmpleados.Rows)
            {
                Empleados.Add(new Empleado
                {
                    EmpleadoID = (int)row["EmpleadoID"],
                    NombreCompleto = row["NombreCompleto"].ToString()
                });
            }

            // Cargar Transportistas (Usando ExecuteSqlQuery para consultas directas)
            Transportistas.Clear();
            var dtTransportistas = DatabaseHelper.ExecuteSqlQuery("SELECT * FROM Transportistas ORDER BY CompaniaNombre");
            foreach (System.Data.DataRow row in dtTransportistas.Rows)
            {
                Transportistas.Add(new Transportista
                {
                    TransportistaID = (int)row["TransportistaID"],
                    CompaniaNombre = row["CompaniaNombre"].ToString()
                });
            }
        }

        private void GuardarPedido(object? parameter)
        {
            try
            {
                if (PedidoSeleccionado == null)
                {
                    MessageBox.Show("No hay un pedido seleccionado", "Advertencia",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string opcion = PedidoSeleccionado.PedidoID == 0 ? "C" : "U";

                DatabaseHelper.ExecuteNonQuery("SP_Pedidos_Crud", new SqlParameter[]
                {
                    new SqlParameter("@Opcion", opcion),
                    new SqlParameter("@PedidoID", PedidoSeleccionado.PedidoID),
                    new SqlParameter("@ClienteID", PedidoSeleccionado.ClienteID ?? (object)DBNull.Value),
                    new SqlParameter("@EmpleadoID", PedidoSeleccionado.EmpleadoID ?? (object)DBNull.Value),
                    new SqlParameter("@FechaPedido", PedidoSeleccionado.FechaPedido),
                    new SqlParameter("@FechaRequerida", PedidoSeleccionado.FechaRequerida ?? (object)DBNull.Value),
                    new SqlParameter("@FechaEnvio", PedidoSeleccionado.FechaEnvio ?? (object)DBNull.Value),
                    new SqlParameter("@TransportistaID", PedidoSeleccionado.TransportistaID ?? (object)DBNull.Value),
                    new SqlParameter("@Destinatario", PedidoSeleccionado.Destinatario ?? (object)DBNull.Value),
                    new SqlParameter("@CiudadDestino", PedidoSeleccionado.CiudadDestino ?? (object)DBNull.Value),
                    new SqlParameter("@PaisDestino", PedidoSeleccionado.PaisDestino ?? (object)DBNull.Value)
                });

                string mensaje = opcion == "C" ? "Pedido creado correctamente" : "Pedido actualizado correctamente";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                CargarPedidos();
                LimpiarFormulario(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarPedido(object? parameter)
        {
            if (PedidoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un pedido para eliminar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Está seguro de eliminar el pedido #{PedidoSeleccionado.PedidoID}?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("SP_Pedidos_Crud", new SqlParameter[]
                    {
                        new SqlParameter("@Opcion", "D"),
                        new SqlParameter("@PedidoID", PedidoSeleccionado.PedidoID)
                    });

                    MessageBox.Show("Pedido eliminado correctamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CargarPedidos();
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
            PedidoSeleccionado = new Pedido
            {
                PedidoID = 0,
                FechaPedido = DateTime.Now
            };
        }

        private void GenerarReporte(object? parameter)
        {
            DetallePedidos.Clear();
            var dt = DatabaseHelper.ExecuteQuery("SP_DetallePedidos_PorFechas",
                new SqlParameter[]
                {
                    new SqlParameter("@FechaInicio", FechaInicio),
                    new SqlParameter("@FechaFin", FechaFin)
                });

            foreach (System.Data.DataRow row in dt.Rows)
            {
                DetallePedidos.Add(new DetallePedido
                {
                    PedidoID = (int)row["PedidoID"],
                    ProductoID = (int)row["ProductoID"],
                    NombreProducto = row["NombreProducto"].ToString(),
                    Cliente = row["Cliente"].ToString(),
                    FechaPedido = (DateTime)row["FechaPedido"],
                    Cantidad = (short)row["Cantidad"],
                    PrecioUnidad = (decimal)row["PrecioUnidad"],
                    Descuento = (decimal)row["Descuento"],
                    Total = (decimal)row["Total"]
                });
            }
        }
    }
}