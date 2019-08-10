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
				if (input == null)
				{
					return;
				}
				if (step == 0)
				{
					_startParams = JsonConvert.DeserializeObject<GameParams<WorldStartParams>>(input).@params;
					CollisionHelper.SetStartParams(_startParams);
					DirectionHelper.SetStartParams(_startParams);
				} else if (step == 1)
				{
					_currentParams = JsonConvert.DeserializeObject<GameParams<WorldTickParams>>(input);
					direction = DirectionHelper.FindFirstDirection(_currentParams.@params);
					Console.WriteLine("{{\"command\": \"{0}\"}}", direction);
				}
				else
				{
					_currentParams = JsonConvert.DeserializeObject<GameParams<WorldTickParams>>(input);
					if (_currentParams.type == "end_game")
					{
						return;
					}
					direction = DirectionHelper.FindNewDirection(_currentParams.@params);
					Console.WriteLine("{{\"command\": \"{0}\"}}", direction);

				}

				GameLogger.Log("Iteration #" + step);
				GameLogger.Log(input);
				GameLogger.Log(direction);
				GameLogger.SavePartialLogs();
				
				step++;
			}
		}
	}
}
