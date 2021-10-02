using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace PseudoBanic.Data
{
    public class TaskInfo
    {
        [Key]
        public int ID { get; set; }
        public int MetadataID { get; set; }
        public TaskMeta Metadata { get; set; }
        public int Status { get; set; }
        public int QuorumLeft { get; set; }
        public string InputData { get; set; }
        public string Consensus { get; set; }

        public bool IsValid()
        {
            if (MetadataID <= 0) return false;

            return true;
        }

        public static bool AddToDatabase(TaskInfo toAdd)
        {
            using var dbContext = new TasksDbContext();
            dbContext.Tasks.Add(toAdd);

            return dbContext.SaveChanges() > 0;
        }

        public static TaskInfo GetByID(int taskID)
        {
            using var dbContext = new TasksDbContext();
            var task = dbContext.Tasks.Where(task => task.ID == taskID)
                .Include(task => task.Metadata)
                .SingleOrDefault();

            return task;
        }

        public static int RetrieveAtomicID(UserInfo user)
        {
            using var dbContext = new TasksDbContext();
            using var connection = dbContext.Database.GetDbConnection();
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM RetrieveAtomic(@userid);";
                command.CommandType = CommandType.Text;

                DbParameter userIDParam = command.CreateParameter();
                userIDParam.ParameterName = "@userid";
                userIDParam.Value = user.ID;
                userIDParam.DbType = DbType.Int32;
                command.Parameters.Add(userIDParam);

                using DbDataReader reader = command.ExecuteReader();
                reader.Read();

                if (!reader.HasRows)
                {
                    return -2;
                }

                return reader.GetInt32(0);
            }
        }

        public static TaskInfo RetrieveAtomic(UserInfo user)
        {
            int taskID = RetrieveAtomicID(user);
            if (taskID <= 0)
                return null;

            return GetByID(taskID);
        }
    }
}