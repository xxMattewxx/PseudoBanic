﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PseudoBanic.Data
{
    class TasksMetadataDbContext : DbContext
    {
        public DbSet<TaskMeta> TasksMetadata { get; set; }

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