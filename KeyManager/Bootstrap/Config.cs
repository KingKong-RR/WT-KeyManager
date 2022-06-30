using Newtonsoft.Json;

namespace KeyManager.Bootstrap
{
    public class Config
    {
        [JsonProperty("DataSource", Required = Required.Always)]
        public static string DataSource { get; set; }

        [JsonProperty("InitialCatalogue", Required = Required.Always)]
        public static string InitialCatalogue { get; set; }

        [JsonProperty("User", Required = Required.Always)]
        public static string User { get; set; }

        [JsonProperty("Password", Required = Required.Always)]
        public static string Password { get; set; }
    }
}