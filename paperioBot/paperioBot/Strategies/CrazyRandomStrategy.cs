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

		public override int SelectDirection(WorldStartParams startParams, State currentState, WorldTickParams currentParams)
		{
			int index = random.Next(0, DirectionHelper.DirectionsCount);
			if (currentState != null && currentParams != null)
			{
				var forbiddenDirections = new List<int>();
				var isSuicide = true;
				while (isSuicide)
				{
					var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), startParams.width, currentState);
					if (newState != null)
					{
						index = DirectionHelper.Way(newState.direction);
						isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, currentParams);
					}
					else
					{
						isSuicide = true;
					}
					if (isSuicide && forbiddenDirections.Count < DirectionHelper.DirectionsCount)
					{
						forbiddenDirections.Add(index);
						while (forbiddenDirections.Contains(index))
						{
							index = random.Next(0, DirectionHelper.DirectionsCount);
						}
					}

					if (forbiddenDirections.Count < DirectionHelper.DirectionsCount)
					{
						isSuicide = false;
					}
				}
			}

			return index;
		}
	}
}
