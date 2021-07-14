using System;
using System.Net;

namespace PseudoBanic
{
	class MainClass
	{
		public static APIServer api;
		public static void Main (string[] args)
		{
			int port = 189;

			for(int i = 0; i < args.Length; i++) {
				if (args[i] == "--port")
					port = Convert.ToInt32(args[i + 1]);
            }

			Global.Load();

			api = new APIServer(port);

			api.AddAction("/submit", Handlers.Submit.ProcessContext);
			api.AddAction("/addmeta", Handlers.AddTaskMetadata.ProcessContext);
			api.AddAction("/addtask", Handlers.AddTask.ProcessContext);
			api.AddAction("/register", Handlers.Register.ProcessContext);
			api.AddAction("/retrieve", Handlers.Retrieve.ProcessContext);
			api.AddAction("/setlevel", Handlers.ChangeUserLevel.ProcessContext);
			api.AddAction("/query/meta/byid", Handlers.QueryMetadata.ProcessContext);
			api.AddAction("/query/outputs/bytaskid", Handlers.QueryOutput.ProcessContext);
			api.AddAction("/stream/outputs/byappid", Handlers.StreamOutputsByAppID.ProcessContext);
			api.AddAction("/query/basic/bydiscordid", Handlers.QueryBasicByDiscordID.ProcessContext);
			api.AddAction("/vaporise/user/bynickname", Handlers.DeleteUserByNickname.ProcessContext);
			api.AddAction("/vaporise/user/bydiscordid", Handlers.DeleteUserByDiscordID.ProcessContext);
			api.AddAction("/progress", Handlers.QueryTotalProgressByMetaID.ProcessContext);
			api.Listen ();
		}
	}
}