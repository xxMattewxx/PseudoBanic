using System;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

namespace PseudoBanic.Data
{
    public class ClientVersion
    {
        [Key]
        public int VersionNumber { get; set; }
        public string Codename { get; set; }
        public string BinaryURL { get; set; }
        public string FileHash { get; set; }

        public bool IsValid()
        {
            if (Codename == null || Codename.Length < 1) return false;
            if (BinaryURL == null || !Uri.IsWellFormedUriString(BinaryURL, UriKind.Absolute)) return false;
            if (FileHash == null || FileHash.Length != 32) return false;

            return true;
        }

        public string ToJSON()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateCache()
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var jsonStr = ToJSON();

            cachedDB.StringSet("versions-latest", jsonStr, TimeSpan.FromHours(3));
        }

        public static ClientVersion GetLatest()
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var key = "versions-latest";
            var cachedValue = cachedDB.StringGet(key);
            if (cachedValue != RedisValue.Null)
                return FromJSON(cachedValue);

            using var dbContext = new VersionDbContext();
            var version = dbContext.Versions.OrderBy(x => x.VersionNumber).Last();

            if (version != null)
                version.UpdateCache();
            return version;
        }

        public static long AddToDatabase(ClientVersion toAdd)
        {
            using var dbContext = new VersionDbContext();
            dbContext.Versions.Add(toAdd);

            if (dbContext.SaveChanges() > 0)
            {
                toAdd.UpdateCache();
                return toAdd.VersionNumber;
            }
            return -1;
        }

        public static ClientVersion FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<ClientVersion>(json);
        }
    }
}
