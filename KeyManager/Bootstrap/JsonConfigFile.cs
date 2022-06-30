using System;
using System.IO;
using log4net;
using Newtonsoft.Json;

namespace KeyManager.Bootstrap
{
    class JsonConfigFile
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonConfigFile));

        public static JsonConfigFile Instance => new JsonConfigFile();

        public bool Load()
        {
            var programPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
            var jsonConfigPath = new Uri(programPath + "Resources/KeyManagerConfig.json").LocalPath;

            if (Load(jsonConfigPath))
                return true;

            jsonConfigPath = new Uri(programPath + "KeyManagerConfig.json").LocalPath;
            return Load(jsonConfigPath, true);
        }

        public bool Load(string jsonConfigPath, bool showException = false)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Reuse
                };

                // Console.WriteLine($"Config file path: {jsonConfigPath}");

                JsonConvert.DeserializeObject<Config>(
                    File.ReadAllText(jsonConfigPath),
                    serializerSettings
                );

                Log.Debug($"Json config file loaded.");
            }
            catch (Exception e)
            {
                if (showException)
                {
                    Log.Error($"Json config file could not be loaded! {e}");
                    System.Windows.MessageBox.Show($"Configuration file could not be loaded! Filepath:  {jsonConfigPath} \n\n {e}");
                }

                return false;
            }

            return true;
        }
    }
}