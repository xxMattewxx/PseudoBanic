using System;

using PseudoBanic.Handlers.Tasks;
using PseudoBanic.Handlers.Accounts;
using PseudoBanic.Handlers.Versions;
using PseudoBanic.Handlers.Leaderboard;
using PseudoBanic.Data;
using System.Linq;

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

            api = new APIServer(port);

            api.AddAction("/accounts/register", Register.ProcessContext);

            api.AddAction("/accounts/setlevel/byuserid", ChangeUserLevelByUserID.ProcessContext);
            api.AddAction("/accounts/setlevel/byusername", ChangeUserLevelByUsername.ProcessContext);
            api.AddAction("/accounts/setlevel/bydiscordid", ChangeUserLevelByDiscordID.ProcessContext);

            api.AddAction("/accounts/vaporise/user/byuserid", DeleteUserByUserID.ProcessContext);
            api.AddAction("/accounts/vaporise/user/byusername", DeleteUserByUsername.ProcessContext);
            api.AddAction("/accounts/vaporise/user/bydiscordid", DeleteUserByDiscordID.ProcessContext);

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

            api.AddAction("/leaderboard/historical/query/all", GetAllData.ProcessContext);
            /*api.AddAction("/tasks/query/progress/bymetaid", QueryTotalProgressByMetaID.ProcessContext);
            api.AddAction("/tasks/query/outputs/bytaskid", QueryOutput.ProcessContext);*/
            api.AddAction("/tasks/query/outputs/byappid", StreamOutputsByAppID.ProcessContext);

            using var dbContext = new HistoricalLeaderboardDbContext();
            var lol = dbContext.HistoricalLeaderboard.GroupBy(p => p.UserID);

            foreach (var aux in lol.ToList())
                Console.WriteLine(aux);

            api.Listen();
        }
    }
}