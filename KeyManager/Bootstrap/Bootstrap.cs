using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using log4net;

namespace KeyManager.Bootstrap
{
    public class Bootstrap
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bootstrap));
        private static bool _appRunning;

        public static void StartUp()
        {
            // set CultureInfo to english, otherwise exceptions are in german
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            // check if there is already a viewer running, if so, show message and quit
            string fileNoEnding = Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
            Process[] processes = Process.GetProcessesByName(fileNoEnding);

            if (processes.Length > 1 && !fileNoEnding.Contains("vshost")
            ) //vshost - if we are debugging, there is always a process XXX.vshost.exe running...
            {
                string msg = "There is already an instance running, please close other instance first!";
                Log.Fatal(msg);
                System.Windows.MessageBox.Show(msg, "KeyManager is already running...");
                Application.Current.Shutdown();
                return;
            }

            Log.Info("KeyManager is starting up...");
            _appRunning = true;
            if (!JsonConfigFile.Instance.Load())
            {
                Log.Fatal("Could not load configuration file!");
                Application.Current.Shutdown();
                return;
            }
        }
    }
}
