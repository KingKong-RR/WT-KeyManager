using System.Windows;
using log4net;

namespace KeyManager.Views
{
    /// <summary>
    /// Interaction logic for EditGroupDialogView.xaml
    /// </summary>
    public partial class EditGroupDialogView : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bootstrap.Bootstrap));

        public EditGroupDialogView()
        {
            InitializeComponent();
        }
    }
}
