using Newtonsoft.Json;
using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;

namespace paperioBot
{
	class PaperioBot
	{
		#region Properties

		private static WorldStartParams _startParams;

		private static GameParams<WorldTickParams> _currentParams;

		private static State myState = new State();

		#endregion

		static void Main(string[] args)
		{
			//GameLogger.On();
			var step = 0;
			string direction = null;
			while (true)
			{
				var input = Console.ReadLine();
				if (step == 0)
				{
					_startParams = JsonConvert.DeserializeObject<GameParams<WorldStartParams>>(input).@params;
					CollisionHelper.SetStartParams(_startParams);
					DirectionHelper.SetStartParams(_startParams);
				} else if (step == 1)
				{
					_currentParams = JsonConvert.DeserializeObject<GameParams<WorldTickParams>>(input);
					direction = DirectionHelper.FindFirstDirection(_currentParams.@params);
				}
				else
				{
					try
					{
						_currentParams = JsonConvert.DeserializeObject<GameParams<WorldTickParams>>(input);
						direction = DirectionHelper.FindNewDirection(_currentParams.@params);
					}
					catch (Exception)
					{
					}
				}

				GameLogger.Log("Iteration #" + step);
				GameLogger.Log(input);
				GameLogger.Log(direction);
				GameLogger.SavePartialLogs();
				
				Console.WriteLine("{{\"command\": \"{0}\"}}", direction);
				step++;
			}
		}
	}
}
