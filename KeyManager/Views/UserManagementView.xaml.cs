using System.Windows.Controls;
using log4net;

namespace KeyManager.Views
{
    /// <summary>
    /// Interaction logic for UserManagementView.xaml
    /// </summary>
    public partial class UserManagementView : UserControl
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bootstrap.Bootstrap));

        public UserManagementView()
        {
            InitializeComponent();
        }
    }
}
