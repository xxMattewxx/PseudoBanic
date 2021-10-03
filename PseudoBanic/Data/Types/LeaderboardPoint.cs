using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data.Types
{
    class LeaderboardPoint
    {
        [Key]
        public long ID { get; set; }
        public long MetadataID { get; set; }
        public int Points { get; set; }
        public int ValidatedPoints { get; set; }
        public int InvalidatedPoints { get; set; }
        public DateTime SnapshotTime { get; set; }

        public static List<LeaderboardPoint> GetAllHistoricalData()
        {
            using var dbContext = new LeaderboardDbContext();
            return dbContext.HistoricalLeaderboard.ToList();
        }
    }
}
