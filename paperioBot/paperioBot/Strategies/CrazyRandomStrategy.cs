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
			var pathFinder = new HunterPathFinder(startParams, CurrentTickParams);
			var path = pathFinder.BuildWeightsAndReturnWay(false);
			return path.Length >= 2 ? path[1] : random.Next(0, DirectionHelper.DirectionsCount);
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
					var isAssult = CurrentTickParams.players["i"].territory.Any(t => t[0] == currentState.position[0] && t[1] == currentState.position[1]);
					var path = pathFinder.BuildWeightsAndReturnWay(!isAssult);
					index = path?.FirstOrDefault() ?? random.Next(0, DirectionHelper.DirectionsCount);
					var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), startParams.width, currentState);
					if (newState != null)
					{
						isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, CurrentTickParams);
						if (isSuicide)
						{
							var newIndex = DirectionHelper.Way(newState.direction);
							if (newIndex != index)
							{
								forbiddenDirections.Add(index);
							}
							index = newIndex;
						}
					}
				}
			}

			return index;
		}
	}
}
