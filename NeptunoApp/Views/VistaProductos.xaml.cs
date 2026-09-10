using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeptunoApp.Views
{
    public partial class VistaProductos : UserControl
    {
        public VistaProductos()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");

            e.Handled = regex.IsMatch(e.Text);
        }
    }
}