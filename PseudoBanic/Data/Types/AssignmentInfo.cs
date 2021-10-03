using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PseudoBanic.Data
{
    class AssignmentInfo
    {
        public int AssignmentID { get; set; }
        public int TaskID { get; set; }
        public TaskInfo Task { get; set; }
        public int UserID { get; set; }
        public UserInfo User { get; set; }
        public DateTime Deadline { get; set; }
        public int State { get; set; }
        public string Output { get; set; }

        public bool UpdateOutput(string output)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Global.DBConnectionStr.ConnectionString))
                {
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM UpdateAtomic(@assignmentID, @output);";
                    command.Parameters.AddWithValue("@assignmentID", AssignmentID);
                    command.Parameters.AddWithValue("@output", output);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static AssignmentInfo GetFromTaskInfo(int userID, int taskID)
        {
            using var dbContext = new AssignmentsDbContext();
            var assignment = dbContext.Assignments.Where(x => x.UserID == userID && x.TaskID == taskID).FirstOrDefault();
            return assignment;
        }
    }
}
