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
        const int CACHE_UPDATE_RATE = 1000;

        static async void UpdateData()
        {
            try
            {
                var cachedDB = Global.RedisMultiplexer.GetDatabase();
                using var dbContext = new HistoricalLeaderboardDbContext();

                StringBuilder builder = new StringBuilder();
                var results = dbContext.HistoricalLeaderboard
                    .Where(x => x.ID > LastID)
                    .OrderBy(z => z.ID)
                    .Take(1000)
                    .ToList();

                for (int i = 0; i < Math.Min(1000, results.Count); i++)
                {
                    var aux = results[i];
                    LastID = aux.ID;

                    await cachedDB.ListRightPushAsync(
                        "historical-leaderboard-projectid-" + aux.MetadataID,
                        string.Format("{0} {1} {2} {3} {4}", aux.UserID, aux.Points, aux.ValidatedPoints, aux.InvalidatedPoints, Utils.ConvertToUnixTimestamp(aux.SnapshotTime))
                    );
                }
            }
            catch { }
        }

        public static async Task<string> GetData(long projectID)
        {
            try
            {
                var cachedDB = Global.RedisMultiplexer.GetDatabase();
                var value = await cachedDB.ListRangeAsync("historical-leaderboard-projectid-" + projectID);
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
