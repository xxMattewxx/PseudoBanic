using System;

using PseudoBanic.Handlers.Accounts;
using PseudoBanic.Handlers.Tasks;
using PseudoBanic.Handlers.Versions;

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
            api.AddAction("/accounts/setlevel", ChangeUserLevel.ProcessContext);
            api.AddAction("/accounts/regen/byname", RegenByUsername.ProcessContext);
            api.AddAction("/accounts/regen/bydiscordid", RegenByDiscordID.ProcessContext);
            api.AddAction("/accounts/query/basic/bydiscordid", QueryBasicByDiscordID.ProcessContext);
            api.AddAction("/accounts/vaporise/user/bynickname", DeleteUserByNickname.ProcessContext);
            api.AddAction("/accounts/vaporise/user/bydiscordid", DeleteUserByDiscordID.ProcessContext);

            api.AddAction("/tasks/submit", Submit.ProcessContext);
            api.AddAction("/tasks/addmeta", AddTaskMetadata.ProcessContext);
            api.AddAction("/tasks/addtask", AddTask.ProcessContext);
            api.AddAction("/tasks/retrieve", Retrieve.ProcessContext);
            api.AddAction("/tasks/query/metadata/bymetaid", QueryMetadata.ProcessContext);
            api.AddAction("/tasks/query/progress/bymetaid", QueryTotalProgressByMetaID.ProcessContext);
            api.AddAction("/tasks/query/outputs/bytaskid", QueryOutput.ProcessContext);
            api.AddAction("/tasks/query/outputs/byappid", StreamOutputsByAppID.ProcessContext);

            api.AddAction("/versions/add", AddVersion.ProcessContext);
            api.AddAction("/versions/latest", GetLatestVersion.ProcessContext);

            api.Listen();
        }
    }
}