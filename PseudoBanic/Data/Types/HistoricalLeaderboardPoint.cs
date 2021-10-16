using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

        public static List<HistoricalLeaderboardPoint> GetDataForTime(long metadataID, DateTime time)
        {
            using var dbContext = new HistoricalLeaderboardDbContext();
            dbContext.HistoricalLeaderboard.FromSqlRaw("");
            return null;
            /*return dbContext.HistoricalLeaderboard
                .Where(x => x.MetadataID == metadataID && x.SnapshotTime < time)
                .OrderByDescending(x => x.SnapshotTime)
                .take
                .ToList();*/
        }
    }
}
