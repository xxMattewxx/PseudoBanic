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
        static long LastID = 0;
        const int CACHE_UPDATE_RATE = 1000 * 30;

        static string UpdateData()
        {
            var cachedDB = Global.RedisMultiplexer.GetDatabase();
            using var dbContext = new HistoricalLeaderboardDbContext();

            StringBuilder builder = new StringBuilder();
            var results = dbContext.HistoricalLeaderboard
                .Where(x => x.ID > LastID)
                .OrderBy(z => z.ID);

            foreach (var aux in results)
            {
                LastID = aux.ID;

                cachedDB.ListRightPush(
                    "historical-leaderboard-projectid-" + aux.MetadataID, 
                    string.Format("{0} {1} {2} {3} {4}\n", aux.UserID, aux.Points, aux.ValidatedPoints, aux.InvalidatedPoints, Utils.ConvertToUnixTimestamp(aux.SnapshotTime))
                );
            }

            return builder.ToString();
        }

        public static string GetData(long projectID)
        {
            try
            {
                var cachedDB = Global.RedisMultiplexer.GetDatabase();
                var value = cachedDB.ListRangeAsync("historical-leaderboard-projectid-" + projectID).Result;
                if (value == null)
                    return null;

                return string.Join('\n', value);
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
                var cachedDB = Global.RedisMultiplexer.GetDatabase();
                Console.WriteLine("[WORKER] Historical Leaderboard cache updater started.");
                while (true)
                {
                    List<TaskMeta> projects = TaskMeta.GetAll();
                    if (LastID == 0)
                        foreach (var project in projects)
                            cachedDB.KeyDelete("historical-leaderboard-projectid-" + project.ID);

                    UpdateData();
                    Thread.Sleep(CACHE_UPDATE_RATE);
                }
            });
            thread.Start();
        }
    }
}
