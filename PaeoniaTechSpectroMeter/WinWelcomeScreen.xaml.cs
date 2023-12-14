
//using System.Windows;
using System.Windows;
using System.Windows.Controls;


namespace PaeoniaTechSpectroMeter
{
    /// <summary>
    /// Interaction logic for WinWelcomeScreen.xaml
    /// </summary>
    public partial class WinWelcomeScreen : Window
    {
        public WinWelcomeScreen()
        {
            InitializeComponent();
        }

        bool closing = false;
        public WinWelcomeScreen(bool closing = false)
        {
            
           // Dias_Lisa.Lisa.Action();
            InitializeComponent();
            this.closing = closing;
        }

        private void ListMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
