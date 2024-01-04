using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for MessageBoxInfoDialog.xaml
    /// </summary>
    public partial class MessageBoxInfoDialog : Window
    {
        public MessageBoxInfoDialog(string message)
        {
            InitializeComponent();
            DataContext = new MessageBoxInformation { Message = message };
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
