using PseudoBanic.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoBanic.Workers
{
    class HistoricalLeaderboardHelper
    {
        static long LastID = 0;
        const int CACHE_UPDATE_COUNT = 2000;
        const int CACHE_UPDATE_RATE = 1000 * 30;
        static Dictionary<long, List<string>> cache = new Dictionary<long, List<string>>();
        static object lockObj = new object();

        static void Append(long projectID, string text)
        {
            if (!cache.ContainsKey(projectID))
                cache.Add(projectID, new List<string>());

            cache[projectID].Add(text);
        }

        static void UpdateData()
        {

            try
            {
                using var dbContext = new HistoricalLeaderboardDbContext();

                var results = dbContext.HistoricalLeaderboard
                    .Where(x => x.ID > LastID)
                    .OrderBy(z => z.ID)
                    .Take(CACHE_UPDATE_COUNT)
                    .ToList();

                lock (lockObj)
                {
                    for (int i = 0; i < Math.Min(CACHE_UPDATE_COUNT, results.Count); i++)
                    {
                        var aux = results[i];
                        LastID = aux.ID;

                        Append(aux.MetadataID, string.Format("{0} {1} {2} {3} {4}", aux.UserID, aux.Points, aux.ValidatedPoints, aux.InvalidatedPoints, Utils.ConvertToUnixTimestamp(aux.SnapshotTime)));
                    }
                }
            }
            catch { }
        }

        public static string GetData(long projectID)
        {
            try
            {
                lock (lockObj)
                {
                    if (!cache.ContainsKey(projectID))
                        return null;

                    return string.Join('\n', cache[projectID]);
                }
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
                    UpdateData();
                    Thread.Sleep(CACHE_UPDATE_RATE);
                }
            });
            thread.Start();
        }
    }
}
