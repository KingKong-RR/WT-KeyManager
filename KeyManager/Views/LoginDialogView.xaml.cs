using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyManager.DataBase;
using log4net;
using KeyManager.ViewModels;

namespace KeyManager.Views
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>

    public partial class LoginDialogView
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bootstrap.Bootstrap));

        public LoginDialogView()
        {
            InitializeComponent();
        }

    }
}
