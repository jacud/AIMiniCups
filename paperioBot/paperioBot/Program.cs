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

		#region Methods

		private static void GetMyPosition()
		{
			var me = _currentParams.@params.players["i"];
			myState.position = me.position;
			myState.direction = me.direction;
		}

		#endregion

		static void Main(string[] args)
		{
			var isFirstStep = true;
			string direction = null;
			while (true)
			{
				var input = Console.ReadLine();
				if (isFirstStep)
				{
					_startParams = JsonConvert.DeserializeObject<GameParams<WorldStartParams>>(input).@params;
					CollisionHelper.SetStartParams(_startParams);
					direction = CollisionHelper.FindNewDirection(null, null);
					isFirstStep = false;
				}
				else
				{
					try
					{
						_currentParams = JsonConvert.DeserializeObject<GameParams<WorldTickParams>>(input);
						GetMyPosition();
						direction = CollisionHelper.FindNewDirection(myState, _currentParams);
					}
					catch (Exception)
					{
					}
				}
				
				GameLogger.Log(input);
				GameLogger.SavePartialLogs();
				
				Console.WriteLine("{{\"command\": \"{0}\"}}", direction);
			}
		}
	}
}
