using PseudoBanic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoBanic.Workers
{
    class HistoricalLeaderboardHelper
    {
        const int CACHE_UPDATE_RATE = 1000 * 30;
        static int cacheVersion = 0;
        static string GenerateData(long projectID)
        {
            using var dbContext = new HistoricalLeaderboardDbContext();
            StringBuilder builder = new StringBuilder();
            foreach (var aux in dbContext.HistoricalLeaderboard
                .Where(x => x.MetadataID == projectID)
                .OrderBy(z => z.ID))
            {
                builder.Append(string.Format("{0} {1} {2} {3} {4}\n", aux.UserID, aux.Points, aux.ValidatedPoints, aux.InvalidatedPoints, Utils.ConvertToUnixTimestamp(aux.SnapshotTime)));
            }

            return builder.ToString();
        }

        static void UpdateCache(long projectID, string newValue)
        {
            try
            {
                var cachedDB = Global.RedisMultiplexer.GetDatabase();

                cachedDB.StringSet("historical-leaderboard-projectid-" + projectID + "-" + (cacheVersion + 1), newValue, TimeSpan.FromMinutes(3));
            }
            catch { }
        }

        public static string GetData(long projectID)
        {
            try
            {
                var cachedDB = Global.RedisMultiplexer.GetDatabase();
                var value = cachedDB.StringGet("historical-leaderboard-projectid-" + projectID + cacheVersion);
                if (!value.HasValue)
                    return null;

                return value.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static void RunWorker()
        {
            Thread thread = new Thread(() =>
            {
                Console.WriteLine("[WORKER] Historical Leaderboard cache updater started.");
                while (true)
                {
                    List<TaskMeta> projects = TaskMeta.GetAll();
                    foreach(var project in projects)
                    {
                        UpdateCache(project.ID, GenerateData(project.ID));
                    }
                    cacheVersion++;
                    Thread.Sleep(CACHE_UPDATE_RATE);
                }
            });
            thread.Start();
        }
    }
}
