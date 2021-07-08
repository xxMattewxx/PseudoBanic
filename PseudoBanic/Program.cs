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
			api.AddAction("/querymeta/byid", Handlers.QueryMetadata.ProcessContext);
			api.AddAction("/vaporise/user/bynickname", Handlers.DeleteUserByNickname.ProcessContext);
			api.AddAction("/vaporise/user/bydiscordid", Handlers.DeleteUserByDiscordID.ProcessContext);

			api.Listen ();
		}
	}
}