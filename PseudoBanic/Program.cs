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

			api.AddAction("/register", Handlers.Register.ProcessContext);
			api.AddAction("/retrieve", Handlers.Retrieve.ProcessContext);

			api.Listen ();
		}
	}
}