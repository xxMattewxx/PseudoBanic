using MySqlConnector;
using Npgsql;
using System;
using System.Data.Common;
using StackExchange.Redis;

namespace PseudoBanic
{
    class Global
    {
        public static DbConnectionStringBuilder DBConnectionStr;
        public static ConnectionMultiplexer RedisMultiplexer;
        public static String DBDriver;

        public static void Load()
        {
            DotEnv.Load(".env");

            DBDriver = Environment.GetEnvironmentVariable("DB_DRIVER");
            DBConnectionStr = DBDriver switch
            {
                "PSQL" => new NpgsqlConnectionStringBuilder()
                {
                    Host = Environment.GetEnvironmentVariable("DB_HOST"),
                    Username = Environment.GetEnvironmentVariable("DB_USER"),
                    Password = Environment.GetEnvironmentVariable("DB_PASS"),
                    Database = Environment.GetEnvironmentVariable("DB_NAME")
                },
                _ => throw new Exception("Invalid database driver: " + DBDriver)
            };

            ConfigurationOptions redisOptions = new ConfigurationOptions()
            {
                Password = Environment.GetEnvironmentVariable("REDIS_PASS")
            };

            string redisServers = Environment.GetEnvironmentVariable("REDIS_SERVERS");
            string[] redisEndpoints = redisServers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            foreach(var endpoint in redisEndpoints)
            {
                redisOptions.EndPoints.Add(endpoint);
            }

            RedisMultiplexer = ConnectionMultiplexer.Connect(redisOptions);
        }
    }
}
