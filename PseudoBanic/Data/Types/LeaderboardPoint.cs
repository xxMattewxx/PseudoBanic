using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using PseudoBanic.Data.Dtos;

namespace PseudoBanic.Data
{
    [Table("Leaderboard")]
    class LeaderboardPoint
    {
        [Key, JsonIgnore]
        public long ID { get; set; }
        public UsernameReadDto User { get; set; }
        [JsonIgnore]
        public long MetadataID { get; set; }
        public int Points { get; set; }
        public int ValidatedPoints { get; set; }
        public int InvalidatedPoints { get; set; }

        public static List<LeaderboardPoint> GetAllData()
        {
            using var dbContext = new LeaderboardDbContext();
            return dbContext.Leaderboard.ToList();
        }

        public static List<LeaderboardPoint> GetDataForProject(long id)
        {
            using var dbContext = new LeaderboardDbContext();
            return dbContext.Leaderboard
                .Where(x => x.MetadataID == id)
                .Include(point => point.User)
                .OrderBy(z => z.ValidatedPoints)
                .ToList();
        }
    }
}
