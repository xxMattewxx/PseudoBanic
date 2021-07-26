using MySqlConnector;
using Newtonsoft.Json;
using System.IO;

namespace PseudoBanic
{
    class Global
    {
        public static MySqlConnectionStringBuilder builder;
        public static string KafkaServers;

        public static void Load()
        {
            builder = JsonConvert.DeserializeObject<MySqlConnectionStringBuilder>(
                File.ReadAllText("sqlconnection.cfg")
            );

            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("config.cfg"));
            KafkaServers = (string)config.KafkaServers;
        }
    }
}
