using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PseudoBanic.Data
{
    [Table("HistoricalLeaderboard")]
    class HistoricalLeaderboardPoint
    {
        [Key]
        public long ID { get; set; }
        public int UserID { get; set; }
        public UserInfo User { get; set; }
        public long MetadataID { get; set; }
        public int Points { get; set; }
        public int ValidatedPoints { get; set; }
        public int InvalidatedPoints { get; set; }
        public DateTime SnapshotTime { get; set; }

        public static List<HistoricalLeaderboardPoint> GetDataForTime(DateTime time)
        {
            return null;
            //return dbContext.HistoricalLeaderboard.ToList();
        }
    }
}
