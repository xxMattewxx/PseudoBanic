using System;
using System.Net;

namespace PseudoBanic
{
	class MainClass
	{
		public static APIServer api = new APIServer();
		public static void Main (string[] args)
		{
			Global.Load();

			api.AddAction("/addtask", Handlers.AddTask.ProcessContext);
			api.AddAction("/register", Handlers.Register.ProcessContext);
			api.AddAction("/retrieve", Handlers.Retrieve.ProcessContext);
			api.AddAction("/setlevel", Handlers.ChangeUserLevel.ProcessContext);
			api.AddAction("/querymeta/byid", Handlers.QueryMetadata.ProcessContext);
			api.AddAction("/vaporise/user/bynickname", Handlers.DeleteUserByNickname.ProcessContext);

			api.Listen ();
		}
	}
}