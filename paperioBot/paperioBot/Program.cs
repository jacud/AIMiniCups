using System;
using Newtonsoft.Json;
using paperioBot.InternalClasses;

namespace paperioBot
{
	class PaperioBot
	{
		static void Main(string[] args)
		{
			var commands = new string[4] { "left", "right", "up", "down" };
			Random random = new Random();
			while (true)
			{
				var input = Console.ReadLine();
				//var inputObj = JsonConvert.DeserializeObject<GameParams>(input);
				GameLogger.Log(input);
				GameLogger.SavePartialLogs();
				int index = random.Next(0, commands.Length);
				Console.WriteLine("{{\"command\": \"{0}\"}}", commands[index]);
			}
		}
	}
}
