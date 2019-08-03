using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace paperioBot.Helpers
{
	internal static class GameLogger
	{
		private static List<string> logs = new List<string>();

		private static string FileName = DateTime.UtcNow.Ticks.ToString() + ".log";

		public static void Log(object obj)
		{
			logs.Add(JsonConvert.SerializeObject(obj));
			logs.Add(Environment.NewLine);
		}

		public static void Log(string obj)
		{
			logs.Add(JsonConvert.SerializeObject(obj));
			logs.Add(Environment.NewLine);
		}

		public static void SaveLogs()
		{
			if (!File.Exists(FileName))
			{
				using (StreamWriter sw = File.CreateText(FileName))
				{
					logs.ForEach(s =>
					{
						sw.WriteLine(s);
					});
				}
			}
		}

		public static void SavePartialLogs()
		{
			if (!File.Exists(FileName))
			{
				using (StreamWriter sw = File.CreateText(FileName))
				{
					logs.ForEach(s =>
					{
						sw.WriteLine(s);
					});
				}
			}
			else
			{
				using (StreamWriter sw = File.AppendText(FileName))
				{
					logs.ForEach(s =>
					{
						sw.WriteLine(s);
					});
				}
			}

			logs.Clear();
		}
	}
}
