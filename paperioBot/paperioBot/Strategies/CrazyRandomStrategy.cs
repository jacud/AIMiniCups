using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace paperioBot.Strategies
{
	class CrazyRandomStrategy : Strategy
	{
		private static Random random = new Random();

		public CrazyRandomStrategy() : base() { }

		public CrazyRandomStrategy(WorldTickParams currentTickParams) : base(currentTickParams) {}

		public override int SelectFirstDirection(WorldStartParams startParams, State currentState)
		{
			return random.Next(0, DirectionHelper.DirectionsCount);
		}

		public override int SelectDirection(WorldStartParams startParams, State currentState)
		{
			int index = random.Next(0, DirectionHelper.DirectionsCount);
			if (currentState != null && CurrentTickParams!= null)
			{
				var forbiddenDirections = new List<int>{DirectionHelper.Way(currentState.direction)};
				var isSuicide = true;
				while (isSuicide && forbiddenDirections.Count < DirectionHelper.DirectionsCount)
				{
					var pathFinder = new HunterPathFinder(startParams, CurrentTickParams);
					var path = pathFinder.BuildWeightsAndReturnWay().Reverse().ToArray();
					index = path.Length > 2 ? path[1] : random.Next(0, DirectionHelper.DirectionsCount);
					var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), startParams.width, currentState);
					if (newState != null)
					{
						isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, CurrentTickParams);
						var newIndex = DirectionHelper.Way(newState.direction);
						if (newIndex != index)
						{
							forbiddenDirections.Add(index);
						}

						index = newIndex;
					}
				}
			}

			return index;
		}
	}
}
