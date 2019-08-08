using Newtonsoft.Json;
using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
					direction = DirectionHelper.Direction(0);
					Console.WriteLine("{{\"command\": \"{0}\"}}", direction);
				}
				else
				{
					try
					{
						_currentParams = JsonConvert.DeserializeObject<GameParams<WorldTickParams>>(input);

						int nextDirection;
						for (nextDirection = 0; nextDirection < 4; nextDirection++)
						{
							var me = _currentParams.@params.players["i"];
							var nextPos = GetNextPos(me.position, nextDirection);
							if (CheckIsInBound(nextPos) && !CheckInTail(nextPos, me.lines.ToArray()))
							{
								break;
							}
						}

						direction = DirectionHelper.Direction(nextDirection);
						Console.WriteLine("{{\"command\": \"{0}\"}}", direction);
					}
					catch (Exception)
					{
					}
				}

				GameLogger.Log("Iteration #" + step);
				GameLogger.Log(input);
				GameLogger.Log(direction);
				GameLogger.SavePartialLogs();
				
				
				step++;
			}
		}

		private static int[] GetNextPos(int[] pos, int direction)
		{
			switch (direction)
			{
				case 0: return new[] {pos[0] - 30, pos[1]};
				case 1: return new[] {pos[0] + 30, pos[1]};
				case 2: return new[] {pos[0], pos[1] + 30};
				case 3: return new[] {pos[0], pos[1] - 30};
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private static bool CheckIsInBound(int[] point)
		{
			const int cellSize = 30;
			const int width = 930;
			const int height = 930;
			return point[0] >= cellSize / 2 && point[0] <= width - cellSize / 2 && point[1] >= cellSize / 2 &&
			       point[1] <= height - cellSize / 2;
		}

		private static bool CheckInTail(int[] point, int[][] tail)
		{
			return tail.FirstOrDefault(x => x[0] == point[0] && x[1] == point[1]) != null;
		}
	}
}
