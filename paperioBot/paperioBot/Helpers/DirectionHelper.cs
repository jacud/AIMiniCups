using paperioBot.InternalClasses;
using paperioBot.Strategies;
using System;

namespace paperioBot.Helpers
{
	internal static class DirectionHelper
	{
		#region Properties

		private static readonly string[] _directions = new[] { "left", "right", "up", "down" };

		private static readonly Strategy _strategy = new CrazyRandomStrategy();

		public static int DirectionsCount => _directions.Length;

		private static WorldStartParams _startParams;

		public static int FirstWay { get; private set; } = -1;

		public static int[] CorrectionDeltas { get; private set; } = new[] {0, 0};

		#endregion

		private static void CalculateDeltas()
		{
			var x = FirstWay < 2 ? -_startParams.speed * (FirstWay * 2 - 1) : 0;
			var y = FirstWay > 2 ? _startParams.speed * (FirstWay * 2 - 5) : 0;
		}

		private static State GetMyPosition(WorldTickParams tickParams)
		{
			var myState = new State();
			var me = tickParams.players["i"];
			myState.position = me.position;
			myState.direction = me.direction;
			return myState;
		}

		public static void SetStartParams(WorldStartParams startParams)
		{
			_startParams = startParams;
		}


		public static int Way(string direction)
		{
			return Array.IndexOf(_directions,direction);
		}

		public static string Direction(int way)
		{
			return _directions[way];
		}

		public static bool CheckIsDirectionsComplanar(int firstDirection, int secondDirection)
		{
			return (firstDirection / 2 == secondDirection / 2) && (firstDirection != secondDirection);
		}

		public static int GetComplanarWay(string direction)
		{
			var way = Way(direction);
			switch (way)
			{
				case 0: return 1;
				case 1: return 0;
				case 2: return 3;
				case 3: return 2;
				default: return -1;
			}
		}

		public static bool CheckIsDirectionsComplanar(string firstDirection, string secondDirection)
		{
			return CheckIsDirectionsComplanar(Way(firstDirection), Way(secondDirection));
		}

		public static string FindFirstDirection(WorldTickParams currentParams)
		{

			if (currentParams == null)
			{
				return null;
			}
			_strategy.MutateStates(currentParams);
			var currentState = GetMyPosition(currentParams);
			var newWay = _strategy.SelectFirstDirection(_startParams, currentState);
			FirstWay = newWay;
			return Direction(newWay);
		}

		public static string FindNewDirection(WorldTickParams currentParams)
		{

			if (currentParams == null)
			{
				return null;
			}
			_strategy.MutateStates(currentParams);
			var currentState = GetMyPosition(currentParams);
			var newWay = _strategy.SelectDirection(_startParams, currentState);

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
