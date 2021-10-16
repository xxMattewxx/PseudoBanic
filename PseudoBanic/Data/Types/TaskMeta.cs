using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PseudoBanic.Data
{
    [Table("TasksMetadata")]
    public class TaskMeta
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BinaryURL { get; set; }
        public string FileHash { get; set; }
        public bool PassByFile { get; set; }

        public bool IsValid()
        {
            if (Name == null || Name.Length < 5) return false;
            if (FileHash == null || FileHash.Length != 32) return false;
            if (BinaryURL == null || !Uri.IsWellFormedUriString(BinaryURL, UriKind.Absolute)) return false;
            if (Description == null || Description.Length < 5) return false;

            return true;
        }

        public static long AddToDatabase(TaskMeta toAdd)
        {
            using var dbContext = new TasksMetadataDbContext();

            dbContext.TasksMetadata.Add(toAdd);

            if (dbContext.SaveChanges() > 0)
            {
                return toAdd.ID;
            }
            return -1;
        }

        public static TaskMeta GetByID(int taskMetaID)
        {
            using var dbContext = new TasksMetadataDbContext();
            var task = dbContext.TasksMetadata.Where(taskMeta => taskMeta.ID == taskMetaID)
                .SingleOrDefault();

            return task;
        }

        public static List<TaskMeta> GetAll()
        {
            using var dbContext = new TasksMetadataDbContext();

            return dbContext.TasksMetadata.ToList();
        }
    }
}
