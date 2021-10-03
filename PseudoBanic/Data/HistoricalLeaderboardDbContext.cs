using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PseudoBanic.Data
{
    class HistoricalLeaderboardDbContext : DbContext
    {
        public DbSet<HistoricalLeaderboardPoint> HistoricalLeaderboard { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder = Global.DBDriver switch
            {
                "PSQL" => optionsBuilder.UseNpgsql(Global.DBConnectionStr.ConnectionString),
                _ => throw new Exception("Unsupported database driver: " + Global.DBDriver)
            };

            base.OnConfiguring(optionsBuilder);
        }
    }
}
