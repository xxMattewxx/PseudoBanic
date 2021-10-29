using System;

using PseudoBanic.Handlers.Tasks;
using PseudoBanic.Handlers.Accounts;
using PseudoBanic.Handlers.Projects;
using PseudoBanic.Handlers.Versions;
using PseudoBanic.Handlers.Leaderboard;
using PseudoBanic.Data;
using System.Linq;
using System.IO;

namespace PseudoBanic
{
    class MainClass
    {
        public static APIServer api;
        public static void Main(string[] args)
        {
            int port = 1337;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port")
                    port = Convert.ToInt32(args[i + 1]);
            }

            Global.Load();

            using var dbContext = new HistoricalLeaderboardDbContext();
            using var fileWriter = new StreamWriter("dump.txt");
            using var debugWriter = new StreamWriter("debug.txt");

            DateTime start = DateTime.Now;
            foreach (var aux in dbContext.HistoricalLeaderboard
                .Where(x => x.MetadataID == 3))
            {
                fileWriter.WriteLine("{0} {1} {2} {3} {4}", aux.UserID, aux.Points, aux.ValidatedPoints, aux.InvalidatedPoints, Utils.ConvertToUnixTimestamp(aux.SnapshotTime));
            }
            fileWriter.Flush();
            debugWriter.WriteLine("Done in {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds);
            debugWriter.Flush();

            api = new APIServer(port);

            api.AddAction("/accounts/register", Register.ProcessContext);

            api.AddAction("/accounts/setlevel/byuserid", ChangeUserLevelByUserID.ProcessContext);
            api.AddAction("/accounts/setlevel/byusername", ChangeUserLevelByUsername.ProcessContext);
            api.AddAction("/accounts/setlevel/bydiscordid", ChangeUserLevelByDiscordID.ProcessContext);

            api.AddAction("/accounts/vaporise/user/byuserid", DeleteUserByUserID.ProcessContext);
            api.AddAction("/accounts/vaporise/user/byusername", DeleteUserByUsername.ProcessContext);
            api.AddAction("/accounts/vaporise/user/bydiscordid", DeleteUserByDiscordID.ProcessContext);

            api.AddAction("/accounts/query/basic/me", QueryBasicMe.ProcessContext);
            api.AddAction("/accounts/query/basic/byuserid", QueryBasicByUserID.ProcessContext);
            api.AddAction("/accounts/query/basic/byusername", QueryBasicByUsername.ProcessContext);
            api.AddAction("/accounts/query/basic/bydiscordid", QueryBasicByDiscordID.ProcessContext);

            api.AddAction("/accounts/regen/byuserid", RegenByUserID.ProcessContext);
            api.AddAction("/accounts/regen/byusername", RegenByUsername.ProcessContext);
            api.AddAction("/accounts/regen/bydiscordid", RegenByDiscordID.ProcessContext);

            api.AddAction("/versions/add", AddVersion.ProcessContext);
            api.AddAction("/versions/latest", GetLatestVersion.ProcessContext);

            api.AddAction("/tasks/submit", Submit.ProcessContext);
            api.AddAction("/tasks/addtask", AddTask.ProcessContext);
            api.AddAction("/tasks/retrieve", Retrieve.ProcessContext);
            api.AddAction("/tasks/addmeta", AddTaskMetadata.ProcessContext);

            api.AddAction("/tasks/query/metadata/bymetaid", QueryMetadata.ProcessContext);
            api.AddAction("/tasks/query/outputs/byappid", StreamOutputsByAppID.ProcessContext);
            api.AddAction("/tasks/query/progress/bymetaid", QueryTotalProgressByMetaID.ProcessContext);

            api.AddAction("/projects/list", ListProjects.ProcessContext);

            api.AddAction("/leaderboard/current", GetCurrentLeaderboard.ProcessContext);
            api.AddAction("/leaderboard/historical/byprojectid", Handlers.Leaderboard.Historical.ByProjectID.ProcessContext);


            api.Listen();
        }
    }
}