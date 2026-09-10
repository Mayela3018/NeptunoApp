using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeptunoApp.Views
{
    public partial class VistaProveedores : UserControl
    {
        public VistaProveedores()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        
        private void PhoneValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9-]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}