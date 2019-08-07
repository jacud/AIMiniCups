using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;
using System.Collections.Generic;

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
				var forbiddenDirections = new List<int>();
				var isSuicide = true;
				while (isSuicide)
				{
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

					if (isSuicide && forbiddenDirections.Count < DirectionHelper.DirectionsCount)
					{
						forbiddenDirections.Add(index);
						while (forbiddenDirections.Contains(index))
						{
							index = random.Next(0, DirectionHelper.DirectionsCount);
						}
					}
				}
			}

			return index;
		}
	}
}
