using System.Windows;
using log4net;
using KeyManager.Utilities;

namespace KeyManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bootstrap.Bootstrap));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OpenLogin.OpenLoginDialog();
        }
    }
}
