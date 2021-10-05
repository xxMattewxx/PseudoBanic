using System;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PseudoBanic.Data
{
    [Table("Users")]
    public class UserInfo
    {
        [Key, JsonIgnore]
        public int ID { get; set; }
        public string Username { get; set; }
        public string APIKey { get; set; }
        public int AdminLevel { get; set; }
        public Int64 DiscordID { get; set; }

        public bool IsValidForRegister()
        {
            if (Username == null || !Utils.IsValidUsername(Username)) return false;
            if (DiscordID == 0) return false;

            return true;
        }

        public bool SetAdminLevel(int adminLevel)
        {
            using var context = new UserDbContext();
            var user = context.Users.FirstOrDefault(user => user.ID == ID);
            user.AdminLevel = adminLevel;

            if (context.SaveChanges() > 0)
            {
                user.UpdateCache();
                return true;
            }
            return false;
        }

        public bool SetAPIKey(string APIKey)
        {
            using var context = new UserDbContext();
            var user = context.Users.FirstOrDefault(user => user.ID == ID);
            user.APIKey = Utils.SHA256String(APIKey);

            if (context.SaveChanges() > 0)
            {
                user.UpdateCache();
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            using var context = new UserDbContext();
            var user = context.Users.FirstOrDefault(user => user.ID == ID);
            context.Remove(user);

            if(context.SaveChanges() > 0)
            {
                RemoveFromCache();
                return true;
            }
            return false;
        }

        public string ToJSON()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void UpdateCache()
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var jsonStr = ToJSON();

            cachedDB.StringSet("users-apikey-" + APIKey, jsonStr, TimeSpan.FromHours(3));
            cachedDB.StringSet("users-userid-" + ID, jsonStr, TimeSpan.FromMinutes(10));
            cachedDB.StringSet("users-username-" + Username, jsonStr, TimeSpan.FromMinutes(10));
            cachedDB.StringSet("users-discordid-" + DiscordID, jsonStr, TimeSpan.FromMinutes(10));
        }

        public void RemoveFromCache()
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            cachedDB.KeyDelete(new RedisKey[] {
                "users-apikey-" + APIKey,
                "users-userid-" + ID,
                "users-username-" + Username,
                "users-discordid-" + DiscordID
            });
        }

        public static UserInfo FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        public static bool AddToDatabase(UserInfo toAdd)
        {
            using var dbContext = new UserDbContext();
            var user = new UserInfo()
            {
                Username = toAdd.Username,
                APIKey = Utils.SHA256String(toAdd.APIKey),
                AdminLevel = 0,
                DiscordID = toAdd.DiscordID
            };

            dbContext.Users.Add(user);

            if (dbContext.SaveChanges() > 0)
            {
                user.UpdateCache();
                return true;
            }
            return false;
        }

        public static UserInfo GetByUserID(int userID)
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var key = "users-userid-" + userID;
            var cachedValue = cachedDB.StringGet(key);
            if (cachedValue != RedisValue.Null)
                return FromJSON(cachedValue);

            using var dbContext = new UserDbContext();
            var user = dbContext.Users.Where(user => user.ID == userID).FirstOrDefault();

            if (user != null)
                user.UpdateCache();
            return user;
        }

        public static UserInfo GetByDiscordID(Int64 DiscordID)
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var key = "users-discordid-" + DiscordID;
            var cachedValue = cachedDB.StringGet(key);
            if(cachedValue != RedisValue.Null)
                return FromJSON(cachedValue);

            using var dbContext = new UserDbContext();
            var user = dbContext.Users.Where(user => user.DiscordID == DiscordID).FirstOrDefault();
            
            if (user != null)
                user.UpdateCache();
            return user;
        }

        public static UserInfo GetByAPIKey(string APIKey)
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var hashedAPIKey = Utils.SHA256String(APIKey);
            var key = "users-apikey-" + hashedAPIKey;
            var cachedValue = cachedDB.StringGet(key);
            if (cachedValue != RedisValue.Null)
                return FromJSON(cachedValue);

            using var dbContext = new UserDbContext();
            var user = dbContext.Users.Where(user => user.APIKey == hashedAPIKey).FirstOrDefault();

            if(user != null)
                user.UpdateCache();
            return user;
        }

        public static UserInfo GetByUsername(string Username)
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            var key = "users-username-" + Username;
            var cachedValue = cachedDB.StringGet(key);
            if (cachedValue != RedisValue.Null)
                return FromJSON(cachedValue);

            using var dbContext = new UserDbContext();
            var user = dbContext.Users.Where(user => user.Username == Username).FirstOrDefault();

            if (user != null)
                user.UpdateCache();
            return user;
        }
    }
}
