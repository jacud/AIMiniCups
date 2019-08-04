using paperioBot.InternalClasses;
using paperioBot.Strategies;
using System;

namespace paperioBot.Helpers
{
	internal static class DirectionHelper
	{
		#region Properties

		private static string[] directions = new[] { "left", "right", "up", "down" };

		private static Strategy _strategy = new GreedyCubeStrategy();

		public static int DirectionsCount => directions.Length;

		private static WorldStartParams _startParams;

		public static int FirstWay { get; private set; } = -1;

		public static int[] CorrectionDeltas { get; private set; } = new[] {0, 0};

		#endregion

		private static void CalculateDeltas()
		{
			var x = FirstWay < 2 ? -_startParams.speed * (FirstWay * 2 - 1) : 0;
			var y = FirstWay > 2 ? _startParams.speed * (FirstWay * 2 - 5) : 0;
		}

		public static void SetStartParams(WorldStartParams startParams)
		{
			_startParams = startParams;
		}


		public static int Way(string direction)
		{
			return Array.IndexOf(directions,direction);
		}

		public static string Direction(int way)
		{
			return directions[way];
		}

		public static bool CheckIsDerectionsComplanar(int firstDirection, int secondDirection)
		{
			return (firstDirection / 2 == secondDirection / 2) && (firstDirection != secondDirection);
		}

		public static bool CheckIsDerectionsComplanar(string firstDirection, string secondDirection)
		{
			return CheckIsDerectionsComplanar(Way(firstDirection), Way(secondDirection));
		}

		public static string FindNewDirection(State currentState, GameParams<WorldTickParams> currentParams)
		{

			if (currentParams == null)
			{
				return null;
			}
			_strategy.MutateStates(currentParams.@params);
			var newWay = _strategy.SelectDirection(_startParams, currentState);
			if (FirstWay == -1)
			{
				FirstWay = newWay;
			}
			return Direction(newWay);
		}

		public static int GetDirectionToPosition(State currentState, int[] cell)
		{
			if (currentState.position[0] == cell[0])
			{
				if (cell[1] > currentState.position[1])
				{
					return 2;
				} else {
					return 3;
				}
			} else if (currentState.position[1] == cell[1])
			{
				if (cell[0] > currentState.position[0])
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

			throw new ArgumentOutOfRangeException();
		}
	}
}
