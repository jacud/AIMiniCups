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

		private static bool _isActive = false;

		public static void On()
		{
			_isActive = true;
		}

		public static void Log(object obj)
		{
			if (!_isActive)
			{
				return;
			}

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
			if (!_isActive)
			{
				return;
			}

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
			if (!_isActive)
			{
				return;
			}

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
