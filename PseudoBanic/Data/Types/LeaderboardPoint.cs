﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PseudoBanic.Data
{
    [Table("Leaderboard")]
    class LeaderboardPoint
    {
        [Key]
        public long ID { get; set; }
        public long MetadataID { get; set; }
        public int Points { get; set; }
        public int ValidatedPoints { get; set; }
        public int InvalidatedPoints { get; set; }
        public DateTime SnapshotTime { get; set; }

        public static List<LeaderboardPoint> GetAllData()
        {
            using var dbContext = new LeaderboardDbContext();
            return dbContext.Leaderboard.ToList();
        }
    }
}
